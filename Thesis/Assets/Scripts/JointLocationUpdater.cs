using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using UnityEngine;

[System.Serializable]
public class JointLocationUpdater 
{
    private const string _hips = "pelvis";
    private const string _leftupperLeg = "left_hip";
    private const string _rightUpperLeg = "right_hip";
    private const string _leftLowerLeg = "left_knee";
    private const string _rightLowerleg = "right_knee";
    private const string _leftFoot = "left_ankle";
    private const string _rightFoot = "right_ankle";
    private const string _spine = "spine1";
    private const string _chest = "spine2";
    private const string _upperSpine = "spine3";
    private const string _leftShoulder = "left_collar";
    private const string _rightShouldher = "right_collar";
    private const string _leftUpperArm = "left_shoulder";
    private const string _rightUpperArm = "right_shoulder";
    private const string _leftLowerArm = "left_elbow";
    private const string _rightLowerArm = "right_elbow";
    private const string _leftHand = "left_wrist";
    private const string _righthand = "right_wrist";

    float inity = 0.06041686f;

    private const float minSpineShoulderUpperarmDegree = 100f;
    private const float maxSpineShoulderUpperArmDegree = 130f;

    private const float minShoulderArmElbowDegree = 65f;
    private const float maxShoulderArmElbowDegree = 120f;

    private readonly Dictionary<TsHumanBoneIndex, Vector3> _initialBodyJointLocations= new Dictionary<TsHumanBoneIndex,Vector3>();
    public Dictionary<TsHumanBoneIndex, Vector3> InitialBodyJointLocations {get { return _initialBodyJointLocations;}}

    public JointLocationUpdater(Transform root)
    {
        

        Debug.Log("called");
        _initialBodyJointLocations.Add(TsHumanBoneIndex.Hips, TransformUtils.FindChildRecursive( root,_hips).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftUpperLeg, TransformUtils.FindChildRecursive(root,_leftupperLeg).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightUpperLeg, TransformUtils.FindChildRecursive(root,_rightUpperLeg).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftLowerLeg, TransformUtils.FindChildRecursive(root,_leftLowerLeg).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightLowerLeg, TransformUtils.FindChildRecursive(root,_rightLowerleg).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftFoot, TransformUtils.FindChildRecursive(root,_leftFoot).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightFoot, TransformUtils.FindChildRecursive(root,_rightFoot).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.Spine, TransformUtils.FindChildRecursive(root,_spine).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.Chest, TransformUtils.FindChildRecursive(root,_chest).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.UpperSpine, TransformUtils.FindChildRecursive(root,_upperSpine).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftShoulder, TransformUtils.FindChildRecursive(root,_leftShoulder).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightShoulder, TransformUtils.FindChildRecursive(root,_rightShouldher).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftUpperArm, TransformUtils.FindChildRecursive(root,_leftUpperArm).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightUpperArm, TransformUtils.FindChildRecursive(root,_rightUpperArm).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftLowerArm, TransformUtils.FindChildRecursive(root,_leftLowerArm).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightLowerArm, TransformUtils.FindChildRecursive(root,_rightLowerArm).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftHand, TransformUtils.FindChildRecursive(root,_leftHand).localPosition);
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightHand, TransformUtils.FindChildRecursive(root,_righthand).localPosition);
    }

    // Start is called before the first frame update
   
    public Vector3 UpdateShoulderLocations(TsHumanBoneIndex bone, float verticalDegree, float horizontalDegree)
    {
        //Debug.Log(_initialBodyJointLocations[TsHumanBoneIndex.LeftUpperArm].localPosition); return Vector3.zero;
       
        verticalDegree = Mathf.Clamp(verticalDegree, minSpineShoulderUpperarmDegree, maxSpineShoulderUpperArmDegree);
        verticalDegree -= minSpineShoulderUpperarmDegree; verticalDegree /= 90;
        float pos_Y = Mathf.Pow(verticalDegree, 3);
        float initial_pos_Y = _initialBodyJointLocations[bone].y;

        horizontalDegree = Mathf.Clamp(horizontalDegree, minShoulderArmElbowDegree, maxShoulderArmElbowDegree);
        horizontalDegree-= minShoulderArmElbowDegree; 

        //Debug.Log(initial_pos_Y + pos_Y);

        return new Vector3(_initialBodyJointLocations[bone].x, initial_pos_Y + pos_Y, _initialBodyJointLocations[bone].z);

    } 

    public Vector3 ResetLocations(int index)
    {
      return _initialBodyJointLocations[(TsHumanBoneIndex)index];
    } 

}
