using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using UnityEngine;

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



    private const float minSpineShoulderUpperarmDegree = 100f;
    private const float maxSpineShoulderUpperArmDegree = 130f;

    private const float minShoulderArmEbowDegree = 65f;
    private const float maxShoulderArmEbowDegree = 120f;

    private Dictionary<TsHumanBoneIndex,Transform> _initialBodyJointLocations= new Dictionary<TsHumanBoneIndex,Transform>();
    public Dictionary<TsHumanBoneIndex,Transform> InitialBodyJointLocations {get { return _initialBodyJointLocations;}}

    public JointLocationUpdater(Transform root)
    {
        _initialBodyJointLocations.Clear();


        _initialBodyJointLocations.Add(TsHumanBoneIndex.Hips, root.Find(_hips));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftUpperLeg, root.Find(_leftupperLeg));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightUpperLeg, root.Find(_rightUpperLeg));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftLowerLeg, root.Find(_leftLowerLeg));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightLowerLeg, root.Find(_rightLowerleg));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftFoot, root.Find(_leftFoot));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightFoot, root.Find(_rightFoot));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.Spine, root.Find(_spine));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.Chest, root.Find(_chest));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.UpperSpine, root.Find(_upperSpine));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftShoulder, root.Find(_leftShoulder));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightShoulder, root.Find(_rightShouldher));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftUpperArm, root.Find(_leftUpperArm));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightUpperArm, root.Find(_rightUpperArm));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftLowerArm, root.Find(_leftLowerArm));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightLowerArm, root.Find(_rightLowerArm));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.LeftHand, root.Find(_leftHand));
        _initialBodyJointLocations.Add(TsHumanBoneIndex.RightHand, root.Find(_righthand));
    }

    // Start is called before the first frame update
   
    public Vector3 UpdateShoulderLocations(float verticalDegree, float horizontalDegree)
    {
        verticalDegree = Mathf.Clamp(verticalDegree, minSpineShoulderUpperarmDegree, maxSpineShoulderUpperArmDegree);
        verticalDegree -= minSpineShoulderUpperarmDegree; verticalDegree /= 100;
        float pos_Y = Mathf.Pow(verticalDegree, 3);

        float initial_pos_Y = _initialBodyJointLocations[TsHumanBoneIndex.LeftUpperArm].localPosition.y;

        return new Vector3(_initialBodyJointLocations[TsHumanBoneIndex.LeftUpperArm].localPosition.x, pos_Y + initial_pos_Y, _initialBodyJointLocations[TsHumanBoneIndex.LeftUpperArm].localPosition.z);

    } 

}
