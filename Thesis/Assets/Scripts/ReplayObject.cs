using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayObject
{
    public string animationName;
    public List<ReplayInfo> replayInfo = new List<ReplayInfo>();
   

    public ReplayObject() { }
    public ReplayObject(string name, List<ReplayInfo> info)
    {
        animationName = name;
        replayInfo = info;
    }
    // public List<float> timeStamp;   
}