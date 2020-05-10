using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer : MonoBehaviour
{
    private GameObject WaterModel;
    private GameObject[] CloudModels;
    private GameObject PalmTreeModel;
    private GameObject FirTreeModel;
    private GameObject[] RockModels;

    private float WaterOffset;
    private float WaterHeight;

    private float cloudHeight;
    private float cloudRadius;

    private float BlockVertexWidth;
    private int BlockVertexCount;
    private uint SAND, GRASS, MOUNTAIN;
    private int ModelsPerBlock;
    private float ModelPlacementChance;

    public void Init(int heightmapBaseN, int blockVertexWidth, int heightmapContentWidth, float waterHeight, float volcanoClamp, Tuple<uint, uint, uint, uint, uint> biomeTuple, int modelsPerBlock, float modelPlacementChance)
    {
        BlockVertexWidth = blockVertexWidth;
        BlockVertexCount = (int)Mathf.Pow(2, heightmapBaseN) + 1;
        SAND = biomeTuple.Item2;
        GRASS = biomeTuple.Item3;
        MOUNTAIN = biomeTuple.Item4;

        ModelsPerBlock = modelsPerBlock;
        ModelPlacementChance = modelPlacementChance;

        // Water
        WaterModel = (GameObject)Resources.Load("PipelinePrefabs/Models/WaterModel");
        WaterModel.transform.localScale = new Vector3(blockVertexWidth * 2, blockVertexWidth / 30, blockVertexWidth * 2);

        WaterOffset = blockVertexWidth * (heightmapContentWidth / 2);
        WaterHeight = waterHeight;

        // Clouds
        CloudModels = new GameObject[] {
            (GameObject)Resources.Load("ExternalAssets/LowPolyClouds/Models/RG_Cloud-01"),
            (GameObject)Resources.Load("ExternalAssets/LowPolyClouds/Models/RG_Cloud-02"),
            (GameObject)Resources.Load("ExternalAssets/LowPolyClouds/Models/RG_Cloud-03"),
            (GameObject)Resources.Load("ExternalAssets/LowPolyClouds/Models/RG_Cloud-04"),
            (GameObject)Resources.Load("ExternalAssets/LowPolyClouds/Models/RG_Cloud-05")
        };

        cloudHeight = volcanoClamp * 1.2f;
        cloudRadius = blockVertexWidth * 1.75f;
        Vector3 cloudScale = new Vector3(blockVertexWidth / 24f, blockVertexWidth / 24f, blockVertexWidth / 24f);

        foreach (GameObject c in CloudModels)
        {
            c.transform.localScale = cloudScale;
        }

        // Palm trees
        PalmTreeModel = (GameObject)Resources.Load("ExternalAssets/Trees/Prefabs/Palm_Tree");
        PalmTreeModel.transform.localScale = new Vector3(14f, 14f, 14f);

        // Fir trees
        FirTreeModel = (GameObject)Resources.Load("ExternalAssets/Trees/Prefabs/Fir_Tree");
        FirTreeModel.transform.localScale = new Vector3(14f, 14f, 14f);

        // Rocks
        RockModels = new GameObject[]
        {
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock1"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock2"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock3"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock4"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock5"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock6"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock7"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock8"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock9"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock10"),
            (GameObject)Resources.Load("ExternalAssets/LowPoly Rocks/Prefabs/Rock11")
        };

        foreach (GameObject r in RockModels)
        {
            r.transform.localScale = new Vector3(20f, 20f, 20f);
        }
    }

    public void PlaceWater(Vector3 position)
    {
        Vector3 adjustedPosition = new Vector3(position.x + WaterOffset, WaterHeight, position.z + WaterOffset);
        GameObject WaterInstance = GameObject.Instantiate(WaterModel, adjustedPosition, Quaternion.identity);

        WaterInstance.transform.parent = transform;
    }

    public void PlaceClouds(Vector3 position)
    {
        System.Random rand = new System.Random();
        Vector3 center = new Vector3(position.x, cloudHeight, position.z);
        Vector3 start = new Vector3(center.x - cloudRadius, center.y, center.z);
        Vector3 end = new Vector3(center.x + cloudRadius, center.y, center.z);

        for (float o = 0f; o < 2.0f; o += 0.25f)
        {
            Vector3 circlePosition = Vector3.SlerpUnclamped(start - center, end - center, o) + center;

            GameObject CloudInstance = GameObject.Instantiate(CloudModels[rand.Next(0, CloudModels.Length)], circlePosition, Quaternion.Euler(new Vector3(0f, -360f * (o / 2), 0f)));
            CloudInstance.transform.parent = transform;
        }
    }

    public void PlacePossibleModels(float[,] heightmap, uint[,] subBiome, Vector3 topLeftCorner)
    {
        System.Random rand = new System.Random();
        int modelCount = rand.Next(0, 4);   // Choose between 0 and 3 models to place on this block

        // Choose modelCount positions on the subBiome to add models
        int selected = 0, i = 0, j = 0;
        while (selected < modelCount && !(i == BlockVertexCount - 1 && j == BlockVertexCount - 1))
        {
            // increment indicies
            if(j == BlockVertexCount - 1)
            {
                j = 0;
                i++;
            } else
            {
                j++;
            }

            if (UnityEngine.Random.Range(0f, 1f) < ModelPlacementChance)
            {
                selected++;
                Vector3 position = new Vector3(topLeftCorner.x + (((float)j / BlockVertexCount) * BlockVertexWidth), heightmap[i, j] - 20f, topLeftCorner.z - (((float)i / BlockVertexCount) * BlockVertexWidth));

                if (subBiome[i, j] == SAND && heightmap[i, j] > WaterHeight)
                {
                    GameObject model = GameObject.Instantiate(PalmTreeModel, position, Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360f), 0f));
                    model.transform.parent = transform;
                } else if (subBiome[i, j] == GRASS)
                {
                    GameObject model = GameObject.Instantiate(FirTreeModel, position, Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360f), 0f));
                    model.transform.parent = transform;
                } else if (subBiome[i, j] == MOUNTAIN)
                {
                    GameObject model = GameObject.Instantiate(RockModels[rand.Next(0, 11)], position, Quaternion.Euler(0f, UnityEngine.Random.Range(0, 360f), 0f));
                    model.transform.parent = transform;
                } else
                {
                    selected--;
                }
            }
        }
    }
}
