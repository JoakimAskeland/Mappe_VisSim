using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class testAvBall : MonoBehaviour
{
    [SerializeField] public TestAvPlan trianglePlane;
    [SerializeField] public float radius = 5f;
    [SerializeField] private Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    [SerializeField] private Vector3 acceleration = Vector3.zero;

    float mass = 1f;

    public Vector3 currentVelocity = new();
    public Vector3 newVelocity = Vector3.zero;
    Vector3 previousPosition;
    Vector3 newPosition;

    //private int triangle = -1;
    private float baryY;

    private void Awake()
    {
        previousPosition = transform.position;
        newPosition = transform.position;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
    }

    private void FixedUpdate()
    {
        Vector3 force = new Vector3();

        if (CollisionDetectionPlane())
        {
            Vector3 surfaceNormal = trianglePlane.normalV;
            Vector3 normalF = -Vector3.Dot(gravity, surfaceNormal) * surfaceNormal;
            force = gravity + normalF;
            acceleration = force;
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalF);

            if (trianglePlane.enteredTriangle)
            {
                trianglePlane.enteredTriangle = false;
                normalF = (surfaceNormal + surfaceNormal).normalized;
                currentVelocity = Vector3.ProjectOnPlane(currentVelocity, normalF);
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

    private bool CollisionDetectionPlane()
    {
        Vector3 pos = transform.position;
        Vector3 baryc = trianglePlane.baryc(new Vector2(pos.x, pos.z));
        Vector3 normalVec = trianglePlane.normalV;

        float dotProduct = Vector3.Dot(baryc - pos, normalVec);

        if (Mathf.Abs(dotProduct) <= radius)
        {
            baryY = baryc.y;
            Vector3 collisionPos = pos + dotProduct * normalVec;
            return true;
        }
        return false;
    }
}
