using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer : MonoBehaviour
{
    private GameObject WaterModel;

    private float WaterOffset;

    public void Init(int blockVertexWidth, int heightmapContentWidth)
    {
        WaterModel = (GameObject)Resources.Load("PipelinePrefabs/Models/WaterModel");

        WaterOffset = blockVertexWidth * (heightmapContentWidth / 2);
    }

    public void PlaceWater(Vector3 position)
    {
        Vector3 adjustedPosition = new Vector3(position.x + WaterOffset, 0, position.z + WaterOffset);
        GameObject WaterInstance = GameObject.Instantiate(WaterModel, adjustedPosition, Quaternion.identity);

        WaterInstance.transform.parent = this.transform;
    }
}
