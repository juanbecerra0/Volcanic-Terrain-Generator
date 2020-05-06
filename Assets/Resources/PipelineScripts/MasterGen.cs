using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Initializes the terrain engine, updates new terrain block generation
public class MasterGen : MonoBehaviour
{
    // Init variables
    public int block_Radius = 2;
    public int block_VertexWidth = 256;

    public int heightmap_PowerN = 7;
    public float heightmap_WaterBase = 0f;
    public float heightmap_SandBase = 2f;
    public float heightmap_GrassBase = 6f;
    public float heightmap_MountainBase = 15f;
    public float heightmap_SnowBase = 12f;
    public float heightmap_WaterDisp = 0.01f;
    public float heightmap_SandDisp = 0.4f;
    public float heightmap_GrassDisp = 1.4f;
    public float heightmap_MountainDisp = 4.4f;
    public float heightmap_SnowDisp = 0.01f;

    public int biome_HeightmapContentWidth = 10;
    public int biome_Dimensions = 1024;
    public int biome_SeedSpacing = 32;
    public float biome_DisplacementDiv = 2.5f;
    public uint biome_Water = 1;
    public uint biome_Sand = 2;
    public uint biome_Grass = 3;
    public uint biome_Mountain = 4;
    public uint biome_Snow = 5;

    public int material_Resolution = 512;

    private Color texture_WaterColor = new Color(0.004f, 0.467f, 0.745f);
    private Color texture_SandColor = new Color(0.827f, 0.781f, 0.635f);
    private Color texture_GrassColor = new Color(0.255f, 0.573f, 0.294f);
    private Color texture_MountainColor = new Color(0.333f, 0.267f, 0.200f);
    private Color texture_SnowColor = new Color(0.812f, 0.063f, 0.125f);

    // Biome gen
    private GameObject BiomeGenInstance;
    private BiomeGen BiomeGenScript;

    // Map database
    private GameObject MapDatabaseInstance;
    private MapDatabase MapDatabaseScript;

    // Heightmap gen
    private GameObject HeightmapGenInstance;
    private HeightmapGen HeightmapGenScript;

    // Material gen
    private GameObject MaterialGenInstance;
    private MaterialGen MaterialGenScript;

    // Model gen / master terrain
    private GameObject ModelGenPrefab;
    private GameObject MasterTerrainInstance;

    // Player controls prefab
    private GameObject PlayerCharacterPrefab;

    private bool CheckErrors() { return (block_Radius < 1 || heightmap_PowerN < 3 || block_VertexWidth < 1 || material_Resolution < 64); }

    private void InitPrefabsAndScripts()
    {
        // Biome/color tuple
        Tuple<uint, uint, uint, uint, uint> BiomeTuple = new Tuple<uint, uint, uint, uint, uint>(biome_Water, biome_Sand, biome_Grass, biome_Mountain, biome_Snow);
        Tuple<Color, Color, Color, Color, Color> ColorTuple = new Tuple<Color, Color, Color, Color, Color>(texture_WaterColor, texture_SandColor, texture_GrassColor, texture_MountainColor, texture_SnowColor);

        // Heightmap tuples
        Tuple<float, float, float, float, float> heightmapBasesTuple = new Tuple<float, float, float, float, float>(heightmap_WaterBase, heightmap_SandBase, heightmap_GrassBase, heightmap_MountainBase, heightmap_SnowBase);
        Tuple<float, float, float, float, float> heightmapDispTuple = new Tuple<float, float, float, float, float>(heightmap_WaterDisp, heightmap_SandDisp, heightmap_GrassDisp, heightmap_MountainDisp, heightmap_SnowDisp);

        // Biome gen
        GameObject BiomeGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/BiomeGenPrefab");
        BiomeGenInstance = (GameObject)GameObject.Instantiate(BiomeGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        BiomeGenScript = BiomeGenInstance.GetComponent<BiomeGen>();
        BiomeGenScript.Init(biome_Dimensions, biome_SeedSpacing, biome_DisplacementDiv, BiomeTuple);

        // Map database
        GameObject MapDatabasePrefab = (GameObject)Resources.Load("PipelinePrefabs/MapDatabasePrefab");
        MapDatabaseInstance = (GameObject)GameObject.Instantiate(MapDatabasePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MapDatabaseScript = MapDatabaseInstance.GetComponent<MapDatabase>();
        MapDatabaseScript.Init(heightmap_PowerN, biome_Dimensions, biome_HeightmapContentWidth);

        // Heightmap gen
        GameObject HeightmapGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/HeightmapGenPrefab");
        HeightmapGenInstance = (GameObject)GameObject.Instantiate(HeightmapGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenScript = HeightmapGenInstance.GetComponent<HeightmapGen>();
        HeightmapGenScript.Init(heightmap_PowerN, BiomeTuple, heightmapBasesTuple, heightmapDispTuple);

        // Material gen
        GameObject MaterialGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/MaterialGenPrefab");
        MaterialGenInstance = (GameObject)GameObject.Instantiate(MaterialGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MaterialGenScript = MaterialGenInstance.GetComponent<MaterialGen>();
        MaterialGenScript.InitTexture(material_Resolution, heightmap_PowerN, BiomeTuple, ColorTuple);

        // Model gen / master terrain
        ModelGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/ModelGenPrefab");
        GameObject MasterTerrainPrefab = (GameObject) Resources.Load("PipelinePrefabs/MasterTerrainPrefab");
        MasterTerrainInstance = (GameObject)GameObject.Instantiate(MasterTerrainPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // Player character
        PlayerCharacterPrefab = (GameObject)Resources.Load("PipelinePrefabs/PlayerCharacterPrefab");
    }

    private void InitPlayerCharacter()
    {
        GameObject PlayerCharacterInstance = (GameObject)GameObject.Instantiate(PlayerCharacterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
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

        // Activate player controls
        InitPlayerCharacter();
    }

    private void DoShellSequence()
    {
        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= block_Radius; i++, sequenceLength += 2)
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
        Vector3 GetWorldCoordinates(float xIndex, float zIndex) { return new Vector3(xIndex * block_VertexWidth, 0, zIndex * block_VertexWidth); }

        if (!MapDatabaseScript.IsVacent(x, z))
        {
            Debug.Log("Tried to generate block at (" + x + ", " + z + "). Aborted.");
            return;
        }

        // Generate biome and sub-biome, and get the gradient
        MapDatabaseScript.GeneratePossibleBiome(x, z);
        Tuple<uint[,], float[,]> subBiomeGradient = MapDatabaseScript.GetSubBiome(x, z);

        // Generate heightmap
        float[,] Heightmap = HeightmapGenScript.GenerateHeightmap(x, z, subBiomeGradient.Item1, subBiomeGradient.Item2);

        // Generate material
        Texture2D Texture = MaterialGenScript.GenerateTexture(Heightmap, subBiomeGradient.Item1);
        // TODO generate bumpmap and misc.

        // Generate model
        GameObject ModelGenInstance = (GameObject)GameObject.Instantiate(ModelGenPrefab, GetWorldCoordinates(x, z), Quaternion.identity);
        ModelGen ModelGenScript = ModelGenInstance.GetComponent<ModelGen>();
        ModelGenScript.GenerateMesh(Heightmap, Texture, block_VertexWidth);

        // Merge model to master mesh
        ModelGenInstance.transform.parent = MasterTerrainInstance.transform;
    }
}
