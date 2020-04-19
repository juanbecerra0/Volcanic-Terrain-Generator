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
    public float heightmap_CornerInitMin = 0.0f;
    public float heightmap_CornerInitMax = 1.0f;
    public float heightmap_DisplacementMin = -0.5f;
    public float heightmap_DisplacementMax = 5.0f;

    public int biome_HeightmapContentWidth = 10;

    public int biome_Dimensions = 1024;
    public int biome_SeedSpacing = 32;
    public uint biome_Water = 1;
    public uint biome_Sand = 2;
    public uint biome_Grass = 3;
    public uint biome_Mountain = 4;
    public uint biome_Snow = 5;

    public int material_Resolution = 512;

    public float texture_grassMountainThres = 13.0f;
    public float texture_mountainSnowThres = 20.0f;
    private Color texture_DarkGrassColor = new Color(0.255f / 2, 0.573f / 2, 0.294f / 2);
    private Color texture_GrassColor = new Color(0.255f, 0.573f, 0.294f);
    private Color texture_MountainColor = new Color(0.333f, 0.267f, 0.200f);
    private Color texture_SnowColor = new Color(0.900f, 0.900f, 0.900f);

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

    // Player controls prefab
    private GameObject PlayerCharacterPrefab;

    private bool CheckErrors() { return (block_Radius < 1 || heightmap_PowerN < 3 || block_VertexWidth < 1 || material_Resolution < 64); }

    private void InitPrefabsAndScripts()
    {
        // Map database
        GameObject MapDatabasePrefab = (GameObject)Resources.Load("PipelinePrefabs/MapDatabasePrefab");
        MapDatabaseInstance = (GameObject)GameObject.Instantiate(MapDatabasePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MapDatabaseScript = MapDatabaseInstance.GetComponent<MapDatabase>();
        MapDatabaseScript.Init(biome_HeightmapContentWidth);

        // Heightmap gen
        GameObject HeightmapGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/HeightmapGenPrefab");
        HeightmapGenInstance = (GameObject)GameObject.Instantiate(HeightmapGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        HeightmapGenScript = HeightmapGenInstance.GetComponent<HeightmapGen>();
        HeightmapGenScript.Init(heightmap_PowerN, heightmap_CornerInitMin, heightmap_CornerInitMax, heightmap_DisplacementMin, heightmap_DisplacementMax);

        // Biome gen
        GameObject BiomeGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/BiomeGenPrefab");
        BiomeGenInstance = (GameObject)GameObject.Instantiate(BiomeGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        BiomeGenScript = BiomeGenInstance.GetComponent<BiomeGen>();
        BiomeGenScript.Init(biome_Dimensions, biome_SeedSpacing, biome_Water, biome_Sand, biome_Grass, biome_Mountain, biome_Snow);

        // TODO test
        //BiomeGenScript.WriteAsTextFile(BiomeGenScript.GenerateBiome(), "C:/Users/becer/Downloads/test.txt");
        uint[,] TL = BiomeGenScript.GenerateBiome(null, null, null, null);
        uint[,] TR = BiomeGenScript.GenerateBiome(null, null, null, TL);
        uint[,] BR = BiomeGenScript.GenerateBiome(TR, null, null, null);
        uint[,] BL = BiomeGenScript.GenerateBiome(TL, BR, null, null);
        BiomeGenScript.WriteAsTexture(TL, "C:/Users/becer/Downloads/TL.PNG");
        BiomeGenScript.WriteAsTexture(TR, "C:/Users/becer/Downloads/TR.PNG");
        BiomeGenScript.WriteAsTexture(BR, "C:/Users/becer/Downloads/BR.PNG");
        BiomeGenScript.WriteAsTexture(BL, "C:/Users/becer/Downloads/BL.PNG");
        BiomeGenScript.WriteAsTextFile(TL, "C:/Users/becer/Downloads/TL.TXT");
        BiomeGenScript.WriteAsTextFile(TR, "C:/Users/becer/Downloads/TR.TXT");
        BiomeGenScript.WriteAsTextFile(BR, "C:/Users/becer/Downloads/BR.TXT");
        BiomeGenScript.WriteAsTextFile(BL, "C:/Users/becer/Downloads/BL.TXT");

        // Material gen
        GameObject MaterialGenPrefab = (GameObject)Resources.Load("PipelinePrefabs/MaterialGenPrefab");
        MaterialGenInstance = (GameObject)GameObject.Instantiate(MaterialGenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        MaterialGenScript = MaterialGenInstance.GetComponent<MaterialGen>();
        MaterialGenScript.InitTexture(material_Resolution, heightmap_PowerN, texture_grassMountainThres, texture_mountainSnowThres,
            texture_DarkGrassColor, texture_GrassColor, texture_MountainColor, texture_SnowColor);

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

        // Generate biome/heightmap
        float[,] Heightmap = HeightmapGenScript.GenerateHeightmap(x, z);

        // Generate material
        Texture2D Texture = MaterialGenScript.GenerateTexture(Heightmap);
        // TODO generate bumpmap and misc.

        // Generate model
        GameObject ModelGenInstance = (GameObject)GameObject.Instantiate(ModelGenPrefab, GetWorldCoordinates(x, z), Quaternion.identity);
        ModelGen ModelGenScript = ModelGenInstance.GetComponent<ModelGen>();
        ModelGenScript.GenerateMesh(Heightmap, Texture, block_VertexWidth);

        // Merge model to master mesh
        ModelGenInstance.transform.parent = MasterTerrainInstance.transform;
    }
}
