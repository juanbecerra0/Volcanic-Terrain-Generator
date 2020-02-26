using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class MeshPlacer : MonoBehaviour
{
    public int initialBlockRadius = 2;  // The initial radius from the origin of the scene
    public int heightmapBaseN = 7;      // The dimensions of generated heightmaps (2^(n)+1)
    public int blockSize = 100;         // The dimensions of a MeshGen object

    // Start is called before the first frame update
    // Initialize the map with several meshes based on input radius
    void Start()
    {
        // Error checking
        if (initialBlockRadius < 1)
            initialBlockRadius = 1;

        if (heightmapBaseN < 3)
            heightmapBaseN = 3;

        if (blockSize < 1)
            blockSize = 1;

        // Initialize HeightmapGenerator prefab, create instance, and attatch script
        GameObject heightmapGeneratorPrefab = (GameObject)Resources.Load("Prefabs/HeightmapGenerator");
        GameObject heightmapGeneratorInstance = (GameObject)GameObject.Instantiate(heightmapGeneratorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenerator heightmapGeneratorScript = heightmapGeneratorInstance.GetComponent<HeightmapGenerator>();
        heightmapGeneratorScript.Initialize((int)Mathf.Pow(2, heightmapBaseN) + 1);

        // Initialize MeshGenerator prefab, create instance, and attach scriptb
        GameObject meshGeneratorPrefab = (GameObject)Resources.Load("Prefabs/MeshGenerator");

        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= initialBlockRadius; i++, sequenceLength += 2)
        {
            // Iterates through each shell sequence
            for(int j = 0; j < 4; j++)
            {
                // Iterates through shell sequence lengths
                for (int k = 0; k < sequenceLength; k++)
                {
                    int xIndex = 0, zIndex = 0;

                    if (j == 0)  // Top-left shell
                    {
                        xIndex = -i + k;
                        zIndex = i - 1;
                    }
                    else if (j == 1)  // Top-right shell
                    {
                        xIndex = i - 1;
                        zIndex = i - k - 1;
                    }
                    else if (j == 2)  // Bottom-right shell
                    {
                        xIndex = i - k - 1;
                        zIndex = -i;
                    }
                    else if (j == 3)    // Bottom-left shell
                    {
                        xIndex = -i;
                        zIndex = -i + k;
                    }

                    // Generate instance of mesh generator prefab
                    GameObject meshGeneratorPrefabInstance = (GameObject)GameObject.Instantiate(meshGeneratorPrefab, GetWorldCoordinates(xIndex, zIndex), Quaternion.identity);

                    // Generate mesh for instance
                    MeshGenerator meshGeneratorScript = meshGeneratorPrefabInstance.GetComponent<MeshGenerator>();
                    meshGeneratorScript.GenerateMesh(heightmapGeneratorScript.GenerateHeightmap(xIndex, zIndex), blockSize);
                }
            }
        }
    }

    private Vector3 GetWorldCoordinates(float xIndex, float zIndex)
    {
        return new Vector3(xIndex * blockSize, 0, zIndex * blockSize);
    }
}
