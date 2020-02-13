using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    int dimensions;

    public void GenerateMesh(int dimensions, float[,] heightmap) {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape(dimensions, heightmap);
        UpdateMesh();
    }

    void CreateShape(int dimensions, float[,] heightmap)
    {
        this.dimensions = dimensions + 1;
        vertices = new Vector3[(dimensions + 1) * (dimensions + 1)];
        float heightmapSize = (float) heightmap.GetLength(0);

        for (int i = 0, z = 0; z <= dimensions; z++)
        {
            for (int x = 0; x <= dimensions; x++, i++)
            {
                int xIndex = Mathf.FloorToInt(((float)x / (float)dimensions) * (heightmapSize - 0.01f));
                int zIndex = Mathf.FloorToInt(((float)z / (float)dimensions) * (heightmapSize - 0.01f));

                float y = heightmap[xIndex, zIndex];
                vertices[i] = new Vector3(x, y, z);
            }
        }

        triangles = new int[dimensions * dimensions * 6];
        int vert = 0, tris = 0;

        for (int z = 0; z < dimensions; z++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + dimensions + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + dimensions + 1;
                triangles[tris + 5] = vert + dimensions + 2;

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
