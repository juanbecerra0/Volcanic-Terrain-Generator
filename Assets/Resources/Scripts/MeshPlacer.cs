using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class MeshPlacer : MonoBehaviour
{
    public int vertexCount = 20;        // Verticies in a MeshGen object
    public int blockSize = 100;         // The dimensions of a MeshGen object
    public int initialBlockRadius = 2;  // The initial radius from the origin of the scene
    public int heightmapBaseN = 7;      // The dimensions of generated heightmaps (2^(n)+1)

    // Start is called before the first frame update
    // Initialize the map with several meshes based on input radius
    void Start()
    {
        // Initialize HeightmapGenerator prefab, create instance, and attach init script
        GameObject heightmapGeneratorPrefab = (GameObject)Resources.Load("Prefabs/HeightmapGenerator");
        GameObject heightmapGeneratorInstance = (GameObject)GameObject.Instantiate(heightmapGeneratorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenerator hmScript = heightmapGeneratorInstance.GetComponent<HeightmapGenerator>();
        hmScript.Initialize((int)Mathf.Pow(2, heightmapBaseN) + 1);

        // Initialize MeshGenerator prefab
        GameObject prefab = (GameObject) Resources.Load("Prefabs/MeshGenerator");

        // Error checking
        if (vertexCount < 2)
            vertexCount = 2;

        if (initialBlockRadius < 1)
            initialBlockRadius = 1;

        if (heightmapBaseN < 3)
            heightmapBaseN = 3;

        if (!prefab)
        {
            Debug.Log("Prefab could not be found");
            return;
        }

        Quaternion orientation = Quaternion.identity;

        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= initialBlockRadius; i++, sequenceLength += 2)
        {
            // Iterates through each shell sequence
            for(int j = 0; j < 4; j++)
            {
                // Iterates through shell sequence lengths
                for (int k = 0; k < sequenceLength; k++)
                {
                    float xLocation = 0, zLocation = 0;
                    int xIndex = 0, zIndex = 0;

                    if (j == 0)  // Top-left shell
                    {
                        xLocation = -(blockSize * ((sequenceLength / 2) + 1)) + (k * blockSize);
                        zLocation =  (blockSize * ((sequenceLength / 2) + 1)) - (blockSize);
                        xIndex = -i + k;
                        zIndex =  i;
                    }
                    else if (j == 1)  // Top-right shell
                    {
                        xLocation = (blockSize * ((sequenceLength / 2) + 1)) - (blockSize);
                        zLocation = (blockSize * ((sequenceLength / 2) + 1)) - (k * blockSize) - (blockSize);
                        xIndex = i - 1;
                        zIndex = i - k;
                    }
                    else if (j == 2)  // Bottom-right shell
                    {
                        xLocation = (blockSize * ((sequenceLength / 2) + 1)) - (k * blockSize) - (blockSize);
                        zLocation = -(blockSize * ((sequenceLength / 2) + 1));
                        xIndex =  i - 1 - k;
                        zIndex = -i + 1;
                    }
                    else if (j == 3)    // Bottom-left shell
                    {
                        xLocation = -(blockSize * ((sequenceLength / 2) + 1));
                        zLocation = -(blockSize * ((sequenceLength / 2) + 1)) + (k * blockSize);
                        xIndex = -i;
                        zIndex = -i + 1 + k;
                    }

                    // Calculate location
                    Vector3 position = new Vector3(xLocation, 0, zLocation);

                    // Generate instance of prefab
                    GameObject prefabInstance = (GameObject)GameObject.Instantiate(prefab, position, orientation);

                    // Generate mesh for instance
                    MeshGenerator script = prefabInstance.GetComponent<MeshGenerator>();
                    script.GenerateMesh(vertexCount, blockSize, hmScript.GenerateHeightmap(xIndex, zIndex));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
