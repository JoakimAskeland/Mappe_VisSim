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
    float radius = 0.15f;
    private Vector3 previousNormal;

    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.Find("Ball");
        trianglePlane = GameObject.Find("Plan fra 2.3-2.4"); 

        if (trianglePlane != null)
        {
            trianglePoints = trianglePlane.GetComponent<TestAvPlan>().vertices; // Må endres?
        }
        else { Debug.LogError("trianglePlane GameObject not found"); }

        transform.localScale = new Vector3(radius * 2f, radius * 2f, radius * 2f);

        previousNormal = Vector3.up;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Visualization of some vectors on the ball
        Debug.DrawRay(transform.position, acceleration, Color.yellow);
        Debug.DrawRay(transform.position, velocity, Color.green);
    }
}
