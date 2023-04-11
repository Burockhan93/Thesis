using System;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;
using UnityEditor;
using System.Text;

public class TsHumanAnimator : MonoBehaviour
{
    public bool Controller;
    [Tooltip("For visualisation and Calculation purposes needed")]
    public GameObject[] boxes;
    [Header("Joint locations, the order is important and is the same with TsHumanBoneIndex enum")]
    [SerializeField]
    private Transform[] _joints;

    private JointLocationUpdater _jointlocationUpdater;
    [Tooltip("Root transform of rig, needed to find joint locations of smplx")]
    public Transform Root;
   
    private bool _correctShoulder;
    private bool _correctElbow;
    public GameObject leftShoulder;
    public GameObject rightShoulder;
    private JointRotationCalculator _jointRotationCalculator = new JointRotationCalculator();
    private TsUI _ui;
    //Holds the initial suit skeleton positions-in case it is needed
    private InitialSkeletonPositions _initialPositions = new InitialSkeletonPositions();
    //Updates the local joint positions of smplx.
    
    [SerializeField]
    private TsMotionProvider m_motionProvider;

    [SerializeField]
    private TsAvatarSettings m_avatarSettings;

    private Dictionary<TsHumanBoneIndex, Transform> m_bonesTransforms = new Dictionary<TsHumanBoneIndex, Transform>();
    public Dictionary<TsHumanBoneIndex, Transform> BoneTransforms { get { return m_bonesTransforms; } }
    private TsHumanBoneIndex m_rootBone = TsHumanBoneIndex.Hips;

    private ISkeleton skeleton; 
    public ISkeleton Skeleton { get { return skeleton; } }

    [Range(0,1.1f)] public float coefficientrz;
    [Range(0,1.1f)] public float coefficientlz;
    [Range(0,1.1f)] public float coefficienty;
    [Range(0,1.1f)] public float coefficientx;

    //Calibrate
    private bool _isCalibrated = false;
    //General timer
    private float _timer;
    //bu degisecek isyerindeki scriptle ayni olmasi lazm
    public bool Replay;
    private void Start()
    {
        _jointlocationUpdater = new JointLocationUpdater(Root);

        _ui= FindObjectOfType<TsUI>();


        if (m_avatarSettings == null)
        {
            Debug.LogError("Missing avatar settings for this character.");
            enabled = false;
            return;
        }

        if(!m_avatarSettings.IsValid)
        {
            Debug.LogError("Invalid avatar settings for this character. Check that all required bones is configured correctly.");
            enabled = false;
            return;
        }

        SetupAvatarBones();
        
    }

    public void SetupAvatarBones()
    {
        foreach (var reqBoneIndex in TsHumanBones.SuitBones)
        {
            var transformName = m_avatarSettings.GetTransformName(reqBoneIndex);
            var boneTransform = TransformUtils.FindChildRecursive(transform, transformName);
            if (boneTransform != null && !m_bonesTransforms.ContainsKey(reqBoneIndex))
            {
                m_bonesTransforms.Add(reqBoneIndex, boneTransform);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //DrawGizmos();
        skeleton = m_motionProvider.GetSkeleton(Time.time);
        if (!Replay) Update(skeleton);
        else
        {

        }

        if (!Controller) return;
        if(Input.GetKey(KeyCode.C)) _isCalibrated = true;
        if (Input.GetKey(KeyCode.S)) _correctShoulder = true;
        if (Input.GetKey(KeyCode.E)) _correctElbow = true;
        if (Input.GetKey(KeyCode.R)) { _correctShoulder = false; _correctElbow = false; ResetJoints(); }
        if(Input.GetKey(KeyCode.B)) foreach (GameObject g in boxes) g.SetActive(!g.active);
    }
    public void ReplayUpdate(ReplayInfo ri)
    {
        ////pos + euler rot(Uncomment this section if you want to replay using both positions(vec3) and rotations(vec3) )
        //foreach (var boneIndex in TsHumanBones.SuitBones)
        //{
        //    var poseRotation = Quaternion.identity;
        //    var targetRotation = Vector3.zero;
        //    //var targetRotation = Quaternion.identity; 
        //   if (ri.replayRotation.ContainsKey(boneIndex))
        //    {
        //        poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //        targetRotation = ri.replayRotation[boneIndex];
        //    }
        //    else
        //    {
        //        //Debug.Log("bulunamadi: " + boneIndex.ToString());
        //        continue;
        //    }

        //    TryDoWithBone(boneIndex, (boneTransform) =>
        //    {
        //        boneTransform.rotation = Quaternion.Euler( targetRotation.x, targetRotation.y, targetRotation.z);
        //        boneTransform.position = ri.replayPosition[boneIndex];
        //    });
        //}

        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //   // var hipsPos = m_motionProvider.GetSkeleton(Time.time).GetBoneTransform(TsHumanBoneIndex.Hips).position;
        //   // boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];
        //});
        Debug.Log("Hey");
        //quaternion
        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            //var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
            MyQuaternion my;
            Quaternion targetRotation;
            if (ri.replayRotationQuaternion.TryGetValue(boneIndex, out my))
            {
                 targetRotation = MyQuaternion.ConvertToQuat(my);
            }
            else continue;


            TryDoWithBone(boneIndex, (boneTransform) =>
            {
                boneTransform.rotation = targetRotation;
            });
        }
        ////it is important to note that, changing objects transform.position overrides collisions.
        ///
        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //    boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];
        //});

        //Debug.Log(ri.replayPosition[TsHumanBoneIndex.Hips].y);
        //TryDoWithBone(m_rootBone, (boneTransform) =>
        //{
        //    if (ri.replayPosition[TsHumanBoneIndex.Hips].y < 1)
        //    {
        //        boneTransform.transform.position = ri.replayPosition[TsHumanBoneIndex.Hips];

        //    }
        //    else
        //    {
        //        boneTransform.transform.position = new Vector3(ri.replayPosition[TsHumanBoneIndex.Hips].x, 1, ri.replayPosition[TsHumanBoneIndex.Hips].z);
        //    }

        //});

        //if (firstHipPosition.HasValue)
        //{
        //    TryDoWithBone(m_rootBone, (boneTransform) =>
        //    {
        //        // if(ri.replayPosition[TsHumanBoneIndex.Hips].y<1)
        //        boneTransform.transform.position = new Vector3(ri.replayPosition[TsHumanBoneIndex.Hips].x,
        //            Mathf.Lerp(firstHipPosition.Value, ri.replayPosition[TsHumanBoneIndex.Hips].y,Time.deltaTime*0.1f),
        //            ri.replayPosition[TsHumanBoneIndex.Hips].z);
        //        firstHipPosition = boneTransform.position.y;

        //    });

        //}
        //else
        //{
        //    firstHipPosition = ri.replayPosition[TsHumanBoneIndex.Hips].y;
        //}       


    }

    private void Update(ISkeleton skeleton)
    {
        
        
        if (skeleton == null)
        {
            return;
        }


       

        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            if ((int)boneIndex == 8 || (int)boneIndex == 10 || (int)boneIndex == 11 || (int)boneIndex >19) continue;
           
            var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
           
            var targetRotation = Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform(boneIndex).rotation);

            if (_correctShoulder)
            {
                UpdateJoints();
            }

            if(_correctElbow)
            {

                if (boneIndex == TsHumanBoneIndex.LeftLowerArm)
                {
                    float angle = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.LeftUpperArm, TsHumanBoneIndex.LeftLowerArm, TsHumanBoneIndex.LeftHand);


                    coefficientlz = Mathf.Pow(100, 3) / Mathf.Pow(angle, 3);
                    coefficientlz = Mathf.Clamp(coefficientlz, 0, 1f);

                    TryDoWithBone(boneIndex, (boneTransform) =>
                    {
                        boneTransform.rotation = targetRotation * poseRotation;
                        float y = _joints[16].localEulerAngles.y;
                        float z = _joints[16].localEulerAngles.z;

                        if (y > 180) y = 0;
                        if (z > 90) z = 0;

                        _joints[16].localEulerAngles = new Vector3(_joints[16].localEulerAngles.x * coefficientx, y * coefficienty, z * coefficientlz);
                    });
                    continue;
                }

                
                if (boneIndex == TsHumanBoneIndex.RightLowerArm)
                {

                    float angle = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.RightUpperArm, TsHumanBoneIndex.RightLowerArm, TsHumanBoneIndex.RightHand);
                   

                    coefficientrz = (Mathf.Pow(100, 3) / Mathf.Pow(angle, 3));
                    coefficientrz = Mathf.Clamp(coefficientrz, 0, 1f);

               
                    TryDoWithBone(boneIndex, (boneTransform) =>
                    {
                        boneTransform.rotation = targetRotation * poseRotation;
                        float y = 360 - _joints[17].localEulerAngles.y;
                        float z =  360 -_joints[17].localEulerAngles.z;

                        if (y > 180) y = 0;
                        if (z > 180) z = 0;

                        _joints[17].localEulerAngles = new Vector3(_joints[17].localEulerAngles.x * coefficientx,- y * coefficienty, (-z)*coefficientrz );
                    });
                    continue;

                }
            }
            

            TryDoWithBone(boneIndex, (boneTransform) =>
            {
              boneTransform.rotation = targetRotation * poseRotation; 
            });

            
        }
       // _ui.leftcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.LeftUpperArm, TsHumanBoneIndex.LeftLowerArm, TsHumanBoneIndex.LeftHand).ToString("F4");
       //_ui.rightcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.RightUpperArm, TsHumanBoneIndex.RightLowerArm, TsHumanBoneIndex.RightHand).ToString("F4");




        TryDoWithBone(m_rootBone, (boneTransform) =>
        {
            var hipsPos = skeleton.GetBoneTransform(TsHumanBoneIndex.Hips).position;
            boneTransform.transform.position = Conversion.TsVector3ToUnityVector3(hipsPos);
        });

        if (_isCalibrated)
        {
            _timer += Time.deltaTime;
            _ui.CalibrationCountDown.text = "WAIT..." + "\n" + (3 - (int)_timer).ToString();
            if (_timer > 2)
            {
                Calibrate();

            }
            if (_timer > 3)
            {
                //get the calibrated skeleton of suit
                if (_initialPositions.initialized == false)
                    _initialPositions = new InitialSkeletonPositions(skeleton, transform);

                _timer = 0;
                _isCalibrated = false;
                _ui.CalibrationCountDown.text = string.Empty;
            }
        }


    }

    public void Calibrate()
    {
        
        //TestTS ts = FindObjectOfType<TestTS>();

        //StringBuilder s1 = new StringBuilder();
        //StringBuilder s2 = new StringBuilder();
        m_motionProvider?.Calibrate();
        //for (int i = 0; i < 20; i++)// 11 10 continue nech and head
        //{
        //    if (i == 11 || i == 10 || i==8) continue;
        //    s1.Append((TsHumanBoneIndex)i); s1.Append(": ");
        //    s1.Append(Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform((TsHumanBoneIndex)i).rotation).eulerAngles);
        //    s1.Append(" "); s1.Append(m_bonesTransforms[(TsHumanBoneIndex)i].eulerAngles);
        //    s1.Append(" "); s1.Append(ts.joints[i].rotation.eulerAngles);
          
        //    s1.AppendLine();
        //}
        //Extractor.WriteToFile("Suit_Body_GameObject", s1);
        
        //foreach (var boneIndex in TsHumanBones.SuitBones)
        //{
        //    //var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //    var targetRotation = Conversion.TsRotationToUnityRotation(skeleton.GetBoneTransform(boneIndex).rotation);
        //   Debug.Log((int)boneIndex+ ": "+targetRotation.eulerAngles);

        //}
        foreach (KeyValuePair<TsHumanBoneIndex, Transform> kvp in m_bonesTransforms) 
        {
            //var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
            //Debug.Log((int)kvp.Key + ": " + kvp.Value.eulerAngles);
            // Debug.Log(targetRotation.eulerAngles);

        }
       

        
    }
    private void UpdateJoints()
    {
        _joints[14].localPosition = _jointlocationUpdater.UpdateShoulderLocations(TsHumanBoneIndex.LeftUpperArm, _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.Spine, TsHumanBoneIndex.LeftShoulder, TsHumanBoneIndex.LeftUpperArm),
            _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.LeftShoulder, TsHumanBoneIndex.LeftUpperArm, TsHumanBoneIndex.LeftLowerArm));

        _joints[15].localPosition = _jointlocationUpdater.UpdateShoulderLocations(TsHumanBoneIndex.RightUpperArm,_jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.Spine, TsHumanBoneIndex.RightShoulder, TsHumanBoneIndex.RightUpperArm),
           _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.RightShoulder, TsHumanBoneIndex.RightUpperArm, TsHumanBoneIndex.RightLowerArm));

        //Transform leftelbow;
        //m_bonesTransforms.TryGetValue(TsHumanBoneIndex.LeftLowerArm, out leftelbow); if(leftelbow) leftelbow.rotation=Quaternion.identity;
        //Transform rightelbow;
        //m_bonesTransforms.TryGetValue(TsHumanBoneIndex.RightLowerArm, out rightelbow); if(rightelbow) rightelbow.rotation=Quaternion.identity;
    }
    private void ResetJoints()
    {  
        for(int i =0; i < 20; i++)
        {
            if (i == 10 || i == 11) continue;
            _joints[i].localPosition = _jointlocationUpdater.ResetLocations(i);

        }      
    }

    private void TryDoWithBone(TsHumanBoneIndex boneIndex, Action<Transform> action)
    {
        if (!m_bonesTransforms.TryGetValue(boneIndex, out var boneTransform))
        {
            return;
        }
        action(boneTransform);
    }

    public static Quaternion HeadingOffset(Quaternion b)
    {
        Quaternion offset = Inverse(b, true, true, true);
        offset.x = 0f;
        offset.z = 0f;

        float mag = offset.w * offset.w + offset.y * offset.y;

        offset.w /= Mathf.Sqrt(mag);
        offset.y /= Mathf.Sqrt(mag);

        return offset;
    }
    private static Quaternion Inverse(Quaternion vector, bool X, bool Y, bool Z)
    {
        vector.x *= X ? -1f : 1f;
        vector.y *= Y ? -1f : 1f;
        vector.z *= Z ? -1f : 1f;
        return vector;
    }

    //Draws the smplx body structure
    private void DrawGizmos()
    {


        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)0].position, m_bonesTransforms[(TsHumanBoneIndex)1].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)1].position, m_bonesTransforms[(TsHumanBoneIndex)3].position, Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)3].position, m_bonesTransforms[(TsHumanBoneIndex)5].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)0].position, m_bonesTransforms[(TsHumanBoneIndex)2].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)2].position, m_bonesTransforms[(TsHumanBoneIndex)4].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)4].position, m_bonesTransforms[(TsHumanBoneIndex)6].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)0].position, m_bonesTransforms[(TsHumanBoneIndex)7].position,Color.green);
        //Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)7].position, m_bonesTransforms[(TsHumanBoneIndex)9].position,Color.green);
        // 7-8, 8-9  neglected
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)7].position, m_bonesTransforms[(TsHumanBoneIndex)8].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)8].position, m_bonesTransforms[(TsHumanBoneIndex)9].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)9].position, m_bonesTransforms[(TsHumanBoneIndex)12].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)9].position, m_bonesTransforms[(TsHumanBoneIndex)13].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)13].position, m_bonesTransforms[(TsHumanBoneIndex)15].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)15].position, m_bonesTransforms[(TsHumanBoneIndex)17].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)17].position, m_bonesTransforms[(TsHumanBoneIndex)19].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)12].position, m_bonesTransforms[(TsHumanBoneIndex)14].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)14].position, m_bonesTransforms[(TsHumanBoneIndex)16].position,Color.green);
        Debug.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)16].position, m_bonesTransforms[(TsHumanBoneIndex)18].position,Color.green);
    }
    private void OnDrawGizmos()
    {
        return;
        
        Color blue = new Color(0, 0, 1, 125);
        Gizmos.color = blue;

        foreach (KeyValuePair<TsHumanBoneIndex,Transform> kvp in m_bonesTransforms)
        {
            Gizmos.DrawSphere(kvp.Value.position, 0.01f);
            //Debug.Log((int)kvp.Key);
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 0].position, m_bonesTransforms[(TsHumanBoneIndex)1].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 1].position, m_bonesTransforms[(TsHumanBoneIndex)3].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 3].position, m_bonesTransforms[(TsHumanBoneIndex)5].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 0].position, m_bonesTransforms[(TsHumanBoneIndex)2].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 2].position, m_bonesTransforms[(TsHumanBoneIndex)4].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 4].position, m_bonesTransforms[(TsHumanBoneIndex)6].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 0].position, m_bonesTransforms[(TsHumanBoneIndex)7].position);
        //Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 7].position, m_bonesTransforms[(TsHumanBoneIndex)9].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)7].position, m_bonesTransforms[(TsHumanBoneIndex)8].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex)8].position, m_bonesTransforms[(TsHumanBoneIndex)9].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 9].position, m_bonesTransforms[(TsHumanBoneIndex)12].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 9].position, m_bonesTransforms[(TsHumanBoneIndex)13].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 13].position, m_bonesTransforms[(TsHumanBoneIndex)15].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 15].position, m_bonesTransforms[(TsHumanBoneIndex)17].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 17].position, m_bonesTransforms[(TsHumanBoneIndex)19].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 12].position, m_bonesTransforms[(TsHumanBoneIndex)14].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 14].position, m_bonesTransforms[(TsHumanBoneIndex)16].position);
        Gizmos.DrawLine(m_bonesTransforms[(TsHumanBoneIndex) 16].position, m_bonesTransforms[(TsHumanBoneIndex)18].position);
    }

    private void OnGUI()
    {
        //if (GUI.Button(new Rect(10, 70, 80, 45), "Boxes On/Off"))
        //{
        //    foreach(GameObject g in boxes) g.SetActive(!g.active);
        //}
        //if (GUI.Button(new Rect(10, 120, 80, 45), "Calibrate"))
        //{
        //    _isCalibrated = true;
        //}
        //if (GUI.Button(new Rect(10, 170, 80, 45), "Correct"))
        //{
        //    _correct = true;
        //}
        //if (GUI.Button(new Rect(100, 170, 80, 45), "Reset"))
        //{
        //    _correct = false; ResetJoints();
        //}
    }

}


/// <summary>
/// //this doesnt work because suits bones dont exist in the scene, thats why we can not convert them to local positions as childs of gameobject. I tried though
/// </summary>

[CustomEditor(typeof(TsHumanAnimator))]
public class TsHumanAnimatorEditor: Editor
{

    private void OnSceneGUI()
    {
        //    TsHumanAnimator _avatar = target as TsHumanAnimator;
        //    Transform _t = _avatar.transform;

        //    foreach (var boneIndex in TsHumanBones.SuitBones)
        //    {
        //        if ((int)boneIndex == 8) return;
        //        //var poseRotation = m_avatarSettings.GetIPoseRotation(boneIndex);
        //        var targetTransform = _avatar.Skeleton.GetBoneTransform(boneIndex);

        //        Quaternion handleRotation = Conversion.TsRotationToUnityRotation(targetTransform.rotation);
        //        Debug.Log(handleRotation);
        //       // handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleRotation : Quaternion.identity;
        //        Vector3 point = Conversion.TsVector3ToUnityVector3(targetTransform.position);
        //        Handles.DoPositionHandle(point, handleRotation);

        //    }
    }

}

