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

    // Generates y coordinates based on heightmap data, then update the mesh
    public void GenerateMesh(float[,] heightmap, int blockSize)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int vertexWidth = heightmap.GetLength(1);

        vertices = new Vector3[(vertexWidth) * (vertexWidth)];

        float vertexSpacing = (float) ((float)blockSize / (float)(vertexWidth - 1));

        int xIndex = vertexWidth - 1;
        int yIndex = 0;

        for (int i = 0, z = 0; z < vertexWidth; z++)
        {
            for (int x = 0; x < vertexWidth; x++, i++)
            {
                float xComp = (float)x * vertexSpacing;
                float yComp = heightmap[xIndex, yIndex++];
                float zComp = (float)z * vertexSpacing;

                vertices[i] = new Vector3(xComp, yComp, zComp);
            }
            xIndex--;
            yIndex = 0;
        }

        triangles = new int[(vertexWidth - 1) * (vertexWidth - 1) * 6];
        int vert = 0, tris = 0;

        for (int z = 0; z < vertexWidth - 1; z++)
        {
            for (int x = 0; x < vertexWidth - 1; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + (vertexWidth - 1) + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + (vertexWidth - 1) + 1;
                triangles[tris + 5] = vert + (vertexWidth - 1) + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        UpdateMesh();
    }

    // Updates mesh components and recalculates normals
    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
