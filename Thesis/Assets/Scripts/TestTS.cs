using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsAPI;
using TsAPI.Types;
using TsSDK;
using System;
using UnityEditor;
using System.IO;
using System.Text;

public class TestTS : MonoBehaviour
{
    public Transform[] joints;

    [SerializeField]
    private TsMotionProvider m_motionProvider;
    [SerializeField]
    private TsHumanAnimator _tsHumanAnimator;
    private ISkeleton skeleton;
    public ISkeleton Skeleton { get { return skeleton; } }

    JointRotationCalculator jointRotation;
    //path to extract infos
    private string path = string.Empty;

    //recording boolean
    [HideInInspector] bool _shouldSave;
    //recording string
    StringBuilder stringBuilder = new StringBuilder();
    void Start()
    {
        jointRotation = new JointRotationCalculator();
        //reference to assets folder
        path = Application.dataPath+ @"/ExtractedFiles";
        Debug.Log(path);
     
    }

   
    void Update()
    {
        skeleton = m_motionProvider.GetSkeleton(Time.time);
        jointRotation.UpdateSkeleton(skeleton);
        //Debug.Log(jointRotation.CalculateAngles(JointRotationIndex.LeftUpperLeg_LeftLowerLeg, JointRotationIndex.LeftLowerLeg_LeftFoot));
        DrawGizmos();
    }
    public void RecordCollarBones(bool _shouldsave)
    {
        stringBuilder= new StringBuilder();
        _shouldSave = _shouldsave;
        StartCoroutine(StartRecordingCollarBones(stringBuilder));
    }
    public void PutMark()
    {
        if(stringBuilder!=null) stringBuilder.Append("------------------------------------");stringBuilder.AppendLine();
    }
    IEnumerator StartRecordingCollarBones(StringBuilder stringBuilder)
    {
       
        stringBuilder.Append("----12---"); stringBuilder.Append("----13---"); stringBuilder.AppendLine();
        while (_shouldSave)
        {
            TsTransform boneTransform;

            if (skeleton != null && skeleton.GetBoneTransform((TsHumanBoneIndex)12, out boneTransform))
            {
                stringBuilder.Append( Conversion.TsRotationToUnityRotation(boneTransform.rotation).ToString("F4"));
                stringBuilder.Append(' ');

            }
            if (skeleton != null && skeleton.GetBoneTransform((TsHumanBoneIndex)13, out boneTransform))
            {
                stringBuilder.Append(Conversion.TsRotationToUnityRotation(boneTransform.rotation).ToString("F4"));
                stringBuilder.AppendLine();

            }

            yield return new WaitForEndOfFrame();
        }
        Extractor.WriteToFile("CollarBoneRotations", stringBuilder);

    }
    public void CalculateJointDifferences()
    {
        Dictionary<TsHumanBoneIndex, float> dict = new Dictionary<TsHumanBoneIndex, float>();
        //foreach (KeyValuePair<TsHumanBoneIndex, Transform> kvp in _tsHumanAnimator.BoneTransforms)
        //{
        //    Debug.Log((int)kvp.Key + " " + kvp.Value);
        //}
        //return;
        //var values = Enum.GetValues(typeof(TsHumanBoneIndex));
        int count = 0;
        foreach (TsHumanBoneIndex i in TsHumanBones.SuitBones)
        {
            TsTransform boneTransform;

            if (skeleton != null && skeleton.GetBoneTransform(i, out boneTransform))
            {
                Debug.Log((int )i);
                count++;
                dict.Add(i, Vector3.Distance(Conversion.TsVector3ToUnityVector3(boneTransform.position), _tsHumanAnimator.BoneTransforms[i].position));

            }
        }
        
        StringBuilder stringBuilder= new StringBuilder();
        foreach(KeyValuePair<TsHumanBoneIndex,float> kvp in dict)
        {
            stringBuilder.Append(kvp.Key);
            stringBuilder.Append(": ");
            stringBuilder.Append(kvp.Value);
            stringBuilder.AppendLine();
            Debug.Log((int)kvp.Key +" "+kvp.Value );
        }
        File.WriteAllText((path + @"/MyFile.txt"), stringBuilder.ToString());
       
    }
    //Draws suit body structure
    private void DrawGizmos()
    {
        return;
        if (skeleton == null) return;
        Debug.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)14).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)12).position), Color.green);
        Debug.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)15).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position), Color.green);
        Debug.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position), Color.green);
        Vector3 Hips_LeftUpperLeg = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position)- Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position);
        Vector3 Hips_RightUpperLeg= Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position);
        Vector3  LeftUpperLeg_LeftLowerLeg= Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position); 
        Vector3 RightUpperLeg_RightLowerLeg=Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)4).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position);
        Vector3 LeftLowerLeg_LeftFoot = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)5).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position);

        //Debug.Log(Vector3.Angle(LeftUpperLeg_LeftLowerLeg, LeftLowerLeg_LeftFoot));
        //Debug.Log(Vector3.Angle(Hips_LeftUpperLeg, LeftUpperLeg_LeftLowerLeg));

        //Vector3 LeftLowerLeg_LeftFoot=Vector3.zero;
        //Vector3 RightLowerLeg_RightFoot=Vector3.zero;
        //Vector3 Hips_Spine=Vector3.zero;
        //Vector3 Spine_Chest=Vector3.zero;
        //Vector3 Chest_UpperSpine=Vector3.zero;
        //Vector3  UpperSpine_LeftShoulder=Vector3.zero;
        //Vector3  UpperSpine_RightShoulder=Vector3.zero;
        //Vector3  LeftShoulder_LeftUpperArm=Vector3.zero;
        //Vector3  RightShoulder_RightUpperArm=Vector3.zero;
        //Vector3 LeftUpperArm_LeftLowerArm=Vector3.zero;
        //Vector3  RightUpperArm_RightLowerArm=Vector3.zero;
        //Vector3  LeftLowerArm_LeftHand=Vector3.zero;
        //Vector3  RightLowerArm_RightHand=Vector3.zero;
    }
    private void OnDrawGizmos()
    {
       
        Color a = Color.red; a.a = 128;
        Gizmos.color = a;
        int count = 0;
        foreach(TsHumanBoneIndex i in TsHumanBones.SuitBones)
        {
            TsTransform boneTransform;

        //if (skeleton !=null && skeleton.GetBoneTransform(i,out boneTransform) && ((int)i==0 || (int)i == 1))
        if (skeleton !=null && skeleton.GetBoneTransform(i,out boneTransform) )
            {
               Gizmos.DrawSphere( Conversion.TsVector3ToUnityVector3( boneTransform.position), 0.01f);  
               //count++;
               //Debug.Log(Conversion.TsVector3ToUnityVector3(boneTransform.position).ToString("F4") + " "+ (int)i);
               //Debug.Log(boneTransform.rotation + " "+ (int)i);
               //Debug.Log(boneTransform.position + " "+ (int)i);
               //Debug.Log(transform.TransformPoint(Conversion.TsVector3ToUnityVector3(boneTransform.position)) + " "+ (int)i);
            }
        }
        //Debug.Log(count);
        count = 0;

        //8 is chest and not being considered
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)5).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)4).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)4).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)6).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)7).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)7).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)12).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)15).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)15).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)17).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)17).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)19).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)12).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)14).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)14).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)16).position));
        Gizmos.DrawLine(Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)16).position), Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)18).position));


    }


}

[CustomEditor(typeof(TestTS))]
public class TestTSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestTS _testts = (TestTS)target;
        if (GUILayout.Button("Calculate Joint Differences"))
        {
            _testts.CalculateJointDifferences();
        }
        GUILayout.BeginHorizontal("box");

        if (GUILayout.Button("Start"))
        {
            _testts.RecordCollarBones(true);
        }
        if (GUILayout.Button("Mark"))
        {
            _testts.PutMark();
        }
        if (GUILayout.Button("End"))
        {
            _testts.RecordCollarBones(false);
        }

        GUILayout.EndHorizontal();
       
       
    }
}

