using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public void GenerateMesh(int vertexCount) {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape(vertexCount);
        UpdateMesh();
    }

    void CreateShape(int size)
    {
        vertices = new Vector3[(size + 1) * (size + 1)];

        for (int i = 0, z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++, i++)
            {
                float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 3f;
                vertices[i] = new Vector3(x, y, z);
            }
        }

        triangles = new int[size * size * 6];
        int vert = 0, tris = 0;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size + 1;
                triangles[tris + 5] = vert + size + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
