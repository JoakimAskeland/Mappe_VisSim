using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Mostly using my code from Oblig2 in this script, it was called ballMovement in that assignment as well.
public class ballMovement : MonoBehaviour
{
    public GameObject ball;
    public GameObject trianglePlane;
    public List<Vector3> trianglePoints;

    [SerializeField] private int TriangleNumber = 1;
    private Vector3 triangleNormal;
    [SerializeField] private Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public float mass = 1f;
    [SerializeField] private Vector3 acceleration = Vector3.zero;
    [SerializeField] private Vector3 velocity = new Vector3(40f, 1f, 40f);
    float radius = 5f;
    private Vector3 previousNormal;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Ball");
        trianglePlane = GameObject.Find("Plan fra 2.3-.24"); 

        if (trianglePlane != null)
        {
            trianglePoints = trianglePlane.GetComponent<TestAvPlan>().vertices; // M� endres?
        }
        else { Debug.LogError("trianglePlane GameObject not found"); }

        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

        previousNormal = Vector3.up;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        barycentricCoords();

        // Visualization of some vectors on the ball
        Debug.DrawRay(transform.position, acceleration, Color.yellow);
        Debug.DrawRay(transform.position, velocity, Color.green);
        Debug.DrawRay(transform.position, gravity, Color.red); 
    }

    void barycentricCoords()
    {
        Vector3 ballPosition = transform.position;

        for (int i = 0; i < trianglePoints.Count; i += 3)
        {
            Vector3 v0 = Vector3.zero;
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;

            v0 = trianglePoints[i];
            v1 = trianglePoints[i + 1];
            v2 = trianglePoints[i + 2];

            if (TriangleNumber < trianglePoints.Count / 3)
            {
                //Debug.Log("Triangle " + TriangleNumber);
                TriangleNumber++;
            }

            //Debug.Log("v0: " + v0 + " | v1: " + v1 + " | v2: " + v2);

            Vector3 v0v1 = v1 - v0;
            Vector3 v0v2 = v2 - v0;
            Vector3 v0bp = ballPosition - v0;

            //Debug.Log("v0v1: " + v0v1 + " | v0v2: " + v0v2);

            // Overstack, Cramer's rule
            float dot00 = Vector3.Dot(v0v1, v0v1);
            float dot01 = Vector3.Dot(v0v1, v0v2);
            float dot11 = Vector3.Dot(v0v2, v0v2);
            float dot20 = Vector3.Dot(v0bp, v0v1);
            float dot21 = Vector3.Dot(v0bp, v0v2);
            float denom = dot00 * dot11 - dot01 * dot01;
            float v = (dot11 * dot20 - dot01 * dot21) / denom;
            float w = (dot00 * dot21 - dot01 * dot20) / denom;
            float u = 1.0f - v - w;

            //float dot00 = Vector3.Dot(v0v1, v0v1);
            //float dot01 = Vector3.Dot(v0v1, v0v2);
            //float dot02 = Vector3.Dot(v0v1, v0bp);
            //float dot11 = Vector3.Dot(v0v2, v0v2);
            //float dot12 = Vector3.Dot(v0v2, v0bp);

            ////float denom = dot00 * dot11 - dot01 * dot01;

            ////float u = (dot11 * dot02 - dot01 * dot12) / denom;
            ////float v = (dot00 * dot12 - dot01 * dot02) / denom;
            ////float w = 1 - u - v;

            //float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            //float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            //float v = (dot00 * dot12 - dot01 * dot02) * invDenom;
            //float w = 1 - u - v;

            //Debug.Log("u: " + u + "v: " + v + "w: " + w);

            Vector3 barycWorldCoord = new Vector3(u, v, w);
            float height = v0.y * w + v1.y * u + v2.y * v;
            //Debug.Log("height: " + height);

            bool isInsideTriangle = (u >= 0) && (v >= 0) && (w >= 0) && (u + v < 1);

            if (isInsideTriangle)
            {
                //Debug.Log("Ball is inside triangle");
                Debug.Log("isInsideTriangle: " + isInsideTriangle);
                Debug.Log("Acceleration: " + acceleration + " | Velocity: " + velocity + " | normal: " + triangleNormal);


                Vector3 collisionPoint = closestPointOnTriangle(ballPosition, v0, v1, v2);

                triangleNormal = Vector3.Cross(v0v1, v0v2).normalized;
                var triangleNormalnonNormalized = Vector3.Cross(v1 - v0, v2 - v0);
                Debug.Log("u: " + u + "v: " + v + "w: " + w);
                Debug.Log("v0v1: " + v0v1 + " | v1(" + v1 + ") - v0(" + v0);
                Debug.Log("v0v2: " + v0v2 + " | v2(" + v2 + ") - v0(" + v0);

                // Calculating acceleration
                Vector3 gravityProjection = -Vector3.Dot(gravity, triangleNormal) * triangleNormal;
                //Debug.Log("gravityprojection: " + gravityProjection);
                //Debug.Log("TriangleNormal: " + triangleNormal);
                Debug.Log("TriangleNormalnonNormalized: " + triangleNormalnonNormalized);
                acceleration = gravityProjection + gravity;

                //Debug.Log("Acceleration: " + acceleration + " | Velocity: " + velocity + " | normal: " + triangleNormal);
                velocity = Vector3.ProjectOnPlane(velocity, triangleNormal); // Anders
                //Debug.Log("Acceleration: " + acceleration + " | Velocity: " + velocity + " | normal: " + triangleNormal);
                var newVelocity = velocity + acceleration * Time.deltaTime;
                velocity = newVelocity;

                var newPosition = ballPosition + velocity * Time.deltaTime;
                ballPosition = newPosition;

                Debug.Log("Height: " + height + " | newPosition: " + newPosition);

                //transform.position = ballPosition;
                transform.position = new Vector3(newPosition.x, height + radius - 0.03f, newPosition.z);
                //transform.position = newPosition;
                //transform.position = collisionPoint;
                //transform.up = triangleNormal;
                Debug.Log("collisonPoint: " + collisionPoint);
                // ||||||||||||||||||||||| Virker som at h�yden blir feil....

                // New triangle
                if (!sameNormal(triangleNormal, previousNormal))
                {
                    //Debug.Log("New triangle");

                    previousNormal = triangleNormal;
                }
                Debug.Log("Acceleration: " + acceleration + " | Velocity: " + velocity + " | normal: " + triangleNormal);
            }
            else
            {
                //Debug.Log("Ball is outside triangle");
                //Debug.Log("isInsideTriangle: " + isInsideTriangle);

                transform.position += velocity*Time.deltaTime + 0.5f*gravity * Time.deltaTime * Time.deltaTime; // Anders
            }
            //isInsideTriangle = false;

            if (ballPosition.y <= -10f)
                transform.position = new Vector3(ballPosition.x, -10f, ballPosition.z);

        }
    }

    bool sameNormal(Vector3 normalA, Vector3 normalB)
    {
        float angleTolerance = 0.1f;
        float angle = Vector3.Angle(normalA, normalB);

        return angle <= angleTolerance;
    }

    Vector3 closestPointOnTriangle(Vector3 point, Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 edgeAB = B - A;
        Vector3 edgeAC = C - A;
        Vector3 edgeBC = C - B;

        Vector3 pointToA = point - A;

        float dotAB = Vector3.Dot(pointToA, edgeAB);
        float dotAC = Vector3.Dot(pointToA, edgeAC);

        if (dotAB <= 0f && dotAC <= 0f)
        {
            return A;
        }

        Vector3 pointToB = point - B;
        float dotBC = Vector3.Dot(pointToB, -edgeBC);
        float dotBA = -Vector3.Dot(pointToB, -edgeAB);

        if (dotBC <= 0f && dotBA <= 0f)
        {
            return B;
        }

        float dotACBC = Vector3.Dot(edgeAC, edgeBC);
        float dotACPoint = Vector3.Dot(edgeAC, pointToA);
        float dotBCPoint = -Vector3.Dot(edgeBC, pointToB);

        float denom = 1f / (dotACBC * dotACBC - edgeAC.sqrMagnitude * edgeBC.sqrMagnitude);

        float parameterU = (dotBC * dotACBC + dotACPoint * edgeBC.sqrMagnitude - dotBCPoint * dotACBC) * denom;
        float parameterV = (dotAC * dotACBC + dotBCPoint * edgeAC.sqrMagnitude - dotACPoint * dotACBC) * denom;

        if (parameterU <= 0f)
        {
            return A + parameterV * edgeAC;
        }
        else if (parameterV <= 0f)
        {
            return B + parameterU * -edgeBC;
        }
        else if (parameterU + parameterV >= 1f)
        {
            return C + parameterU * edgeAC + parameterV * edgeBC;
        }
        else
        {
            return A + parameterV * edgeAC + parameterU * -edgeBC;
        }
    }
}
