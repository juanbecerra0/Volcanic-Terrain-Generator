using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Initializes the terrain engine, updates new terrain block generation
public class Initializer : MonoBehaviour
{
    // Init variables
    public int init_BlockRadius = 2;
    public int block_VertexWidth = 256;
    public int heightmap_PowerN = 7;
    public int material_Resolution = 512;

    // Scripts
    private MapDatabase mapScript;
    private BiomeGen biomeScript;
    private HeightmapGen heightmapScript;
    private MaterialGen materialScript;
    private ModelGen modelScript;

    private bool CheckErrors() { return (init_BlockRadius < 1 || heightmap_PowerN < 3 || block_VertexWidth < 1 || material_Resolution < 64); }

    private void InitScripts()
    {
        mapScript = GameObject.FindObjectOfType(typeof(MapDatabase)) as MapDatabase;
        biomeScript = GameObject.FindObjectOfType(typeof(BiomeGen)) as BiomeGen;
        heightmapScript = GameObject.FindObjectOfType(typeof(HeightmapGen)) as HeightmapGen;
        materialScript = GameObject.FindObjectOfType(typeof(MaterialGen)) as MaterialGen;
        modelScript = GameObject.FindObjectOfType(typeof(ModelGen)) as ModelGen;
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
        InitScripts();

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

    public void GenerateBlockInstance(int xIndex, int zIndex)
    {
        /*
        // Generate instance of mesh generator prefab
        GameObject meshGeneratorPrefabInstance = (GameObject)GameObject.Instantiate(
            meshGeneratorPrefab,
            GetWorldCoordinates(xIndex, zIndex),
            Quaternion.identity
        );

        // Generate heightmap and texture for mesh
        float[,] heightmap = heightmapGeneratorScript.GenerateHeightmap(xIndex, zIndex);
        Texture2D texture = textureGeneratorScript.GenerateTexture(heightmap);

        MeshGenerator meshGeneratorScript = meshGeneratorPrefabInstance.GetComponent<MeshGenerator>();
        meshGeneratorScript.GenerateMesh(
            heightmap,
            texture,
            blockSize
        );

        // Add mesh generator instance transform as child object to master terrain transform
        meshGeneratorPrefabInstance.transform.parent = masterTerrainInstance.transform;
        */
    }

    private Vector3 GetWorldCoordinates(float xIndex, float zIndex)
    {
        return new Vector3(xIndex * block_VertexWidth, 0, zIndex * block_VertexWidth);
    }
}
