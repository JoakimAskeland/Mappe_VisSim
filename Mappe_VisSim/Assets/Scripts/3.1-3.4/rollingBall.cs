using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rollingBall : MonoBehaviour
{
    [SerializeField] public Plan2 trianglePlane;
    [SerializeField] public float radius = 5f;
    [SerializeField] public float mass = 1f;
    [SerializeField] private Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    public Vector3 currentVelocity = new();
    public Vector3 newVelocity = Vector3.zero;
    Vector3 newPosition;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 force = new Vector3();

        if (CollisionDetectionPlane())
        {
            //Debug.Log("Collision detected");
            Vector3 triangleNormal = trianglePlane.normalV;
            Vector3 normalForce = -Vector3.Dot(gravity, triangleNormal) * triangleNormal;
            force = gravity + normalForce;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);

            if (trianglePlane.enteredTriangle)
            {
                trianglePlane.enteredTriangle = false;
                normalForce = (triangleNormal + triangleNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalForce);

                // Bouncing effect
                //currentVelocity = currentVelocity + new Vector3(0, 20, 0);
            }
        }
        else
        {
            acceleration = gravity * mass;
        }

        newVelocity = currentVelocity + acceleration * Time.deltaTime;
        currentVelocity = newVelocity;
        newPosition = transform.position + newVelocity * Time.deltaTime;
        transform.position = newPosition;

        Debug.DrawRay(transform.position, acceleration, Color.yellow);
        Debug.DrawRay(transform.position, newVelocity, Color.green);
        Debug.DrawRay(transform.position, gravity, Color.red);
    }

    // God hjelp fra Linus Norbakken Nagy
    private bool CollisionDetectionPlane()
    {
        Vector3 ballPosition = transform.position;
        Vector3 barycentricCoordinates = trianglePlane.baryc(new Vector2(ballPosition.x, ballPosition.z));
        Vector3 normalVector = trianglePlane.normalV;
        float dot = Vector3.Dot(barycentricCoordinates - ballPosition, normalVector);

        if (Mathf.Abs(dot) <= radius)
        {
            return true;
        }

        return false;
    }
}
