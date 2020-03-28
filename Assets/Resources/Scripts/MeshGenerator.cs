using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    
    // Generates y coordinates based on heightmap data, then update the mesh
    public void GenerateMesh(float[,] heightmap, Texture2D texture, int blockSize)
    {
        // Set up mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Get width of vertices map and the uv scale
        int vertexWidth = heightmap.GetLength(1);

        // Set up vertex and uv arrays
        Vector3[] vertices = new Vector3[(vertexWidth) * (vertexWidth)];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Calculate the space between vertices and uv scale
        float vertexSpacing = (float) ((float)blockSize / (float)(vertexWidth - 1));
        float uvScale = 1.0f / (float)vertexWidth;

        // Set up index mapping for vertices
        int xIndex = vertexWidth - 1;
        int yIndex = 0;

        // Calculate 
        for (int i = 0, z = 0; z < vertexWidth; z++)
        {
            for (int x = 0; x < vertexWidth; x++, i++)
            {
                float xComp = (float)x * vertexSpacing;
                float yComp = heightmap[xIndex, yIndex++];
                float zComp = (float)z * vertexSpacing;

                // Assign vertex and UV coordinate
                vertices[i] = new Vector3(xComp, yComp, zComp);
                uvs[i] = new Vector2(x * uvScale, z * uvScale);
            }
            xIndex--;
            yIndex = 0;
        }

        // Set up vertex index buffer
        int[] triangles = new int[(vertexWidth - 1) * (vertexWidth - 1) * 6];
        int vert = 0, tris = 0;

        // Calculate vertex indicies
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

        // Update this mesh object
        UpdateMesh(vertices, uvs, triangles, texture);
    }

    // Updates mesh components and recalculates normals
    private void UpdateMesh(Vector3[] vertices, Vector2[] uvs, int[] triangles, Texture2D texture)
    {
        // Clear previous changes to mesh
        mesh.Clear();

        // Assign new vertices, uvs, and index buffer
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // Assign texture
        GetComponent<MeshRenderer>().material.mainTexture = texture;

        // Recalculate normals
        mesh.RecalculateNormals();
    }
}
