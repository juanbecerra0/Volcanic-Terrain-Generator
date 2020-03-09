using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class MeshPlacer : MonoBehaviour
{
    public int initialBlockRadius = 2;  // The initial radius from the origin of the scene
    public int heightmapBaseN = 7;      // The dimensions of generated heightmaps (2^(n)+1)
    public int blockSize = 100;         // The dimensions of a MeshGen object

    // Used in applying materials and shaders to all terrain
    private GameObject masterTerrainPrefab;
    private GameObject masterTerrainInstance;
    private MasterTerrain masterTerrainScript;

    // Used in generating heightmaps -> generating geometry
    private GameObject heightmapGeneratorPrefab;
    private GameObject heightmapGeneratorInstance;
    private HeightmapGenerator heightmapGeneratorScript;

    // Prefab that all terrain block objects instantiate from
    private GameObject meshGeneratorPrefab;

    private bool CheckErrors()
    {
        if (initialBlockRadius < 1 || heightmapBaseN < 3 || blockSize < 1)
            return false;
        else
            return true;
    }

    // Start is called before the first frame update
    // Initialize the map with several meshes based on input radius
    void Start()
    {
        // Error checking
        if (!CheckErrors())
            Debug.Log("Mesh Placer Error!");

        // Initialize MasterTerrain prefab and create instance, and attach script
        masterTerrainPrefab = (GameObject)Resources.Load("Prefabs/MasterTerrain");
        masterTerrainInstance = (GameObject)GameObject.Instantiate(masterTerrainPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        masterTerrainScript = masterTerrainInstance.GetComponent<MasterTerrain>();

        // Initialize HeightmapGenerator prefab, create instance, and attatch script
        heightmapGeneratorPrefab = (GameObject)Resources.Load("Prefabs/HeightmapGenerator");
        heightmapGeneratorInstance = (GameObject)GameObject.Instantiate(heightmapGeneratorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        heightmapGeneratorScript = heightmapGeneratorInstance.GetComponent<HeightmapGenerator>();
        heightmapGeneratorScript.Initialize((int)Mathf.Pow(2, heightmapBaseN) + 1);

        // Initialize MeshGenerator prefab
        meshGeneratorPrefab = (GameObject)Resources.Load("Prefabs/MeshGenerator");

        // Initiate the initial shell sequence
        DoShellSequence();
    }

    private void DoShellSequence()
    {
        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= initialBlockRadius; i++, sequenceLength += 2)
        {
            // Iterates through each shell sequence
            for (int j = 0; j < 4; j++)
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

                    // Generate block instance
                    GenerateBlockInstance(xIndex, zIndex);
                }
            }
        }
    }

    private void GenerateBlockInstance(int xIndex, int zIndex)
    {
        // Generate instance of mesh generator prefab
        GameObject meshGeneratorPrefabInstance = (GameObject)GameObject.Instantiate(meshGeneratorPrefab, GetWorldCoordinates(xIndex, zIndex), Quaternion.identity);

        // Generate mesh for instance
        MeshGenerator meshGeneratorScript = meshGeneratorPrefabInstance.GetComponent<MeshGenerator>();
        meshGeneratorScript.GenerateMesh(heightmapGeneratorScript.GenerateHeightmap(xIndex, zIndex), blockSize);

        // Add mesh generator instance transform as child object to master terrain transform
        // Then, update this child's material/shader
        meshGeneratorPrefabInstance.transform.parent = masterTerrainInstance.transform;
    }

    private Vector3 GetWorldCoordinates(float xIndex, float zIndex)
    {
        return new Vector3(xIndex * blockSize, 0, zIndex * blockSize);
    }
}
