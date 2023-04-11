using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CollidableObject : MonoBehaviour
{
   
    public CollidableObjectSettings _settings;

    private string _ignoreTag = "Ignore";
    private string _hardTag = "HardSkin";
    private string _softTag = "SoftSkin";
    private string _defaultTag = "DefaultSkin";

    private float _skinCoefficient = 1;

    Vector3? colP;
    Collider col;
    MeshFilter mesh;
    List<Vector3> points;
    Rigidbody rb;
    bool _collide;
    public GameObject Player;
    public CollidableObjectSettings[] settings;
    private void Awake()
    {
        _settings = settings[Random.Range(0, settings.Length)];

        transform.localScale = _settings.Scale;

        Player = GameObject.Find(@"smplx-male/SMPLX-male/root/pelvis");

        mesh = GetComponent<MeshFilter>();
        col = GetComponent<Collider>();
        points = new List<Vector3>();
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, 3f);
    }
    private void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
       // rb.AddForce(Vector3.back/10);
        rb.AddForce( (Player.transform.position -transform.position).normalized); 
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag(_ignoreTag)) return;
        else if (collision.gameObject.CompareTag(_hardTag)) _skinCoefficient = 0.5f;
        else if (collision.gameObject.CompareTag(_softTag)) _skinCoefficient = 2f; 
        else if (collision.gameObject.CompareTag(_defaultTag)) { }

        Debug.Log(collision.gameObject.name);

        //if (!_collide) return;
        ContactPoint[] points = collision.contacts;
        MeshDeformer md = collision.transform.root.GetComponentInChildren<MeshDeformer>();

        if (md)
        {
            md.AddDeformingForce(points[0].point, _settings.ImpactForce,_settings.DepthOffset * _skinCoefficient, points[0].normal);
        }
        foreach (ContactPoint p in points)
        {
            //Debug.Log(p.point);//world
            colP = transform.InverseTransformPoint(p.point);//local
           // Debug.Log(colP.Value.ToString("F4"));

                DetectVertice(p.point);

         }
        }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(_ignoreTag)) return;
        else if (collision.gameObject.CompareTag(_hardTag)) _skinCoefficient = 0.5f;
        else if (collision.gameObject.CompareTag(_softTag)) _skinCoefficient = 2f;
        else if (collision.gameObject.CompareTag(_defaultTag)) { }
        //if (!_collide) return;
        ContactPoint[] points = collision.contacts;
        MeshDeformer md = collision.transform.root.GetComponentInChildren<MeshDeformer>();

        if (md)
        {
            md.AddDeformingForce(points[0].point, _settings.ImpactForce,_settings.DepthOffset * _skinCoefficient, points[0].normal);
      
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Collider")) _collide= false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collider")) _collide = true;
    }
    private Vector3 DetectVertice(Vector3 point)
    {
        foreach (Vector3 vertice in mesh.mesh.vertices)
        {
            Vector3 globalVertice = transform.TransformPoint(vertice);
            float distance = Vector3.Distance(globalVertice, point);
            if (distance < 0.1f)// change this to _settings.raidus
                points.Add(globalVertice);
            //Debug.Log(Vector3.Distance(vertice, point));
        }
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;


        //if (colP.HasValue) Gizmos.DrawSphere(colP.Value, 0.05f);
        //if (points != null && points.Count > 0)
        //{
        //    foreach (Vector3 p in points)
        //    {
        //        Gizmos.DrawSphere(p, 0.005f);
        //    }
        //}
    }

}
