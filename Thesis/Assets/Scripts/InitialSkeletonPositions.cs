using System.Collections;
using System.Collections.Generic;
using TsAPI.Types;
using TsSDK;
using UnityEngine;

public class InitialSkeletonPositions
{
    public bool initialized { get; private set; }
    private Dictionary<TsHumanBoneIndex, Vector3> _initialPoseDict = new Dictionary<TsHumanBoneIndex, Vector3>();
    public Dictionary<TsHumanBoneIndex, Vector3> InitialPoseDict { get => _initialPoseDict; }
    // Start is called before the first frame update
    public InitialSkeletonPositions() { }
    public InitialSkeletonPositions(ISkeleton _skeleton, Transform parent)
    {
        initialized= true;
        foreach (var boneIndex in TsHumanBones.SuitBones)
        {
            if ((int)boneIndex == 8 || (int)boneIndex == 10 || (int)boneIndex == 11 || (int)boneIndex > 19) continue;
           // Debug.Log(boneIndex+" "+Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(boneIndex).position));
            //GetBoneTransform returns in local space
            Vector3 pos = Conversion.TsVector3ToUnityVector3(_skeleton.GetBoneTransform(boneIndex).position);
            _initialPoseDict.Add(boneIndex,pos);

        }
    }
}
