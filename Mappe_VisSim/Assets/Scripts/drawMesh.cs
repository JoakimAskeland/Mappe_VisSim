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
                    vertices.Add(new Vector3(x, y, z));
                }
            }

            if (vertices.Count % 3 == 0)
            {
                Mesh mesh = new Mesh();
                mesh.vertices = vertices.ToArray();
                int[] triangles = new int[vertices.Count];

                for (int i = 0; i < vertices.Count; i++)
                {
                    triangles[i] = i;
                }

                mesh.triangles = triangles;

                GameObject meshObject = new GameObject("CustomMesh");
                MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter.mesh = mesh;
                meshRenderer.material = material;

                mesh.RecalculateNormals();
            }
            else
            {
                Debug.LogError("Vertex count is not a multiple of 3.");
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
