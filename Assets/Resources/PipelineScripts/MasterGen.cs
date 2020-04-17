using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Initializes the terrain engine, updates new terrain block generation
public class MasterGen : MonoBehaviour
{
    // Init variables
    public int init_BlockRadius = 2;
    public int block_VertexWidth = 256;
    public int heightmap_PowerN = 7;
    public int material_Resolution = 512;

    // Map database
    private GameObject MapDatabaseInstance;
    private MapDatabase MapDatabaseScript;

    // Heightmap gen
    private GameObject HeightmapGenInstance;
    private HeightmapGen HeightmapGenScript;

    // Biome gen
    private GameObject BiomeGenInstance;
    private BiomeGen BiomeGenScript;

    // Material gen
    private GameObject MaterialGenInstance;
    private MaterialGen MaterialGenScript;

    // Model gen / master terrain
    private GameObject ModelGenPrefab;
    private GameObject MasterTerrainInstance;

    private bool CheckErrors() { return (init_BlockRadius < 1 || heightmap_PowerN < 3 || block_VertexWidth < 1 || material_Resolution < 64); }

    private void InitPrefabsAndScripts()
    {
        // Map database
        GameObject MapDatabasePrefab = (GameObject)Resources.Load("PipelinePrefabs/MapDatabasePrefab");
        MapDatabaseInstance = (GameObject)GameObject.Instantiate(MapDatabasePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MapDatabaseScript = MapDatabaseInstance.GetComponent<MapDatabase>();
        MapDatabaseScript.Init();

        // Heightmap gen
        GameObject HeightmapGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/HeightmapGenPrefab");
        HeightmapGenInstance = (GameObject)GameObject.Instantiate(HeightmapGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenScript = HeightmapGenInstance.GetComponent<HeightmapGen>();

        // Biome gen
        GameObject BiomeGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/BiomeGenPrefab");
        BiomeGenInstance = (GameObject)GameObject.Instantiate(BiomeGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        BiomeGenScript = BiomeGenInstance.GetComponent<BiomeGen>();

        // Material gen
        GameObject MaterialGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/MaterialGenPrefab");
        MaterialGenInstance = (GameObject)GameObject.Instantiate(MaterialGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MaterialGenScript = MaterialGenInstance.GetComponent<MaterialGen>();

        // Model gen / master terrain
        ModelGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/ModelGenPrefab");
        GameObject MasterTerrainPrefab = (GameObject) Resources.Load("PipelinePrefabs/MasterTerrainPrefab");
        MasterTerrainInstance = (GameObject)GameObject.Instantiate(MasterTerrainPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Start is called before the first frame update
    // Initialize the map with several meshes based on input radius
    void Start()
    {
        // Error checking
        if (CheckErrors()) {
            Debug.Log("Variable Error!");
            return;
        }

        // Init scripts
        InitPrefabsAndScripts();

        // Initiate the shell sequence
        DoShellSequence();
    }

    private void DoShellSequence()
    {
        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= init_BlockRadius; i++, sequenceLength += 2)
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

    public void GenerateBlockInstance(int x, int z)
    {
        if (!MapDatabaseScript.IsVacent(x, z))
        {
            Debug.Log("Tried to generate block at (" + x + ", " + z + "). Aborted.");
            return;
        }

        // TODO Generate biome/heightmap

        // TODO Generate material

        // TODO Generate model

        // TODO Merge model to master mesh

    }

    private Vector3 GetWorldCoordinates(float xIndex, float zIndex)
    {
        return new Vector3(xIndex * block_VertexWidth, 0, zIndex * block_VertexWidth);
    }
}
