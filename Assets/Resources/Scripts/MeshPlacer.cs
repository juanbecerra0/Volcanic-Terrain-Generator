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
<<<<<<< Updated upstream
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

=======
        // Error checking
>>>>>>> Stashed changes
        if (initialBlockRadius < 1)
            initialBlockRadius = 1;

        if (heightmapBaseN < 3)
            heightmapBaseN = 3;

        if (!prefab)
        {
            Debug.Log("Prefab could not be found");
            return;
        }

        // Initialize HeightmapGenerator prefab, create instance, and attatch script
        GameObject heightmapGeneratorPrefab = (GameObject)Resources.Load("Prefabs/HeightmapGenerator");
        GameObject heightmapGeneratorInstance = (GameObject)GameObject.Instantiate(heightmapGeneratorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenerator heightmapGeneratorScript = heightmapGeneratorInstance.GetComponent<HeightmapGenerator>();
        heightmapGeneratorScript.Initialize((int)Mathf.Pow(2, heightmapBaseN) + 1);

        // Initialize MeshGenerator prefab, create instance, and attach scriptb
        GameObject meshPrefab = (GameObject)Resources.Load("Prefabs/MeshGenerator");
        GameObject meshPrefabInstance = (GameObject)GameObject.Instantiate(meshPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MeshGenerator meshScript = meshPrefabInstance.GetComponent<MeshGenerator>();

        // Initialize master mesh prefab, create instance, and attatch script
        GameObject masterTerrainPrefab = (GameObject)Resources.Load("Prefabs/MasterTerrain");
        GameObject masterTerrainInstance = (GameObject)GameObject.Instantiate(masterTerrainPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MasterTerrain masterTerrainScript = masterTerrainInstance.GetComponent<MasterTerrain>();
        masterTerrainScript.Initialize();

        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= initialBlockRadius; i++, sequenceLength += 2)
        {
            // Iterates through each shell sequence
            for(int j = 0; j < 2; j++)
            {
                // Iterates through shell sequence lengths
                for (int k = 0; k < sequenceLength; k++)
                {
                    int xIndex = 0, zIndex = 0;

                    if (j == 0)  // Top-left shell
                    {
                        xIndex = -i + k;
                        zIndex =  i - 1;
                    }
                    else if (j == 1)  // Top-right shell
                    {
                        xIndex = i - 1;
                        zIndex = i - k - 1;
                    }
                    else if (j == 2)  // Bottom-right shell
                    {
                        xIndex =  i - k - 1;
                        zIndex = -i;
                    }
                    else if (j == 3)    // Bottom-left shell
                    {
                        xIndex = -i;
                        zIndex = -i + k;
                    }

                    // Calculate location
                    Vector3 position = GetWorldCoordinates(xIndex, zIndex);

                    // Generate heightmap
                    float[,] heightmap = heightmapGeneratorScript.GenerateHeightmap(xIndex, zIndex);

                    // Generate mesh for instance
<<<<<<< Updated upstream
                    MeshGenerator script = prefabInstance.GetComponent<MeshGenerator>();
                    script.GenerateMesh(vertexCount, blockSize, hmScript.GenerateHeightmap(xIndex, zIndex));
=======
                    Mesh mesh = meshScript.GenerateMesh(heightmap, blockSize, position);

                    // Send mesh to master terrain component
                    masterTerrainScript.EnqueueMesh(mesh);
>>>>>>> Stashed changes
                }
            }
        }
    }

    private Vector3 GetWorldCoordinates(float xIndex, float zIndex)
    {
        return new Vector3(xIndex * blockSize, 0, zIndex * blockSize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
