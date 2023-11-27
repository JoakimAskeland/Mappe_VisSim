using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestAvPlan : MonoBehaviour
{
    public string fileName = "hoydedata.txt";
    private Vector3[] points;
    private Mesh mesh;

    // LINUS NORDBAKKEN NAGY
    [SerializeField][Range(1, 100)] private int resolution;
    public Vector3 previousNormalV;
    public Vector3 normalV;
    public bool enteredTriangle = false;

    public List<Vector3> vertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();

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
                }
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i].x -= 0.5f * (xMin + xMax);
                points[i].y -= 0.5f * (yMin + yMax);
                points[i].z -= 0.5f * (zMin + zMax);
            }

            // LEST INN PUNKTER HITTIL

            //LINUS NORDBAKKEN NAGY (TESTING)
            // Finding new min and max values for vertex placement
            xMin = float.MaxValue;
            xMax = float.MinValue;
            yMin = float.MaxValue;
            yMax = float.MinValue;
            zMin = float.MaxValue;
            zMax = float.MinValue;
            for (int i = 0; i < points.Length; i++)
            {

                float x = points[i].x;
                float y = points[i].y;
                float z = points[i].z;

                if (xMax < x) { xMax = x; }
                if (xMin > x) { xMin = x; }

                if (yMax < y) { yMax = y; }
                if (yMin > y) { yMin = y; }

                if (zMax < z) { zMax = z; }
                if (zMin > z) { zMin = z; }
            }

            //List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> indices = new List<int>();

            // Make perfect square? Usikker på ka det gjør og om det passer til mine datapunkter
            float max = xMax * 0.5f;
            if (xMax < zMax)
            {
                max = zMax * 0.5f;
            }

            float min = -max;
            float size = max - min;
            float h = size / resolution;
            float hSize = size / 2.0f;

            for (int z = 0; z < resolution + 1; z++)
            {
                for (int x = 0; x < resolution + 1; x++)
                {

                    Vector3 vertex = new Vector3(min + (x * h), 0, min + (z * h));
                    Vector2 uvTemp = new Vector2(x / (float)resolution, z / (float)resolution);
                    //vertex.y = CheckForPoints(new Vector2(vertex.x, vertex.z), h); // Idk bro
                    vertex.y = getHeight(new Vector2(vertex.x, vertex.z), h);
                    vertices.Add(vertex);
                    uv.Add(uvTemp);
                }
            }
            mesh.vertices = vertices.ToArray();
            mesh.uv = uv.ToArray();

            // Indices
            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    // Legge til nabo informasjon her??? -----------------------------------------------------------------------------------
                    int i = (x * resolution) + x + z;
                    // First triangle
                    indices.Add(i);
                    indices.Add(i + resolution + 1);
                    indices.Add(i + resolution + 2);
                    // Second triangle
                    indices.Add(i);
                    indices.Add(i + resolution + 2);
                    indices.Add(i + 1);
                }
            }

            mesh.triangles = indices.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    float getHeight(Vector2 vertex, float size)
    {
        List<float> values = new List<float>();
        float avgH = 0;

        // Defining area
        Vector2 topL = new Vector2(vertex.x - size, vertex.y + size);
        Vector2 topR = new Vector2(vertex.x + size, vertex.y + size);
        Vector2 botL = new Vector2(vertex.x - size, vertex.y - size);
        Vector2 botR = new Vector2(vertex.x + size, vertex.y - size);

        for (int i = 0; i < points.Length; i+= 50) // Redusere fra 50? Å sette den til 10 og 20 gjør at det bruker eksepsjonelt lang tid på å loade, men å sette den mer enn 50 reduserer tiden. 100 funker greit, 1000 gjør det om til et bybilde med skyskrapere, ikke et brukbart plan
        {
            Vector3 temp = getBary(topL, topR, botL, new Vector2(points[i].x, points[i].z));

            if (temp is { x: >= 0, y: >= 0, z: >= 0 })
                values.Add(points[i].y);
            
            else
            {
                temp = getBary(topR, botR, botL, new Vector2(points[i].x, points[i].z));

                if (temp is { x: >= 0, y: >= 0, z: >= 0 })
                    values.Add(points[i].y);
            }
        }

        if (values.Count > 0)
        {
            for (int i = 0; i < values.Count; i++)
            {
                avgH += values[i];
            }
            avgH = avgH / values.Count;
        }

        return avgH;
    }

    public Vector3 getBary(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 x)
    {
        Vector2 v0v1 = v1 - v0;
        Vector2 v0v2 = v2 - v0;
        Vector2 v0bp = x - v0;

        float dot00 = Vector2.Dot(v0v1, v0v1);
        float dot01 = Vector2.Dot(v0v1, v0v2);
        float dot02 = Vector2.Dot(v0v1, v0bp);
        float dot11 = Vector2.Dot(v0v2, v0v2);
        float dot12 = Vector2.Dot(v0v2, v0bp);

        float denom = dot00 * dot11 - dot01 * dot01;

        float u = (dot11 * dot02 - dot01 * dot12) / denom;
        float v = (dot00 * dot12 - dot01 * dot02) / denom;
        float w = 1 - u - v;

        return new Vector3(w, u, v); // Feil rekkefølge? Originalt u, v, w
    }

    public Vector3 baryc(Vector2 objectPos)
    {
        Vector3 v1 = new Vector3();
        Vector3 v2 = new Vector3();
        Vector3 v3 = new Vector3();

        Vector3 baryc = new Vector3(-1, -1, -1);

        int currentTriangle = 0;
        int previousTriangle = -1;

        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            int i1 = mesh.triangles[i * 3 + 1];
            int i2 = mesh.triangles[i * 3 + 2];
            int i3 = mesh.triangles[i * 3 + 0];

            v1 = mesh.vertices[i1];
            v2 = mesh.vertices[i2];
            v3 = mesh.vertices[i3];

            baryc = getBary(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z), objectPos);

            if (baryc is { x: >= 0, y: >= 0, z: >= 0 })
            {
                currentTriangle = i;
                break;
            }
        }

        if (previousTriangle != currentTriangle)
        {
            previousTriangle = currentTriangle;
            previousNormalV = normalV; 
            Vector3 v1v2 = v2 - v1;
            Vector3 v1v3 = v3 - v1;
            normalV = Vector3.Cross(v1v2, v1v3).normalized;
            enteredTriangle = true;
        }

        return baryc.x * v1 + baryc.y * v2 + baryc.z * v3;
    }
}
