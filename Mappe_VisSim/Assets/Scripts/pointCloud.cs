using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class pointCloud : MonoBehaviour
{
    public string fileName = "hoydedata.txt";
    public Material material;
    public float pointSize = 0.1f;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            Vector3[] positions = new Vector3[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(' ');

                if (values.Length >= 3)
                {
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]); // Might have to be 2
                    float z = float.Parse(values[2]); // Might have to be 1
                    positions[i] = new Vector3(x, y, z);
                }
            }

            lineRenderer.positionCount = lines.Length;
            //lineRenderer.positionCount = lines.Length;
            lineRenderer.SetPositions(positions);
            lineRenderer.startWidth = pointSize;
            lineRenderer.endWidth = pointSize;
            lineRenderer.material = material;
        }
        else
        {
            Debug.LogError("File not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
