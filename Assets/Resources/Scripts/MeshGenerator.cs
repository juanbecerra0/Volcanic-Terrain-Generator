using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
<<<<<<< Updated upstream
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public void GenerateMesh(int vertexCount, int blockSize, float[,] heightmap) {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
=======
    // Generates y coordinates based on heightmap data, then update the mesh
    public Mesh GenerateMesh(float[,] heightmap, int blockSize, Vector3 worldPosition)
    {
        Debug.Log(worldPosition.x + " : " + worldPosition.z);
        Mesh mesh = new Mesh();
        Vector3[] vertices;
        int[] triangles;
>>>>>>> Stashed changes

        CreateShape(vertexCount, blockSize, heightmap);
        UpdateMesh();
    }

    void CreateShape(int vertexCount, int blockSize, float[,] heightmap)
    {
        vertexCount += vertexCount; // Need one more vertex on each dimension in order to make (n + 1) by (n + 1) mesh
        vertices = new Vector3[(vertexCount + 1) * (vertexCount + 1)];

        float heightmapSize = heightmap.GetLength(0);
        float vertexSpacing = (float)blockSize / (float)vertexCount;

        for (int i = 0, z = 0; z <= vertexCount; z++)
        {
            for (int x = 0; x <= vertexCount; x++, i++)
            {
<<<<<<< Updated upstream
                int xIndex = Mathf.FloorToInt(((float)x / (float)vertexCount) * (heightmapSize - 0.01f));
                int zIndex = Mathf.FloorToInt(((float)z / (float)vertexCount) * (heightmapSize - 0.01f));

                float xComp = (float)x * vertexSpacing;
                float yComp = heightmap[xIndex, zIndex];
                float zComp = (float)z * vertexSpacing;
=======
                float xComp = ((float)x * vertexSpacing) + worldPosition.x;
                float yComp = heightmap[xIndex, yIndex++];
                float zComp = ((float)z * vertexSpacing) + worldPosition.z;
>>>>>>> Stashed changes

                vertices[i] = new Vector3(xComp, yComp, zComp);
            }
        }

        triangles = new int[vertexCount * vertexCount * 6];
        int vert = 0, tris = 0;

        for (int z = 0; z < vertexCount; z++)
        {
            for (int x = 0; x < vertexCount; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + vertexCount + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + vertexCount + 1;
                triangles[tris + 5] = vert + vertexCount + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
<<<<<<< Updated upstream
    }

    void UpdateMesh()
    {
=======

        // Update mesh components
>>>>>>> Stashed changes
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        // Return generated mesh
        return mesh;
    }
}
