using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    private int textureResolution;
    private int heightmapResolution;
    private float heightmapTextureRatio;

    // Colors
    private float grassMountainThres = 13.0f;
    private float mountainSnowThres = 20.0f;

    private Color grassColor = new Color(0.255f, 0.573f, 0.294f);
    private Color mountainColor = new Color(0.333f, 0.267f, 0.200f);
    private Color snowColor = new Color(0.900f, 0.900f, 0.900f);

    public void Initialize(int textureResolution, int heightmapResolution)
    {
        this.textureResolution = textureResolution;
        this.heightmapResolution = heightmapResolution;
        this.heightmapTextureRatio = (float)((float)heightmapResolution / (float)textureResolution);
    }

    public Texture2D GenerateTexture(float[,] heightmap)
    {
        Texture2D texture = new Texture2D(textureResolution, textureResolution);

        Tuple<int, int> GetHeightmapIndex(Tuple<int, int> textureIndex)
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

        Color GetColor(Tuple<int, int> heightmapIndex)
        {
            // Get actual value
            float height = heightmap[heightmapIndex.Item1, heightmapIndex.Item2];

            // Top
            if (heightmapIndex.Item1 - 1 < 0)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 - 1, heightmapIndex.Item2];

            // Top-right
            if (heightmapIndex.Item1 - 1 < 0 || heightmapIndex.Item2 + 1 >= heightmapResolution)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 - 1, heightmapIndex.Item2 + 1];

            // Right
            if (heightmapIndex.Item2 + 1 >= heightmapResolution)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2 + 1];

            // Bottom-right
            if (heightmapIndex.Item1 + 1 >= heightmapResolution || heightmapIndex.Item2 + 1 >= heightmapResolution)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 + 1, heightmapIndex.Item2 + 1];

            // Bottom
            if (heightmapIndex.Item1 + 1 >= heightmapResolution)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 + 1, heightmapIndex.Item2];

            // Bottom-left
            if (heightmapIndex.Item1 + 1 >= heightmapResolution || heightmapIndex.Item2 - 1 < 0)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 + 1, heightmapIndex.Item2 - 1];

            // Left
            if (heightmapIndex.Item2 - 1 < 0)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2 - 1];

            // Top-left
            if (heightmapIndex.Item1 - 1 < 0 || heightmapIndex.Item2 - 1 < 0)
                height += heightmap[heightmapIndex.Item1, heightmapIndex.Item2];
            else
                height += heightmap[heightmapIndex.Item1 - 1, heightmapIndex.Item2 - 1];

            // Calc average
            height /= 9;

            if (height <= grassMountainThres)
                // Grass
                return grassColor;
            else if (height <= mountainSnowThres)
                // Mountain
                return mountainColor;
            else
                // Snow
                return snowColor;
        }

        for (int i = 0; i < textureResolution; i++)
        {
            for(int j = 0; j < textureResolution; j++)
            {
                // Get delta average of heightmap, then set corresponding color
                Tuple<int, int> heightmapIndex = GetHeightmapIndex(new Tuple<int, int>(i, j));
                texture.SetPixel(i, j, GetColor(heightmapIndex));
            }
        }

        texture.Apply();
        return texture;
    }
}
