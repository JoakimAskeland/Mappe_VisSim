using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Plan2 : MonoBehaviour
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

    // Dictionary for å lagre vertex indices og tilhørende triangler
    private Dictionary<int, List<int>> vertexToTriangles = new Dictionary<int, List<int>>();

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

            //LINUS NORDBAKKEN NAGY
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

            int numVertices = (resolution + 1) * (resolution + 1);
            Vector3[] vertices = new Vector3[numVertices];
            Vector2[] uv = new Vector2[numVertices];
            int[] indices = new int[resolution * resolution * 6]; // 2 triangler per rute

            // Make perfect square
            float max = xMax * 0.5f;
            if (xMax < zMax)
            {
                max = zMax * 0.5f;
            }

            float min = -max;
            float size = max - min;
            float h = size / resolution;
            //float hSize = size / 2.0f;

            int vertIndex = 0;
            int index = 0;

            for (int z = 0; z <= resolution; z++)
            {
                for (int x = 0; x <= resolution; x++)
                {

                    Vector3 vertex = new Vector3(min + (x * h), 0, min + (z * h));
                    Vector2 uvTemp = new Vector2(x / (float)resolution, z / (float)resolution);
                    vertex.y = getHeight(new Vector2(vertex.x, vertex.z), h);
                    
                    vertices[vertIndex] = vertex;
                    uv[vertIndex] = uvTemp;

                    if (x < resolution && z < resolution)
                    {
                        // Kalkulerer vertex indices for denne firkanten/to triangelene
                        int vertIndex1 = vertIndex;
                        int vertIndex2 = vertIndex + resolution + 1;
                        int vertIndex3 = vertIndex + resolution + 2;

                        // Legger til vertices i triangelet til dictionary
                        AddTriangleToVertex(vertIndex1, vertIndex2, vertIndex3);

                        // Første triangel
                        indices[index] = vertIndex1;
                        indices[index + 1] = vertIndex2;
                        indices[index + 2] = vertIndex3;

                        // Andre triangel
                        indices[index + 3] = vertIndex1;
                        indices[index + 4] = vertIndex3;
                        indices[index + 5] = vertIndex1 + 1;

                        index += 6;
                    }

                    vertIndex++;
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = indices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshFilter>().mesh = mesh;
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

        for (int i = 0; i < points.Length; i += 50) // Å sette den til 10 og 20 gjør at det bruker eksepsjonelt lang tid på å loade, men å sette den mer enn 50 reduserer tiden. 100 funker greit, 1000 gjør det om til et bybilde med skyskrapere, ikke et brukbart plan
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

    // Cramer's rule
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

        //return new Vector3(w, u, v); // Feil rekkefølge? Originalt u, v, w
        return new Vector3(u, v, w);
    }

    public Vector3 baryc(Vector2 objectPos)
    {
        Vector3 v1 = new Vector3();
        Vector3 v2 = new Vector3();
        Vector3 v3 = new Vector3();

        Vector3 baryc = new Vector3(-1, -1, -1);

        int currentTriangle = 0;
        int previousTriangle = -1;

        // Find the nearest vertex to objectPos
        int nearestVertexIndex = FindNearestVertexIndex(objectPos);

        if (nearestVertexIndex != -1 && vertexToTriangles.ContainsKey(nearestVertexIndex))
        {
            List<int> triangles = vertexToTriangles[nearestVertexIndex];

            foreach (int triangleIndex in triangles)
            {
                int i1 = mesh.triangles[triangleIndex * 3 + 1];
                int i2 = mesh.triangles[triangleIndex * 3 + 2];
                int i3 = mesh.triangles[triangleIndex * 3 + 0];

                v1 = mesh.vertices[i1];
                v2 = mesh.vertices[i2];
                v3 = mesh.vertices[i3];

                baryc = getBary(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z), objectPos);

                if (baryc is { x: >= 0, y: >= 0, z: >= 0 })
                {
                    currentTriangle = triangleIndex;
                    break;
                }
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

        // Gir verdenskoordinater
        return baryc.x * v1 + baryc.y * v2 + baryc.z * v3;
    }


    // Legger til et triangels vertices til dictionary
    private void AddTriangleToVertex(int vertIndex1, int vertIndex2, int vertIndex3)
    {
        AddVertexToDictionary(vertIndex1, vertIndex2);
        AddVertexToDictionary(vertIndex2, vertIndex3);
        AddVertexToDictionary(vertIndex3, vertIndex1);
    }

    // Legger til vertex koblinger til dictionary
    private void AddVertexToDictionary(int vertexIndex, int connectedVertexIndex)
    {
        if (!vertexToTriangles.ContainsKey(vertexIndex))
        {
            vertexToTriangles[vertexIndex] = new List<int>();
        }

        if (!vertexToTriangles[vertexIndex].Contains(connectedVertexIndex))
        {
            vertexToTriangles[vertexIndex].Add(connectedVertexIndex);
        }
    }

    // Finner nærmeste vertex index til ballens(objectPos) posisjon
    private int FindNearestVertexIndex(Vector2 objectPos)
    {
        int nearestVertexIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vertex = mesh.vertices[i];
            Vector2 vertexPosition = new Vector2(vertex.x, vertex.z);

            // Finner avstanden mellom vertex og ballen(objectPos)
            float distance = Vector2.Distance(vertexPosition, objectPos);

            // Sjekker om dette vertexet er nærmere enn det som for øyeblikket er markert som nærmeste vertex
            if (distance < minDistance)
            {
                minDistance = distance; 
                nearestVertexIndex = i;
                //Debug.Log("nearestVertexIndex: " + nearestVertexIndex);
            }
        }

        return nearestVertexIndex;
    }
}
