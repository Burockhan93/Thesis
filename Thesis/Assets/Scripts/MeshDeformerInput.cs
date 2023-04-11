using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float forceOffset = 0.1f;
    public float force = 10f;
    float _timer;

    MeshDeformer deformer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (Input.GetMouseButton(0) && _timer >0.1f)
        {
            HandleInput();
            _timer = 0;
        }
       
    }


    int closest = 0;
    void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        //if (Physics.Raycast(inputRay, out hit))
        //{
        //    MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
        //    if (deformer)
        //    {
        //        Vector3 point = hit.point;
        //        point += hit.normal * forceOffset;//so that deforming occurs inwards
        //        deformer.AddDeformingForce(point, force);
        //    }

        //}
        if ((Physics.Raycast(inputRay, out hit)))
        {
            //MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            deformer = hit.collider.GetComponent<MeshDeformer>();
            
            if (deformer)
            {
                Vector3 point = hit.point;               
                //point += hit.normal * forceOffset;//so that deforming occurs inwards
                deformer.AddDeformingForce(point, force,0.025f,hit.normal);//manuel depthoffset for camera
            }
           

        }
        
    }
   


}
