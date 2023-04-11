using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsAPI.Types;
using TsSDK;

public class JointRotationCalculator 
{
    private  Vector3 Hips_LeftUpperLeg = Vector3.zero;
    private  Vector3 Hips_RightUpperLeg = Vector3.zero;
    private  Vector3 LeftUpperLeg_LeftLowerLeg = Vector3.zero;
    private  Vector3 RightUpperLeg_RightLowerLeg = Vector3.zero;
    private  Vector3 LeftLowerLeg_LeftFoot = Vector3.zero;
    private  Vector3 RightLowerLeg_RightFoot = Vector3.zero;
    private  Vector3 Hips_Spine = Vector3.zero;
    private  Vector3 Spine_Chest = Vector3.zero;
    private  Vector3 Chest_UpperSpine = Vector3.zero;
    //specialfall while chest position is wrong, we are connecting spine directly to upperspine
    private  Vector3 Spine_UpperSpine = Vector3.zero;
    private  Vector3 UpperSpine_LeftShoulder = Vector3.zero;
    private  Vector3 UpperSpine_RightShoulder = Vector3.zero;
    private  Vector3 LeftShoulder_LeftUpperArm = Vector3.zero;
    private  Vector3 RightShoulder_RightUpperArm = Vector3.zero;
    private  Vector3 LeftUpperArm_LeftLowerArm = Vector3.zero;
    private  Vector3 RightUpperArm_RightLowerArm = Vector3.zero;
    private  Vector3 LeftLowerArm_LeftHand = Vector3.zero;
    private  Vector3 RightLowerArm_RightHand = Vector3.zero;
   
    private static ISkeleton _skeleton;

    private static Dictionary<JointRotationIndex, Vector3> _rotationDict;
    public static  Dictionary<JointRotationIndex,Vector3> RotationDictionary { get { return _rotationDict; } }
    public JointRotationCalculator()
    {
        _rotationDict = new Dictionary<JointRotationIndex, Vector3>
        {
            {JointRotationIndex.Hips_LeftUpperLeg,Hips_LeftUpperLeg },
            {JointRotationIndex.Hips_RightUpperLeg,Hips_RightUpperLeg },
            {JointRotationIndex.LeftUpperLeg_LeftLowerLeg,LeftUpperLeg_LeftLowerLeg },
            {JointRotationIndex.RightUpperLeg_RightLowerLeg,RightUpperLeg_RightLowerLeg },
            {JointRotationIndex.LeftLowerLeg_LeftFoot,LeftLowerLeg_LeftFoot },
            {JointRotationIndex.RightLowerLeg_RightFoot,RightLowerLeg_RightFoot },
            {JointRotationIndex.Hips_Spine,Hips_Spine },
            {JointRotationIndex.Spine_Chest,Spine_Chest },
            {JointRotationIndex.Chest_UpperSpine,Chest_UpperSpine },
            {JointRotationIndex.Spine_UpperSpine,Spine_UpperSpine },
            {JointRotationIndex.UpperSpine_LeftShoulder,UpperSpine_LeftShoulder },
            {JointRotationIndex.UpperSpine_RightShoulder,UpperSpine_RightShoulder },
            {JointRotationIndex.LeftShoulder_LeftUpperArm,LeftShoulder_LeftUpperArm },
            {JointRotationIndex.RightShoulder_RightUpperArm,RightShoulder_RightUpperArm },
            {JointRotationIndex.LeftUpperArm_LeftLowerArm,LeftUpperArm_LeftLowerArm },
            {JointRotationIndex.RightUpperArm_RightLowerArm,RightUpperArm_RightLowerArm },
            {JointRotationIndex.LeftLowerArm_LeftHand,LeftLowerArm_LeftHand },
            {JointRotationIndex.RightLowerArm_RightHand,RightLowerArm_RightHand },
        };
    }

    public void UpdateSkeleton(ISkeleton skeleton)
    {
        if (skeleton == null) return;
        _skeleton = skeleton;

        Hips_LeftUpperLeg = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position);
        Hips_RightUpperLeg = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position);
        LeftUpperLeg_LeftLowerLeg = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)1).position);
        RightUpperLeg_RightLowerLeg = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)4).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)2).position);
        LeftLowerLeg_LeftFoot = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)5).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)3).position);
        RightLowerLeg_RightFoot = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)6).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)4).position);
        Hips_Spine = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)7).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)0).position);
        //Specialfall
        Spine_UpperSpine = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)7).position);
        //Spine_Chest = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)8).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)7).position);
        //Chest_UpperSpine = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)8).position);
        UpperSpine_LeftShoulder = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)12).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position);
        UpperSpine_RightShoulder = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)9).position);
        LeftShoulder_LeftUpperArm = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)14).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)12).position);
        RightShoulder_RightUpperArm = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)15).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)13).position);
        LeftUpperArm_LeftLowerArm = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)16).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)14).position);
        RightUpperArm_RightLowerArm = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)17).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)15).position);
        LeftLowerArm_LeftHand = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)18).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)16).position);
        RightLowerArm_RightHand = Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)19).position) - Conversion.TsVector3ToUnityVector3(skeleton.GetBoneTransform((TsHumanBoneIndex)17).position);

        UpdateRotationDictionary();
    }
    void UpdateRotationDictionary()
    {
        _rotationDict = new Dictionary<JointRotationIndex, Vector3>
        {
            {JointRotationIndex.Hips_LeftUpperLeg,Hips_LeftUpperLeg },
            {JointRotationIndex.Hips_RightUpperLeg,Hips_RightUpperLeg },
            {JointRotationIndex.LeftUpperLeg_LeftLowerLeg,LeftUpperLeg_LeftLowerLeg },
            {JointRotationIndex.RightUpperLeg_RightLowerLeg,RightUpperLeg_RightLowerLeg },
            {JointRotationIndex.LeftLowerLeg_LeftFoot,LeftLowerLeg_LeftFoot },
            {JointRotationIndex.RightLowerLeg_RightFoot,RightLowerLeg_RightFoot },
            {JointRotationIndex.Hips_Spine,Hips_Spine },
            {JointRotationIndex.Spine_Chest,Spine_Chest },
            {JointRotationIndex.Chest_UpperSpine,Chest_UpperSpine },
            {JointRotationIndex.Spine_UpperSpine,Spine_UpperSpine },
            {JointRotationIndex.UpperSpine_LeftShoulder,UpperSpine_LeftShoulder },
            {JointRotationIndex.UpperSpine_RightShoulder,UpperSpine_RightShoulder },
            {JointRotationIndex.LeftShoulder_LeftUpperArm,LeftShoulder_LeftUpperArm },
            {JointRotationIndex.RightShoulder_RightUpperArm,RightShoulder_RightUpperArm },
            {JointRotationIndex.LeftUpperArm_LeftLowerArm,LeftUpperArm_LeftLowerArm },
            {JointRotationIndex.RightUpperArm_RightLowerArm,RightUpperArm_RightLowerArm },
            {JointRotationIndex.LeftLowerArm_LeftHand,LeftLowerArm_LeftHand },
            {JointRotationIndex.RightLowerArm_RightHand,RightLowerArm_RightHand },
        };
    }

    public float CalculateAngle(TsHumanBoneIndex bone1, TsHumanBoneIndex bone2, TsHumanBoneIndex bone3)
    {
        if (_skeleton == null) return 0;
        Vector3 first = Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone2).position) - Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone1).position);
        Vector3 second = Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone2).position) - Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone3).position);
        return Vector3.Angle(first, second);    

    }

    public float CalculateAngle(Vector3 first, TsHumanBoneIndex bone2, TsHumanBoneIndex bone3)
    {
        Vector3 second = Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone2).position) - Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(bone3).position);
        return Vector3.Angle(first, second);
    }
        

    public float CalculateAngles(JointRotationIndex j1, JointRotationIndex j2)
    {
       return Vector3.Angle(RotationDictionary[j2], RotationDictionary[j1]);       
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
