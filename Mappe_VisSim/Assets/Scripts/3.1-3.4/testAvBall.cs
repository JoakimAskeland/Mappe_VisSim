using System.Collections;
using System.Collections.Generic;
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

    private int triangle = -1;


    private void Awake()
    {
        previousPosition = transform.position;
        newPosition = transform.position;
        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
    }

    private void FixedUpdate()
    {
        Vector3 force = new Vector3();


    }

    private bool CollisionDetectionPlane()
    {
        Vector3 pos = transform.position;
        Vector3 baryc = trianglePlane.baryc(new Vector2(pos.x, pos.z));
        Vector3 normalVec = 
    }
}
