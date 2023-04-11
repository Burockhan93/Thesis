using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    private const string _skinnedMesh = "SkinnedMesh";
    private const string _meshRenderer = "MeshRenderer";
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;
    //Ek olarak sadece belli vertexlere
    List<int> displacedVerticeIndices = new List<int>();
   
    //Debug amacli. bu noktanin etrafinda olacak iceri göcme 
    Vector3? impactpoint;
    bool drawGizmos;
    //gecici
    float impactforce = 0.1f;
    //deformation forces
    public float springForce = 30f;//determines how fast the vertex velocities die down. Higher value measn faster(30-25 is ideal)
    public float damping = 5f; // to fight against oscillation. Higher value means less oscillation but overall slower convergence. 
    float uniformScale = 1f;
    //how deep will the mesh deform
    float _deformDepth = 0.005f;
    //radious of impactpoints effect
    float _impactRadius = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.tag ==_skinnedMesh)
            deformingMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        else if(gameObject.tag == _meshRenderer)
            deformingMesh = GetComponent<MeshFilter>().mesh;

        originalVertices = deformingMesh.vertices;
        
        vertexVelocities = new Vector3[originalVertices.Length];
        displacedVertices = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
    }
    public void AddDeformingForce(Vector3 point, float force,float depth, Vector3 normal)
    {
        Debug.DrawLine(Camera.main.transform.position, point);
        //Find nearest Vertex to the hit
        impactpoint = NearestVertex(point);
        //Determine the direction of deformation
         impactpoint = point-normal* depth;//this gives us a location inside the mesh
        drawGizmos = true;
       
        int index = 0;
        foreach (Vector3 vertice in deformingMesh.vertices)
        {
            Vector3 globalVertice = transform.TransformPoint(vertice);

            float distance = Vector3.Distance(globalVertice, point);
            //Determine the indexes of deformed meshes. Loop through entire original mesh, add the index to the list if the distance
            // between vertice and hit point smaller than given distance
            if (distance < _impactRadius)
            {
                displacedVerticeIndices.Add(index);
            }
            index++;

        }
        Debug.Log(displacedVerticeIndices.Count);
       
        //from global to local
        // point = transform.InverseTransformPoint(point);

        //for (int i = 0; i < displacedVertices.Length; i++)
        //{
        //    AddForceToVertex(i, point, force);
        //}

        //New
        // Add forces to the vertices in effect area. 
        for (int i = 0; i < displacedVerticeIndices.Count; i++)
        {
            //  Debug.Log(displacedVerticeIndices.Count);
            //AddForceToVertex(displacedVerticeIndices[i], point, force,true);
            AddForceToVertex(displacedVerticeIndices[i], impactpoint.Value, force, true);
        }
        //displacedVerticeIndices = new List<int>();
         displacedVerticeIndices.Clear();

    }
    
    //Old Methods
    void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        pointToVertex *= uniformScale;
        //point tikladgmz kisim bunun uzakliginin karesi+1 e bagli olarak güc 
        //azalacak uygulanan.
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        //yumusatioz poweri, frame rateden bagmsz hale getirio
        float velocity = attenuatedForce * Time.deltaTime;

        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
    //New Method, b is not important
    void AddForceToVertex(int i, Vector3 point, float force, bool b )
    {
        // find the direction of impact
        //Vector3 pointToVertex = (point- transform.TransformPoint( displacedVertices[i])).normalized;
        Vector3 pointToVertex = (transform.InverseTransformPoint(point) -  displacedVertices[i]).normalized;
        pointToVertex *= uniformScale;
        //this force determines the how much each vertex will be affected. Further the vertice frim impact, less it will be deformed.
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        //Force basically means change velocity for the vertices
        float velocity = attenuatedForce * Time.deltaTime; // Add clamp value here
        
        // new velocities will be given to the vertices
        vertexVelocities[i] += pointToVertex * velocity;

        Vector3 speed = new Vector3(Mathf.Clamp(vertexVelocities[i].x, -0.02f, 0.02f), Mathf.Clamp(vertexVelocities[i].y, -0.02f, 0.02f), Mathf.Clamp(vertexVelocities[i].z, -0.02f, 0.02f));
        vertexVelocities[i] = speed;

    }
    void Update()
    {
        //in each update we update displacements 
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();

        
    }
    void UpdateVertex(int i)
    {
        //adds a 100 fps to the performance
        if (vertexVelocities[i].magnitude == 0) return;
        //We check to see 
        Vector3 velocity = vertexVelocities[i];

        //Determione the displacement between original and deformed vertices
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        //displacement for other scales-not important currently
        displacement *= uniformScale;
        //velocity should slow down if displacement becomes larger(spring effect: original vertices are stable, dispalced ones are bound by the spring) 
        velocity -= displacement * springForce * Time.deltaTime;
        //constantly reduce the velocity so that no oscilattion occurs.
        velocity *= 1 - damping * Time.deltaTime;
        //update velocity array with slowing velocity
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * Time.deltaTime/uniformScale;
    }
    public Vector3 NearestVertex(Vector3 point)
    {

        // Convert point to local space.
        point = transform.InverseTransformPoint(point);

        float minDistanceSqr = Mathf.Infinity;
        Vector3 nearestVertex = Vector3.zero;  

        //for(int i=0; i<deformingMesh.vertexCount; i++)
        //{
        //    Vector3 diff = point - deformingMesh.vertices[i];
        //    float distSqr = diff.sqrMagnitude;

        //    if (distSqr < minDistanceSqr)
        //    {

        //        minDistanceSqr = distSqr;
        //        nearestVertex = deformingMesh.vertices[i];
               

        //    }
        //}
        // Check all vertices to find nearest.
        foreach (Vector3 vertex in deformingMesh.vertices)
        {

            Vector3 diff = point - vertex;
            float distSqr = diff.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {

                minDistanceSqr = distSqr;
                nearestVertex = vertex;

            }

        }
      
        // Convert nearest vertex back to the world space.
        return transform.TransformPoint(nearestVertex);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.magenta;
        if (!drawGizmos) return;
        else
        {
            
            foreach (int i in displacedVerticeIndices)
            {
                Gizmos.DrawSphere(transform.TransformPoint( deformingMesh.vertices[i]), 0.0051f);
            }
            Gizmos.color = UnityEngine.Color.blue;
            Gizmos.DrawCube( impactpoint.Value, new Vector3(0.005f, 0.005f, 0.005f));
            Gizmos.DrawCube( impactpoint.Value, new Vector3(0.05f, 0.05f, 0.05f));
        }
    }
}
