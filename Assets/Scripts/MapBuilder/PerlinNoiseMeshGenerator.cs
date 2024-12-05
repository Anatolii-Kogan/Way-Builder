using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PerlinNoiseMeshGenerator : MonoBehaviour
{
    [Header("Mesh Settings")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10; 
    [SerializeField] private float _scale = 1f; 
    [SerializeField] private float _heightMultiplier = 2f; 
    [SerializeField] private Vector2 _noiseOffset = Vector2.zero;

    private Mesh mesh;

    private void Start()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = GenerateVertices();
        int[] triangles = GenerateTriangles();
        Vector2[] uvs = GenerateUVs(vertices);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    private Vector3[] GenerateVertices()
    {
        Vector3[] vertices = new Vector3[(_width + 1) * (_height + 1)];

        for (int z = 0; z <= _height; z++)
        {
            for (int x = 0; x <= _width; x++)
            {
                float y = Mathf.PerlinNoise((x + _noiseOffset.x) * _scale, (z + _noiseOffset.y) * _scale) * _heightMultiplier;
                vertices[z * (_width + 1) + x] = new Vector3(x, y, z);
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles()
    {
        int[] triangles = new int[_width * _height * 6];
        int triIndex = 0;

        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                int vertexIndex = z * (_width + 1) + x;

                triangles[triIndex] = vertexIndex;
                triangles[triIndex + 1] = vertexIndex + _width + 1;
                triangles[triIndex + 2] = vertexIndex + 1;

                triangles[triIndex + 3] = vertexIndex + 1;
                triangles[triIndex + 4] = vertexIndex + _width + 1;
                triangles[triIndex + 5] = vertexIndex + _width + 2;

                triIndex += 6;
            }
        }

        return triangles;
    }

    private Vector2[] GenerateUVs(Vector3[] vertices)
    {
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / _width, vertices[i].z / _height);
        }

        return uvs;
    }

    private void OnValidate()
    {
        if (_width < 1) _width = 1;
        if (_height < 1) _height = 1;
        if (_scale <= 0) _scale = 0.1f;

        if (mesh == null)
        {
            mesh = new Mesh();
        }

        GenerateMesh();
    }
}
