using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshReader : MonoBehaviour
{
    private Mesh mesh;
    private const string _skinnedMesh = "SkinnedMesh";
    private const string _meshRenderer = "MeshRenderer";
    void Start()
    {
       
    }


    void Update()
    {

        if (gameObject.tag == _skinnedMesh)
            mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        else if (gameObject.tag == _meshRenderer)
            mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        //for (var i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i] += normals[i] * Mathf.Sin(Time.time);
        //}



        Debug.Log(mesh.triangles.Length);
        Debug.Log(mesh.normals.Length);
        Debug.Log(mesh.vertices.Length);
        //foreach(int p in mesh.triangles)
        //{
        //    Debug.Log(p);
        //}
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(new Vector3(0,0,0), 1);
        Gizmos.color = Color.blue;
        if(mesh!=null)
        {

            //for (int i = 0; i < 10; i++)
            //{
            //    Gizmos.DrawSphere(_vertices[i], 0.01f);
            //}
            foreach (Vector3 p in mesh.vertices)
            {
                Vector3 globalVertice = transform.TransformPoint(p);
                Gizmos.DrawSphere(globalVertice, 0.01f);//global coordinates
            }
        }
       
    }
}
