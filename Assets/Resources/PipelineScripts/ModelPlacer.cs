using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPlacer : MonoBehaviour
{
    private GameObject WaterModel;

    private float WaterOffset;
    private float WaterHeight;

    public void Init(int blockVertexWidth, int heightmapContentWidth, float waterHeight)
    {
        WaterModel = (GameObject)Resources.Load("PipelinePrefabs/Models/WaterModel");
        WaterModel.transform.localScale = new Vector3(blockVertexWidth * 2, blockVertexWidth / 30, blockVertexWidth * 2);

        WaterOffset = blockVertexWidth * (heightmapContentWidth / 2);
        WaterHeight = waterHeight;
    }

    public void PlaceWater(Vector3 position)
    {
        Vector3 adjustedPosition = new Vector3(position.x + WaterOffset, WaterHeight, position.z + WaterOffset);
        GameObject WaterInstance = GameObject.Instantiate(WaterModel, adjustedPosition, Quaternion.identity);

        WaterInstance.transform.parent = this.transform;
    }
}
