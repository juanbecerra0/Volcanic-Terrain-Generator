using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer : MonoBehaviour
{
    private GameObject WaterModel;
    private GameObject[] CloudModels;

    private float WaterOffset;
    private float WaterHeight;

    private float cloudHeight;
    private float cloudRadius;

    public void Init(int blockVertexWidth, int heightmapContentWidth, float waterHeight, float volcanoClamp)
    {
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
}
