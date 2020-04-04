using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RamBuoyancy : MonoBehaviour
{
    public float buoyancy = 30;
    public float viscosity = 2;
    public float viscosityAngular = 0.4f;

    public LayerMask layer = 16;

    public new Collider collider;

    [Range(2,10)]
    public int pointsInAxis = 2;
    new Rigidbody rigidbody;
    static RamSpline[] ramSplines;
    static LakePolygon[] lakePolygons;

    List<Vector3> vertices = new List<Vector3>();
    Vector3[] verticesMatrix;
    Vector3 lowestPoint;
    Vector3 center = Vector3.zero;

    public bool debug = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (ramSplines == null)
            ramSplines = FindObjectsOfType<RamSpline>();
        if (lakePolygons == null)
            lakePolygons = FindObjectsOfType<LakePolygon>();

        if (collider == null)
            collider = GetComponent<Collider>();

        if (collider == null)
        {
            Debug.LogError("Buoyancy doesn't have collider");
            this.enabled = false;
            return;
        }

        Vector3 size = collider.bounds.size;
        Vector3 min = collider.bounds.min;
        Vector3 step = new Vector3(size.x / (float)pointsInAxis, size.y / (float)pointsInAxis, size.z / (float)pointsInAxis);

        for (int x = 0; x <= pointsInAxis; x++)
        {
            for (int y = 0; y <= pointsInAxis; y++)
            {
                for (int z = 0; z <= pointsInAxis; z++)
                {
                    Vector3 vertice = new Vector3(min.x + x * step.x, min.y + y * step.y, min.z + z * step.z);
                    Vector3 closestPoint = collider.ClosestPoint(vertice);

                    if (Vector3.Distance(closestPoint, vertice) < float.Epsilon)
                    {
                        vertices.Add(transform.InverseTransformPoint(vertice));
                    }

                }
            }
        }

        verticesMatrix = new Vector3[vertices.Count];

        //Debug.Log(gameObject.name + " " + vertices.Count);

    }

    private void FixedUpdate()
    {

        WaterPhysics();

    }


    public void WaterPhysics()
    {

        Ray ray = new Ray();
        ray.direction = Vector3.up;
        RaycastHit hit;


    

        bool backFace = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;

        var thisMatrix = transform.localToWorldMatrix;

        lowestPoint = vertices[0];
        float minY = float.MaxValue;
        for (int i = 0; i < vertices.Count; i++)
        {
            verticesMatrix[i] = thisMatrix.MultiplyPoint3x4(vertices[i]);

            if (minY > verticesMatrix[i].y)
            {
                lowestPoint = verticesMatrix[i];
                minY = lowestPoint.y;
            }
        }

        ray.origin = lowestPoint;

        center = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 100, layer))
        {
            float width = Mathf.Max(collider.bounds.size.x, collider.bounds.size.z);

            int verticesCount = 0;

            Vector3 velocity = rigidbody.velocity;

            Vector3 velocityDirection = velocity.normalized;

            minY = hit.point.y;

            for (int i = 0; i < verticesMatrix.Length; i++)
            {
                if (verticesMatrix[i].y <= minY)
                {
                    center += verticesMatrix[i];
                    verticesCount++;
                }
            }
            center /= verticesCount;
            //Debug.Log(minY - center.y);
            rigidbody.AddForceAtPosition(Vector3.up * buoyancy * (minY - center.y), center);

            rigidbody.AddForce(velocity * -1 * viscosity);


            if (velocity.magnitude > 0.01f)
            {
                Vector3 v1 = Vector3.Cross(velocity, new Vector3(1, 1, 1)).normalized;

                Vector3 v2 = Vector3.Cross(velocity, v1).normalized;


                Vector3 pointFront = velocity.normalized * 10;
                Ray rayCollider;

                //int test = 0;
                RaycastHit hitCollider;
                
                foreach (var item in verticesMatrix)
                {
                    Vector3 start = pointFront + item;
                    rayCollider = new Ray(start, -velocityDirection);

                    //Debug.Log(start + " " + v1 + " " + v2);

                    //Debug.DrawRay(start, -velocityDirection * 50, Color.cyan);
                    if (collider.Raycast(rayCollider, out hitCollider, 50))
                    {
                        Vector3 pointVelocity = rigidbody.GetPointVelocity(hitCollider.point);
                        rigidbody.AddForceAtPosition(-pointVelocity * viscosityAngular, hitCollider.point);
                        //Debug.Log(hitCollider.point);
                        if (debug)
                            Debug.DrawRay(hitCollider.point, -pointVelocity * viscosityAngular, Color.red, 0.1f);
                    }
                }


                //verticesMatrix
                //for (float x = -width; x < width; x += width * 0.1f)
                //{
                //    for (float y = -width; y < width; y += width * 0.1f)
                //    {
                //        test++;
                //        Vector3 start = pointFront + (v1 * x) + (v2 * y);
                //        rayCollider = new Ray(start, -velocityDirection);

                //        //Debug.Log(start + " " + v1 + " " + v2);

                //        //Debug.DrawRay(start, -velocityDirection*50, Color.cyan);
                //        if (collider.Raycast(rayCollider, out hitCollider, 50))
                //        {
                //            Vector3 pointVelocity = rigidbody.GetPointVelocity(hitCollider.point);
                //            rigidbody.AddForceAtPosition(-pointVelocity * viscosityAngular, hitCollider.point);
                //            //Debug.Log(hitCollider.point);
                //            if (debug)
                //                Debug.DrawRay(hitCollider.point, -pointVelocity * viscosityAngular, Color.red, 0.1f);
                //        }
                //    }
                //}
                //Debug.Log(test);
            }

            RamSpline ramSpline = hit.collider.GetComponent<RamSpline>();
            LakePolygon lakePolygon = hit.collider.GetComponent<LakePolygon>();
            if (ramSpline != null)
            {
                Mesh meshRam = ramSpline.meshfilter.sharedMesh;
                int verticeId1 = meshRam.triangles[hit.triangleIndex * 3];

                Vector3 verticeDirection = ramSpline.verticeDirection[verticeId1];

                Vector2 uv4 = meshRam.uv4[verticeId1];

                verticeDirection = verticeDirection * uv4.y - new Vector3(verticeDirection.z, verticeDirection.y, -verticeDirection.x) * uv4.x;

                rigidbody.AddForce(new Vector3(verticeDirection.x, 0, verticeDirection.z) * ramSpline.floatSpeed);


                if (debug)
                    Debug.DrawRay(center, Vector3.up * buoyancy * (minY - center.y) * 5, Color.blue);
                if (debug)
                    Debug.DrawRay(transform.position, velocity * -1 * viscosity * 5, Color.magenta);
                if (debug)
                    Debug.DrawRay(transform.position, velocity * 5, Color.grey);
                if (debug)
                    Debug.DrawRay(transform.position, rigidbody.angularVelocity * 5, Color.black);
            }
            else if (lakePolygon != null)
            {
                Mesh meshLake = lakePolygon.meshfilter.sharedMesh;
                int verticeId1 = meshLake.triangles[hit.triangleIndex * 3];


                Vector2 uv4 = -meshLake.uv4[verticeId1];

                //Debug.Log(uv4);
                Vector3 verticeDirection = new Vector3(uv4.x, 0, uv4.y);// Vector3.forward * uv4.y + new Vector3(0, 0, 1) * uv4.x;

                rigidbody.AddForce(new Vector3(verticeDirection.x, 0, verticeDirection.z) * lakePolygon.floatSpeed);
                if (debug)
                    Debug.DrawRay(transform.position + Vector3.up, verticeDirection * 5, Color.red);

                if (debug)
                    Debug.DrawRay(center, Vector3.up * buoyancy * (minY - center.y) * 5, Color.blue);
                if (debug)
                    Debug.DrawRay(transform.position, velocity * -1 * viscosity * 5, Color.magenta);
                if (debug)
                    Debug.DrawRay(transform.position, velocity * 5, Color.grey);
                if (debug)
                    Debug.DrawRay(transform.position, rigidbody.angularVelocity * 5, Color.black);

            }
        }




     
        Physics.queriesHitBackfaces = backFace;
    }

    void OnDrawGizmosSelected()
    {
        if (!debug)
            return;


        if (collider != null && verticesMatrix != null)
        {

            var thisMatrix = transform.localToWorldMatrix;
            foreach (var item in verticesMatrix)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(item, .08f);
            }

        }

        if (lowestPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(lowestPoint, .08f);
        }

        if (center != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(center, .08f);

        }
    }

}
