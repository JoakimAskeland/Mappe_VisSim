using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class drawMesh : MonoBehaviour
{
    public string fileName = "hoydedata.txt"; 
    public Material material;
    public List<Vector3> vertices = new List<Vector3>();

    private void Start()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);

        Debug.LogError("fileName: " + fileName);

        float xMin = float.MaxValue;
        float xMax = float.MinValue;

        float yMin = float.MaxValue;
        float yMax = float.MinValue;

        float zMin = float.MaxValue;
        float zMax = float.MinValue;

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);



            foreach (string line in lines)
            {
                string[] coords = line.Split(' '); // This is if we use 'space' to seperate vertices in the txt file

                if (coords.Length == 3)
                {
                    float x = float.Parse(coords[0]);
                    float y = float.Parse(coords[1]);
                    float z = float.Parse(coords[2]);
                    
                    if (xMax < x) { xMax = x; }
                    if (xMin > x) { xMin = x; }

                    if (yMax < y) { yMax = y; }
                    if (yMin > y) { yMin = y; }

                    if (zMax < z) { zMax = z; }
                    if (zMin > z) { zMin = z; }

                    vertices.Add(new Vector3(x, y, z));

                }
            }
        }
    }
}
