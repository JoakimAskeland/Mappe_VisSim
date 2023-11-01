using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class cubeSpawner : MonoBehaviour
{
    public string fileName = "hoydedata.txt";
    public float cubeSize = 0.1f;
    public GameObject cubePrefab;

    [SerializeField] private Mesh cubeInstance;
    [SerializeField] private Material cubeMaterial;
    private RenderParams rp;

    // GPU instancing
    private List<Matrix4x4> tempMatrice = new List<Matrix4x4>();
    private List<List<Matrix4x4>> matrices = new List<List<Matrix4x4>>();
    private int lists;
    private Vector3[] points; 

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        float xMin = float.MaxValue;
        float xMax = float.MinValue;

        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        float zMin = float.MaxValue;
        float zMax = float.MinValue;


        if (File.Exists(filePath))
        {
            //StreamReader sr = new StreamReader(filePath);
            //int lines = int.Parse(sr.ReadLine());
            string[] lines = File.ReadAllLines(filePath);
            points = new Vector3[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(' ');

                if (values.Length >= 3)
                {
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[2]); 
                    float z = float.Parse(values[1]);

                    if (xMax < x) { xMax = x; }
                    if (xMin > x) { xMin = x; }

                    if (yMax < y) { yMax = y; }
                    if (yMin > y) { yMin = y; }

                    if (zMax < z) { zMax = z; }
                    if (zMin > z) { zMin = z; }

                    points[i] = new Vector3(x, y, z);

                    //vertices.Add(new Vector3(x, y, z));

                    //Vector3 position = new Vector3(x, y, z);
                    //CreateCube(position);

                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i].x -= 0.5f * (xMin + xMax);
                points[i].y -= 0.5f * (yMin + yMax);
                points[i].z -= 0.5f * (zMin + zMax);
            }
                    
            for (int i = 0; i < points.Length; i++)
            {
                Matrix4x4 matrix = Matrix4x4.Translate(points[i]);
                tempMatrice.Add(matrix);

                    if (tempMatrice.Count >= 100000)
                    {
                        lists++;
                        matrices.Add(tempMatrice);
                        tempMatrice.Clear();
                    }

            }

            //RenderCubes();
            rp = new RenderParams(cubeMaterial);
        }
        else
        {
            Debug.LogError("File not found");
        }
    }

    //void CreateCube(Vector3 position)
    //{
    //    GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
    //    cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
    //}

    //void RenderCubes()
    //{
    //    Debug.LogError("RenderCubes");
    //    MaterialPropertyBlock props = new MaterialPropertyBlock();
    //    GameObject cubeContainer = new GameObject("CubeContainer");
    //    for (int i = 0; i < matrices.Count; i++)
    //    {
    //        GameObject cube = Instantiate(cubePrefab, cubeContainer.transform);
    //        props.SetMatrix("_ObjectToWorld", matrices[i]);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lists; i++)
        {
            Graphics.RenderMeshInstanced(rp, cubeInstance, 0, matrices[i]);
        }
        
    }
}
