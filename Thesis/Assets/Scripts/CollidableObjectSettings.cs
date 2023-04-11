using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(MeshRenderer))]
[CreateAssetMenu(menuName = "Thesis/CollisionObject")]
public class CollidableObjectSettings : ScriptableObject
{
    [SerializeField]
    private float _impactForce;
    [SerializeField]
    private float _impactRadius;
    [SerializeField]
    private float _depthOffset;
    [SerializeField]
    private Vector3 _scale;
    
    public float ImpactRadius { get => _impactRadius; }
    public float DepthOffset { get => _depthOffset; }
    public Vector3 Scale { get => _scale; }
    public float ImpactForce => _impactForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setDefault()
    {       
        _impactRadius = 0.05f;
        _depthOffset= 0.005f;
        _scale = new Vector3(.1f,.1f,.1f);
        _impactForce= 10.0f;

    }
    public void setBig()
    {
        _impactRadius = 0.2f;
        _depthOffset = 0.0025f;
        _scale = new Vector3(.25f, .25f, .25f);
        _impactForce = 20.0f;
    }
    public void setSmall()
    {
        _impactRadius = 0.015f;
        _depthOffset = 0.01f;
        _scale = new Vector3(.05f, .05f, .05f);
        _impactForce = 5.0f;
    }
    public void setVeryBig()
    {
        _impactRadius = 0.5f;
        _depthOffset = 0.01f;
        _scale = new Vector3(.5f, .5f, .5f);
        _impactForce = 30.0f;
    }

}



[CustomEditor(typeof(CollidableObjectSettings))]
public class CollidableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CollidableObjectSettings collidableObject = target as CollidableObjectSettings;

        if (GUILayout.Button("Default"))
        {
            collidableObject.setDefault();
            EditorUtility.SetDirty(collidableObject);
        }
        if (GUILayout.Button("Big"))
        {
            collidableObject.setBig();
            EditorUtility.SetDirty(collidableObject);
        }
        if (GUILayout.Button("Small"))
        {
            collidableObject.setSmall();
            EditorUtility.SetDirty(collidableObject);
        }
        if (GUILayout.Button("Very Big"))
        {
            collidableObject.setVeryBig();
            EditorUtility.SetDirty(collidableObject);
        }

    }
}
