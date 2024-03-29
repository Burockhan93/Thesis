using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TsAPI.Types;
using TsSDK;
using System.IO;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System;

//[System.Serializable]
//public struct MyQuaternion 
//{
//    public float x;
//    public float y;
//    public float z;
//    public float w;
//    public MyQuaternion(float X, float Y, float Z, float W)
//    {
//        x = X; y = Y; z = Z; w = W;
//    }

//    public static MyQuaternion ConverToMyQuat(Quaternion quat)
//    {
//        return new MyQuaternion(quat.x,quat.y,quat.z,quat.w);
//    }
//    public static Quaternion ConvertToQuat(MyQuaternion myQuat)
//    {
//        return new Quaternion(myQuat.x, myQuat.y, myQuat.z, myQuat.w);
//    }
//    public override string ToString()
//    {
//        return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
//    }

//}
//public class ReplayObject
//{
//    public string subjectName;
//    public List<ReplayInfo> replayInfo = new List<ReplayInfo>();

//    public ReplayObject() { }
//    public ReplayObject(string name, List<ReplayInfo> info)
//    {
//        subjectName = name;
//        replayInfo = info;
//    }
//   // public List<float> timeStamp;   
//}

//public class ReplayInfo : ISkeleton
//{
//    public Dictionary<TsHumanBoneIndex,Vector3> replayPosition = new Dictionary<TsHumanBoneIndex, Vector3>();
//    public Dictionary<TsHumanBoneIndex,Vector3> replayRotation = new Dictionary<TsHumanBoneIndex, Vector3>();
//    //Test
//    public Dictionary<TsHumanBoneIndex,MyQuaternion> replayRotationQuaternion = new Dictionary<TsHumanBoneIndex, MyQuaternion>();
//    public TsTransform GetBoneTransform(TsHumanBoneIndex index)
//    {
//        //will be worked further
//        TsTransform t = new TsTransform(new TsVec3f(), new TsQuat());
//        TsVec3f tt = new TsVec3f(); tt.x = replayPosition[index].x;
//        return t;
//    }
//    /// <summary>
//    /// Needs a better interpolation technique
//    /// </summary>
//    /// <param name="r1"></param>
//    /// <param name="r2"></param>
//    /// <returns></returns>
//    [System.Obsolete]
//    public static ReplayInfo Interpolate(ReplayInfo r1, ReplayInfo r2)
//    {
//        if (r1 == null || r2 == null) return r1;

//        ReplayInfo rReturn = new ReplayInfo();
//        for(int i = 0; i < r1.replayPosition.Count;i++)
//        {
//            if (r1.replayPosition.ContainsKey((TsHumanBoneIndex)i))
//            {
//                rReturn.replayPosition.Add( (TsHumanBoneIndex)i,(r1.replayPosition[(TsHumanBoneIndex)i]+ r2.replayPosition[(TsHumanBoneIndex)i])/2);
//                rReturn.replayRotation.Add( (TsHumanBoneIndex)i,(r1.replayRotation[(TsHumanBoneIndex)i]+ r2.replayRotation[(TsHumanBoneIndex)i])/2);
//            }
//        }
//        return rReturn;

//    }
//    public bool GetBoneTransform(TsHumanBoneIndex boneIndex, out TsTransform boneTransform) { boneTransform = new TsTransform(); return false; }

//    public override string ToString()
//    {
//        string row = "\n";
//        string space = " : ";
//        string s = string.Empty;

//        //Just positions
//        foreach(KeyValuePair<TsHumanBoneIndex,Vector3> kvp in replayPosition)
//        {
//            s = string.Concat(s, kvp.Key,space, kvp.Value,row);
//        }

//        return s;
//    }


//}

public class TsReplaySaver : MonoBehaviour
{
    string path = @"D:\TU Darmstadt\WinterSemester2023\Thesis\Unity\Thesis\Thesis\Assets\TS_Animations\";
    string underScore = "_";
    public TMP_InputField movementName;
    public TsHumanAnimator avatarBoneInfo;

    private bool shouldSave;
    private bool isReplayPlaying;

    private ReplayObject infoToJSon;

    //Coroutine vars
    private IEnumerator coroutine;
    private bool coroutineRunning;
    private int currentIndex = 0;

    //UI Stuff
    [SerializeField]
    private Dropdown replayDropDown;
    [SerializeField] 
    private Button replayButton;
    [SerializeField]
    private GameObject createSubjectField;
    [SerializeField]
    private GameObject startRecordingField;
    public Sprite Play;
    public Sprite Pause;


    float timer;

   
  
    void Start()
    {
        //CsvEditor.DetectCSV();
        FillReplayDropDown();
    }
   

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (shouldSave )
        {
            //infoToJSon.replayInfo.Add(new Dictionary<TsHumanBoneIndex, Quaternion> { [TsHumanBoneIndex.Hips] = avatarBoneInfo.BoneTransforms[0].rotation });
            ReplayInfo rp = new ReplayInfo();
            foreach(KeyValuePair<TsHumanBoneIndex, Transform> kvp in avatarBoneInfo.BoneTransforms)
            {
                rp.replayPosition.Add(kvp.Key, kvp.Value.position);
                rp.replayRotation.Add(kvp.Key, kvp.Value.rotation.eulerAngles);
                rp.replayRotationQuaternion.Add(kvp.Key, MyQuaternion.ConverToMyQuat(kvp.Value.rotation));
            }
            
            infoToJSon.replayInfo.Add(rp);
           
            
            return;
        }
        timer = 0;



        //foreach (KeyValuePair<TsHumanBoneIndex, Transform> kvp in avatarBoneInfo.BoneTransforms)
        //{
        //    Debug.Log("index: " + kvp.Key + " pos: " + kvp.Value.position + " pos: " + kvp.Value.rotation);
        //}
    }

    void FillReplayDropDown()
    {
        //Get all Jsons
        DirectoryInfo dir = new System.IO.DirectoryInfo(path);
        List<string> filenames = new List<string>();
       
        foreach(FileInfo f in dir.GetFiles())
        {
            string St = f.Name;
            //dont consider meta data they are for git
            if (St.Contains("meta")) continue;

            int pTo = St.IndexOf(".json");
           filenames.Add( St.Substring(0, pTo));
    
        }

        replayDropDown.ClearOptions();
        if (filenames.Count == 0) return;
        replayDropDown.AddOptions(filenames);  

    }

    public void CreatenewAnimation()
    {
        if (movementName.text == string.Empty) { Debug.LogError("Please enter a name for this animation"); return; }
        infoToJSon = new ReplayObject() { animationName = movementName.text };
       
        Debug.Log("object created");
    }
    public void StartSaving()
    {
        CreatenewAnimation();
        if (infoToJSon == null) return;
        shouldSave = true;
    }
    public void StopSaving()
    {
        shouldSave = false;
        CreateJSon();
    }
    public void CreateJSon()
    {
        if (!shouldSave)
        {
            Debug.Log("json");
            //string output = JsonUtility.ToJson(infoToJSon);
            //File.WriteAllText(Application.dataPath + $"/{infoToJSon.subjectName}_replayInfos.txt", output);
            string json = JsonConvert.SerializeObject(infoToJSon.replayInfo.ToArray(),Formatting.Indented);

            //write string to file
            System.IO.File.WriteAllText(string.Concat(path, $"{infoToJSon.animationName}",underScore, ".json"), json);
            infoToJSon = null;
        }
        FillReplayDropDown();
    }
    //referenced on scene object
    public void OptionChosenFromDropDown()
    {
        if(coroutineRunning)StopCoroutine(coroutine);
        coroutineRunning = false;
        replayButton.image.sprite = Play;
        currentIndex = 0;
        isReplayPlaying = false;
        avatarBoneInfo.Replay = false;
    }

    public void FreeLook()
    {
        if (coroutineRunning) StopCoroutine(coroutine);
        coroutineRunning = false;
        replayButton.image.sprite = Play;
        currentIndex = 0;
        isReplayPlaying = false;
        avatarBoneInfo.Replay = false;
    }
    public void Replay()
    {
        shouldSave = false;
        if (coroutineRunning) return;
        coroutine = Replayy();
        avatarBoneInfo.Replay = true;
        StartCoroutine(coroutine);

    }

    public IEnumerator Replayy()
    {
        coroutineRunning = true;


        string name = replayDropDown.options[replayDropDown.value].text;
        string jsonArr = File.ReadAllText(string.Concat(path, $"{name}", ".json"));
        var player = JsonConvert.DeserializeObject<List<ReplayInfo>>(jsonArr);

        #region Interpolation attempt - Ignore
        ////Interpolation Attempt- Ignore this part
        //List<ReplayInfo> slowplayer = new List<ReplayInfo>();
        //for (int i =0; i < player.Count; i++)
        //{
        //    if (i % 2 == 0) slowplayer.Add(player[i]);
        //    else
        //    {
        //        if (player[i + 1] != null) slowplayer.Add(ReplayInfo.Interpolate(player[i - 1], player[i + 1]));
        //        else slowplayer.Add(ReplayInfo.Interpolate(player[i - 1], player[i ]));
        //    }
        //}
        #endregion
        // int frameCount = infoToJSon.replayInfo.Count;
        int frameCount = player.Count;
        //int frameCount = slowplayer.Count;
      

        //normal play
        while (currentIndex < frameCount)
        {
            avatarBoneInfo.ReplayUpdate(player[currentIndex]);
            yield return new WaitForEndOfFrame();
            currentIndex += 1;
        }
        yield return null;

        coroutineRunning = false;
        currentIndex = 0;
        avatarBoneInfo.Replay = false;

    }
    //used in the editor

    public void PlayPauseReplay()
    {
        isReplayPlaying = !isReplayPlaying;

        if (isReplayPlaying) replayButton.image.sprite = Pause;
        else replayButton.image.sprite = Play;
    }
   
}
