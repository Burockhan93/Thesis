using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Experiment 
{
    
    //personal info
    string _name; public string Name { get { return _name; } }
    int _age,_weight; public int Age { get { return _age; } } public int Weight { get { return _weight; } }
    float _height; public float Height { get { return _height; } }

    private List<ExperimentQuestion> _experimentQuestions;

    readonly string _firstQuestion = "Do you think that the suit has represented all of your movements correctly?";
    readonly string _secondQuestion = "Was there a visual difference netween two applause positions?";
    readonly string _thirdQuestion = "Again, was there a visual difference between two applause positions?";
    readonly string _fourthQuestion = "We have tried different initalisation degrees. Have you noticed any differences between IPoses,TPoses and applause positions? Which one was better in your opinion?";
    readonly string _fifthQuestion = "We have tried the same poses with some adjustments. Have you noticed any visual difference between two body pose? Which one was better?";
    readonly string _sixthQuestion = "We have made an additional adjustment while rotating the arms. Which was visually more appealing in your opinion? First or second? Was there any visual difference in the applaus position this time ?";
    readonly string _seventhQuestion = "We have made some certain body poses twice in a row with and without adjustments. Which would you consider to be more representative of your movements?";
    readonly string _eightQuestion = "In this scene the both avatars interact with the environment. Which one of them looks like it actually interacts with the objects? What can you say about the visual  response of the bodies of both avatars?";

    [Header("1st Question")]
    public bool _isRepresentative;
    //1st Question - General Representative
    [Header("2nd Question - Small Drift")]
    //2nd question - Drift Over Time
    public bool _isDriftFirst ;
    public float LeftElbowDegreeNoDrift ;
    public float RightElbowDegreeNoDrift ;
    public float WristDistanceNoDrift;
    public float LeftElbowDegreeDrift;
    public float RightElbowDegreeDrift;
    public float WristDistanceDrift;
    //3rd Question - Big Drift
    [Header("3rd Question - Big Drift")]
    public bool _isDriftSecond ;
    public float LeftElbowDegreeNoBigDrift;
    public float RightElbowDegreeNoBigDrift;
    public float WristDistanceNoBigDrift;
    public float LeftElbowDegreeBigDrift;
    public float RightElbowDegreeBigDrift;
    public float WristDistanceBigDrift;

    [Header("4th Question - Degree Change")]
    //4th Question- 83 degree
    public bool _isDegreeImportant ;
    public float _leftShoulderElbowAngle83;
    public float _rightShoulderElbowAngle83;
    public float _leftArmAngle83 ;
    public float _rightArmAngle83;
    public float _wristDistance83;
    public int _bestDegree ;
    public float _leftShoulderElbowAngle8x;
    public float _rightShoulderElbowAngle8x;
    public float _leftArmAngle8x;
    public float _rightArmAngle8x;
    public float _wristDistance8x;
    [Header("5th Question - Moving Shoulder Joints")]
    //5th Question - Moving Shoulders
    public bool _isMovingShoulderBetter ;
    public float _distanceWithout ;
    public float _distanceWith ;
    [Header("6th Question - Arm Rotation Correction")]
    //6th Question - Arms Rotating
    public bool _isArmRotatingBetter ;
    public List<Vector3> _leftElbowLocationsWithout = new List<Vector3>();
    public List<Vector3> _rightElbowLocationsWithout = new List<Vector3>();
    public float _leftElbowDegreeWithout ;
    public float _rightElbowDegreeWithout ;
    public float _applaudWithout ;

    public List<Vector3> leftElbowLocationsWith = new List<Vector3>();
    public List<Vector3> rightElbowLocationsWith = new List<Vector3>();
    public float leftelbowDegreeWith;
    public float rightElbowDegreeWith;
    public float _applausWith ;
    //7th Question - General
    [Header("7th Question - General Questions")]
    public bool _isBetter;
    public int _improvementScore ;
    [Header("8th Question - Mesh Deformation")]
    public bool _isMeshBetter;







    private Dictionary<int, string> _allQuestions; 
    public Dictionary<int, string> AllQuestions { get { return _allQuestions; } }
    public Experiment(string name, int age, int weight, float height)
    {
        _allQuestions = new Dictionary<int, string>
        {
            {1, _firstQuestion},
            {2, _secondQuestion},
            {3, _thirdQuestion},
            {4, _fourthQuestion},
            {5, _fifthQuestion},
            {6, _sixthQuestion},
            {7, _seventhQuestion},
            {8, _eightQuestion},
                
        };
        _name = name;
        _age = age;
        _height= height;
        _weight = weight;
        _experimentQuestions = new List<ExperimentQuestion>();

    }

    public ExperimentQuestion getNextQuestion(int index)
    {
        return new ExperimentQuestion(_allQuestions[index]);
    }
   
    public void AddQuestion(ExperimentQuestion question)
    {
        _experimentQuestions.Add(question);
    }

    public override string ToString()
    {
        return _name +" "+ _age +" "+ _weight +" "+ _height;
        return null;
    }
    // Iki kisimdan uulusun her experiment icin bu class olstrulacak
    //Ilk kisim umfrgae yanitlari
    // ikinci kisim bizim ölcümler.




    //applaud distance ölcülecek
    //line paralleligi
    //El kafa arasi mesafe
    //Elbow travel distance ölcülecek kol döndüdrme hareket boyunca ve yine kollarin paralleigi ölcülecek, applaud ölcülecek mesafe yine
   

    // Start is called before the first frame update
   
}
