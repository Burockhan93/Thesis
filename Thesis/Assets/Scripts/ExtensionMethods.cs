using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static string YesNo(this bool value)
    {
        if (value) return "Yes";
        else return "No";
    }
    public static Vector3 ToEuler(this Quaternion quaternion)
    {
        float x, y, z;
        

        float sinr_cosp = +2.0f * (quaternion.w * quaternion.x + quaternion.y * quaternion.z);
        float cosr_cosp = +1.0f - 2.0f * (quaternion.x * quaternion.x + quaternion.y * quaternion.y);
        x = Mathf.Atan2(sinr_cosp, cosr_cosp);

        float sinp = +2.0f * (quaternion.w * quaternion.y - quaternion.z * quaternion.x);
        if (Mathf.Abs(sinp) >= 1)
        {
            y = Mathf.PI / 2f * Mathf.Sign(sinp);
        }
        else
        {
            y = Mathf.Asin(sinp);
        }

        float siny_cosp = +2.0f * (quaternion.w * quaternion.z + quaternion.x * quaternion.y);
        float cosy_cosp = +1.0f - 2.0f * (quaternion.y * quaternion.y + quaternion.z * quaternion.z);
        z = Mathf.Atan2(siny_cosp, cosy_cosp);

        return new Vector3(x,y,z);
    }
}
