using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    private int textureResolution;
    private float heightmapTextureRatio;
    private float grassMountainThres;
    private float mountainSnowThres;

    public void Initialize(int textureResolution, int heightmapResolution)
    {
        this.textureResolution = textureResolution;
        this.heightmapTextureRatio = (float)((float)heightmapResolution / (float)textureResolution);
        this.grassMountainThres = 10.0f;
        this.mountainSnowThres = 20.0f;
    }

    public Texture2D GenerateTexture(float[,] heightmap)
    {
        Texture2D texture = new Texture2D(textureResolution, textureResolution);

        for(int i = 0; i < textureResolution; i++)
        {
            for(int j = 0; j < textureResolution; j++)
            {
                // Get delta average of heightmap, then set corresponding color
                Tuple<int, int> heightmapIndex = GetHeightmapIndex(new Tuple<int, int>(i, j));
                texture.SetPixel(i, j, GetColor(heightmap[heightmapIndex.Item1, heightmapIndex.Item2]));
            }
        }

        texture.Apply();
        return texture;
    }

    private Tuple<int, int> GetHeightmapIndex(Tuple<int, int> textureIndex)
    {
        int heightmapXIndex;
        int heightmapYIndex;

        float approxHeightmapXIndex = (textureIndex.Item1 - 1) * heightmapTextureRatio;
        float approxHeightmapYIndex = (textureIndex.Item2 - 1) * heightmapTextureRatio;

        // Determine x
        if (approxHeightmapXIndex - Mathf.FloorToInt(approxHeightmapXIndex) <= 0.5)
            heightmapXIndex = Mathf.FloorToInt(approxHeightmapXIndex);
        else
            heightmapXIndex = Mathf.CeilToInt(approxHeightmapXIndex);

        // Determine y
        if (approxHeightmapYIndex - Mathf.FloorToInt(approxHeightmapYIndex) <= 0.5)
            heightmapYIndex = Mathf.FloorToInt(approxHeightmapYIndex);
        else
            heightmapYIndex = Mathf.CeilToInt(approxHeightmapYIndex);

        return new Tuple<int, int>(
            heightmapXIndex,
            heightmapYIndex
        );
    }

    private Color GetColor(float height)
    {
        if (height <= grassMountainThres)
            // Grass
            return Color.green;
        else if (height <= mountainSnowThres)
            // Mountain
            return Color.gray;
        else
            // Snow
            return Color.white;
    }
}
