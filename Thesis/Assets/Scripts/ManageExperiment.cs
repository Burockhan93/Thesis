using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using TsAPI.Types;
using System.IO;
using System.Linq;
using System.Text;

public class ManageExperiment : MonoBehaviour
{
    [Header("Keyboard Controller")]
    public bool Controller = true;
    [Header("TSAnimator Component")]
    public TsHumanAnimator Subjekt;
    [Header("Locations Needed for Evaluation")]
    public GameObject[] locations; //Head, leftShoulder, leftElbow, leftWrist, rightShoulder, RightElbow, rightWrist
    [Header("All qurestions in UI")]
    public GameObject[] questions;
    [Header("Replay and Experiment Interfaces")]
    public GameObject Experiment;
    public GameObject Replay;
    public GameObject CollidableObject;
    [Header("Experiment")]
    public Experiment _experimente;

    //JointCalculator for Suit
    JointRotationCalculator _jointRotationCalculator = new JointRotationCalculator();

    //To visaulize the result
    private TMP_Text result;
    //To keep tabs on which question we are in
    private int _questionIndex = 1;


    private string _tenplatePath = @"D:\TU Darmstadt\WinterSemester2023\Thesis\Unity\Thesis\Thesis\Assets\Agenda\";
    private string _name = "EvaluationTemplate";
    public TMP_Text rr;
    private float _timer;
    private bool _ispressed;
    void Start()
    {
        result = FindObjectOfType<TsUI>().Results.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Controller) return;
        if (Input.GetKeyDown(KeyCode.Alpha0)) GetSuitElbowDegrees();
        if (Input.GetKeyDown(KeyCode.Alpha1)) MeasureWristDistance();
        if (Input.GetKeyDown(KeyCode.Alpha2)) { _ispressed = true;  MeasureParalelityShoulderWristElbow(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) MeasureHeadToHandDistance();
        if (Input.GetKeyDown(KeyCode.Alpha4)) MeasureElbowWristParalelity();
        if (Input.GetKeyDown(KeyCode.Alpha5)) StartCoroutine( MeasureElbowsTravel());
        if (Input.GetKeyDown(KeyCode.Alpha6)) Subjekt.SetupAvatarBones();
       // if (_ispressed) { _timer += Time.deltaTime; if (_timer > 2) { MeasureParalelityShoulderWristElbow(); _timer = 0; } }
    }
    public void SetNextQuestion(int i)
    {
        questions[i].transform.Find("Question").transform.GetComponent<TMP_Text>().text = _experimente.AllQuestions[i];
    }
    public void CreateSubject()
    {
        //foreach(char c in _age) Debug.Log(c); return;
        //Debug.Log(questions[0].transform.Find("Vorname, Name/Text Area/NameText").transform); return;
        string name = questions[0].transform.Find("Vorname, Name/Text Area/NameText").transform.GetComponent<TMP_Text>().text;
        string _age = questions[0].transform.Find("Age/Text Area/AgeText").transform.GetComponent<TMP_Text>().text; _age = _age.Remove(_age.Length - 1);
        int age = int.Parse(_age, System.Globalization.NumberStyles.Integer);
        string _weight = questions[0].transform.Find("Weight/Text Area/WeightText").transform.GetComponent<TMP_Text>().text; _weight =_weight.Remove(_weight.Length - 1);
        int weight = int.Parse(_weight, System.Globalization.NumberStyles.Integer);
        string _height = questions[0].transform.Find("Height/Text Area/HeightText").transform.GetComponent<TMP_Text>().text; _height = _height.Remove(_height.Length - 1);
        float height = float.Parse(_height, NumberStyles.Float);

        _experimente = new Experiment(name, age, weight, height);

        SetNextQuestion(_questionIndex);

        Debug.Log(_experimente.ToString());

    }
    public void FirstQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isRepresentative= value;

    }
    public void SecondQuestion (bool value)
    {
        if (_experimente == null) return;

        _experimente._isDriftFirst = value;
    }
    public void ThirdQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isDriftSecond = value;
    }
    public void FourthQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isDegreeImportant = value;
    }
    public void FifthQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isMovingShoulderBetter = value;
    }
    public void SixthQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isArmRotatingBetter = value;
    }
    public void SeventQuestion(bool value)
    {
        if (_experimente == null) return;

        _experimente._isBetter = value;
    }
    public void EightQuestion(bool value)
    {
        if (_experimente == null) return;
        _experimente._isMeshBetter= value;
    }

    public void GetNextQuestion()
    {
        _questionIndex++;
        SetNextQuestion(_questionIndex);
    }
    public void GetPreviousQuestion()
    {
        if (_questionIndex == 1) return;
        _questionIndex--;
        SetNextQuestion(_questionIndex);
    }

    public void MeasureWristDistance()
    {

        float distance = Vector3.Distance(transform.InverseTransformPoint(locations[3].transform.position), transform.InverseTransformPoint( locations[6].transform.position));
        result.text= distance.ToString();  
        
    }
    public void MeasureHeadToHandDistance()
    {
        float distance = Vector3.Distance(transform.InverseTransformPoint(locations[0].transform.position), transform.InverseTransformPoint(locations[3].transform.position));
        result.text = distance.ToString();
    }
    public void MeasureArmsParalelity()
    {
        Vector3 _leftWristShoulder = transform.InverseTransformPoint(locations[3].transform.position) - transform.InverseTransformPoint(locations[1].transform.position);
        Vector3 _leftWristOnGround = new Vector3(transform.InverseTransformPoint(locations[3].transform.position).x, 0, transform.InverseTransformPoint(locations[3].transform.position).z);
        Vector3 _leftShoulderOnGround = new Vector3(transform.InverseTransformPoint(locations[1].transform.position).x, 0, transform.InverseTransformPoint(locations[1].transform.position).z);
        Vector3 _LeftOnGround = _leftWristOnGround - _leftShoulderOnGround;
        float left = Vector3.Angle(_leftWristShoulder, _LeftOnGround);
        result.text +="\n"+ left.ToString();
        Vector3 _rightWristShoulder = transform.InverseTransformPoint(locations[6].transform.position) - transform.InverseTransformPoint(locations[4].transform.position);
        Vector3 _rightWristOnGround = new Vector3(transform.InverseTransformPoint(locations[6].transform.position).x, 0, transform.InverseTransformPoint(locations[6].transform.position).z);
        Vector3 _rightShoulderOnGround = new Vector3(transform.InverseTransformPoint(locations[4].transform.position).x, 0, transform.InverseTransformPoint(locations[4].transform.position).z);
        Vector3 _RightOnGround = _rightWristOnGround - _rightShoulderOnGround;
        float right = Vector3.Angle(_rightWristShoulder, _RightOnGround);
        result.text += " "+right.ToString();
       Debug.Log(right);
    }
    public void MeasureParalelityShoulderWristElbow()
    {

        Vector3 _leftWristShoulder = transform.InverseTransformPoint(locations[3].transform.position) - transform.InverseTransformPoint(locations[1].transform.position);
        Vector3 _leftElbowShoulder = transform.InverseTransformPoint(locations[2].transform.position) - transform.InverseTransformPoint(locations[1].transform.position);
       // Vector3 leftwrist_rightWrist = transform.InverseTransformPoint(locations[6].transform.position) - transform.InverseTransformPoint(locations[3].transform.position);
        result.text= Vector3.Angle(_leftWristShoulder, _leftElbowShoulder).ToSafeString();
        Vector3 _rightWristShoulder = transform.InverseTransformPoint(locations[6].transform.position) - transform.InverseTransformPoint(locations[4].transform.position);
        Vector3 _rightElbowShoulder = transform.InverseTransformPoint(locations[5].transform.position) - transform.InverseTransformPoint(locations[4].transform.position);
        //Vector3 right_leftwrist = transform.InverseTransformPoint(locations[3].transform.position) - transform.InverseTransformPoint(locations[6].transform.position);
        result.text += " "+Vector3.Angle(_rightWristShoulder, _rightElbowShoulder).ToSafeString();
        MeasureArmsParalelity();
    }
    public IEnumerator MeasureElbowsTravel()
    {
        //This means the first recording havent been done
        while (_experimente._leftElbowLocationsWithout.Count < 1000)
        {
            _experimente._leftElbowLocationsWithout.Add(locations[2].transform.localPosition);
            _experimente._rightElbowLocationsWithout.Add(locations[5].transform.localPosition);
            yield return null;
        }
        //correction will be turned on
        while (_experimente.leftElbowLocationsWith.Count < 1000)
        {
            _experimente.leftElbowLocationsWith.Add(locations[2].transform.localPosition);
            _experimente.rightElbowLocationsWith.Add(locations[5].transform.localPosition);
            yield return null;
        }


    }
    public void MeasureElbowWristParalelity()
    {
        //1.Method Lineparalelity- gives us the degree between elbow to wrist and its projection on the plane(y=0)
        Vector3 _leftElbowWrist = transform.InverseTransformPoint(locations[2].transform.position) - transform.InverseTransformPoint(locations[3].transform.position);
        Vector3 _leftWristOnGround = new Vector3(transform.InverseTransformPoint(locations[3].transform.position).x, 0, transform.InverseTransformPoint(locations[3].transform.position).z);
        Vector3 _leftElbowOnGround = new Vector3(transform.InverseTransformPoint(locations[2].transform.position).x, 0, transform.InverseTransformPoint(locations[2].transform.position).z);
        Vector3 _LeftOnGround = _leftElbowOnGround - _leftWristOnGround;
        float left = Vector3.Angle(_leftElbowWrist, _LeftOnGround);

        Vector3 _rightElbowWrist = transform.InverseTransformPoint(locations[5].transform.position) - transform.InverseTransformPoint(locations[6].transform.position);
        Vector3 _rightWristOnGround = new Vector3(transform.InverseTransformPoint(locations[6].transform.position).x, 0, transform.InverseTransformPoint(locations[6].transform.position).z);
        Vector3 _rightElbowOnGround = new Vector3(transform.InverseTransformPoint(locations[5].transform.position).x, 0, transform.InverseTransformPoint(locations[5].transform.position).z);
        Vector3 _rightOnGround = _rightElbowOnGround - _rightWristOnGround;
        
        float right = Vector3.Angle(_rightElbowWrist, _rightOnGround);
        result.text = left.ToString()  +" "+right.ToString();

        ////2.Method Degree - gives us the degree between shoulder elbow, elbow wrist
        //Vector3 _leftElbowToShoulder = transform.InverseTransformPoint(locations[1].transform.position) - transform.InverseTransformPoint(locations[2].transform.position);
        //Vector3 _leftElbowToWrist = transform.InverseTransformPoint(locations[3].transform.position) - transform.InverseTransformPoint(locations[2].transform.position);
        //float left = Vector3.Angle(_leftElbowToShoulder, _leftElbowToWrist);

        //Vector3 _rightElbowToShoulder = transform.InverseTransformPoint(locations[4].transform.position) - transform.InverseTransformPoint(locations[5].transform.position);
        //Vector3 _rightElbwToWrist = transform.InverseTransformPoint(locations[6].transform.position) - transform.InverseTransformPoint(locations[5].transform.position);
        //float right = Vector3.Angle(_rightElbowToShoulder, _rightElbwToWrist);

        //result.text = left.ToString() + " " + right.ToString();

    }
    public void GetSuitElbowDegrees()
    {
        float left = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.LeftUpperArm, TsHumanBoneIndex.LeftLowerArm, TsHumanBoneIndex.LeftHand);
        float right = _jointRotationCalculator.CalculateAngle(TsHumanBoneIndex.RightUpperArm, TsHumanBoneIndex.RightLowerArm, TsHumanBoneIndex.RightHand);

        result.text = left +" "+right;
    }

    public void PlayScene()
    {
        Experiment.SetActive(false);
        Replay.SetActive(true);
    }

    public void InstantiateCollidableObject()
    {
        Instantiate(CollidableObject); CollidableObject.transform.position = new Vector3(UnityEngine.Random.Range(1,4), UnityEngine.Random.Range(1, 4), UnityEngine.Random.Range(1, 4));
    }

    public void FinishExperiment()
    {
        string template = File.ReadAllText(string.Concat(_tenplatePath, $"{_name}", ".txt"));
        StringBuilder sb = new StringBuilder(template);
        rr.text= template;
        List<int> tt= template.AllIndexesOf(":").ToList();
        Debug.Log(tt.Count);
        tt.Reverse(); for (int i= 0; i < tt.Count; i++)  { tt[i] += 1; }

        sb.Insert(tt[0], " " + _experimente._isMeshBetter);
        sb.Insert(tt[1], " " + _experimente._improvementScore);
        sb.Insert(tt[2], " " + _experimente._isBetter.YesNo());
        sb.Insert(tt[3], " " + _experimente._applausWith);
        sb.Insert(tt[4], " " + _experimente.rightElbowDegreeWith);
        sb.Insert(tt[5], " " + _experimente.leftelbowDegreeWith);
        sb.Insert(tt[6], " " + _experimente._applaudWithout);
        sb.Insert(tt[7], " " + _experimente._rightElbowDegreeWithout);
        sb.Insert(tt[8], " " + _experimente._leftElbowDegreeWithout);
        sb.Insert(tt[9], " " + _experimente._isArmRotatingBetter.YesNo());
        sb.Insert(tt[10], " " + _experimente._distanceWith);
        sb.Insert(tt[11], " " + _experimente._distanceWithout);
        sb.Insert(tt[12], " " + _experimente._isMovingShoulderBetter);
        sb.Insert(tt[13], " " + _experimente._wristDistance8x);
        sb.Insert(tt[14], " " + _experimente._rightArmAngle8x);
        sb.Insert(tt[15], " " + _experimente._leftArmAngle8x);
        sb.Insert(tt[16], " " + _experimente._rightShoulderElbowAngle8x);
        sb.Insert(tt[17], " " + _experimente._leftShoulderElbowAngle8x);
        sb.Insert(tt[18], " " + _experimente._bestDegree);
        sb.Insert(tt[19], " " + _experimente._wristDistance83);
        sb.Insert(tt[20], " " + _experimente._rightArmAngle83);
        sb.Insert(tt[21], " " + _experimente._leftArmAngle83);
        sb.Insert(tt[22], " " + _experimente._rightShoulderElbowAngle83);
        sb.Insert(tt[23], " " + _experimente._leftShoulderElbowAngle83);
        sb.Insert(tt[24], " " + _experimente._isDegreeImportant);
        sb.Insert(tt[25], " " + _experimente.WristDistanceBigDrift);
        sb.Insert(tt[26], " " + _experimente.RightElbowDegreeBigDrift);
        sb.Insert(tt[27], " " + _experimente.LeftElbowDegreeBigDrift);
        sb.Insert(tt[28], " " + _experimente.WristDistanceNoBigDrift);
        sb.Insert(tt[29], " " + _experimente.RightElbowDegreeNoBigDrift);
        sb.Insert(tt[30], " " + _experimente.LeftElbowDegreeNoBigDrift);
        sb.Insert(tt[31], " " + _experimente._isDriftSecond.YesNo());
        sb.Insert(tt[32], " " + _experimente.WristDistanceDrift);
        sb.Insert(tt[33], " " + _experimente.RightElbowDegreeDrift);
        sb.Insert(tt[34], " " + _experimente.LeftElbowDegreeDrift);
        sb.Insert(tt[35], " " + _experimente.WristDistanceNoDrift);
        sb.Insert(tt[36], " " + _experimente.RightElbowDegreeNoDrift);
        sb.Insert(tt[37], " " + _experimente.LeftElbowDegreeNoDrift);
        sb.Insert(tt[38], " " + _experimente._isDriftFirst.YesNo());
        sb.Insert(tt[39], " " + _experimente._isRepresentative.YesNo());
        sb.Insert(tt[40], " " + _experimente.Height);
        sb.Insert(tt[41], " " + _experimente.Weight);
        sb.Insert(tt[42], " " + _experimente.Age);
        sb.Insert(tt[43], " " + _experimente.Name);

        Extractor.WriteToFile(_experimente.Name, sb);


    }
   
}
