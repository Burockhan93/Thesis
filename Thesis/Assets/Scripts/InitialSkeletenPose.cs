using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

/// <summary>
/// Saves the initial local joint positions
/// </summary>
public class InitialSkeletenPose
{
    private Dictionary<TsHumanBoneIndex, Vector3> _initialPoseDict;
    public Dictionary<TsHumanBoneIndex, Vector3> InitialPoseDict { get => _initialPoseDict; }
    // Start is called before the first frame update
    public InitialSkeletenPose() { }
    public InitialSkeletenPose(ISkeleton _skeleton, Transform parent)
    {
        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            if ((int)boneIndex == 8 || (int)boneIndex == 10 || (int)boneIndex == 11 || (int)boneIndex > 19) continue;

            _initialPoseDict.Add(boneIndex, parent.InverseTransformPoint(Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(boneIndex).position)));

        }
    }

   
}
