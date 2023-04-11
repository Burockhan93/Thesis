using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TsAPI.Types;

public class TsUI : MonoBehaviour
{
    public TMP_Text rightcollar;
    public TMP_Text leftcollar;
    public TMP_Text CalibrationCountDown;
    public TMP_Text Results;

    JointRotationCalculator _jointRotationCalculator = new JointRotationCalculator();
    // Start is called before the first frame update
    void Start()
    {
        rightcollar.text = string.Empty;
        leftcollar.text = string.Empty;
        CalibrationCountDown.text = string.Empty;
        Results.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        //leftcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.UpperSpine, TsHumanBoneIndex.LeftShoulder, TsHumanBoneIndex.LeftUpperArm).ToString();
        //rightcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.UpperSpine, TsHumanBoneIndex.RightShoulder, TsHumanBoneIndex.RightUpperArm).ToString();

        //leftcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.LeftShoulder, TsHumanBoneIndex.LeftUpperArm, TsHumanBoneIndex.LeftLowerArm).ToString();
        //rightcollar.text = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.RightShoulder, TsHumanBoneIndex.RightUpperArm, TsHumanBoneIndex.RightLowerArm).ToString();



    }
}
