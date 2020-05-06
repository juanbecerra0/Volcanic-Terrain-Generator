using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialGen : MonoBehaviour
{
    private int textureResolution;
    private int heightmapResolution;
    private float heightmapTextureRatio;

    // Color index
    private uint WaterIndex;
    private uint SandIndex;
    private uint GrassIndex;
    private uint MountainIndex;
    private uint SnowIndex;

    // Colors
    private Color WaterColor;
    private Color SandColor;
    private Color GrassColor;
    private Color MountainColor;
    private Color SnowColor;

    public void InitTexture(int textureResolution, int heightmapBaseN, Tuple<uint, uint, uint, uint, uint> BiomeTuple, Tuple<Color, Color, Color, Color, Color> ColorTuple)
    {
        this.textureResolution = textureResolution;
        this.heightmapResolution = (int)Mathf.Pow(2, heightmapBaseN) + 1; ;
        this.heightmapTextureRatio = (float)((float)heightmapResolution / (float)textureResolution);

        WaterIndex = BiomeTuple.Item1;
        SandIndex = BiomeTuple.Item2;
        GrassIndex = BiomeTuple.Item3;
        MountainIndex = BiomeTuple.Item4;
        SnowIndex = BiomeTuple.Item5;

        WaterColor = ColorTuple.Item1;
        SandColor = ColorTuple.Item2;
        GrassColor = ColorTuple.Item3;
        MountainColor = ColorTuple.Item4;
        SnowColor = ColorTuple.Item5;
    }

    public Texture2D GenerateTexture(float[,] heightmap, uint[,] biomeMap)
    {
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
            uint index = biomeMap[heightmapIndex.Item1, heightmapIndex.Item2];

            if (index == WaterIndex)
                return WaterColor;
            else if (index == SandIndex)
                return SandColor;
            else if (index == GrassIndex)
                return GrassColor;
            else if (index == MountainIndex)
                return MountainColor;
            else if (index == SnowIndex)
                return SnowColor;
            else
                return Color.red;
        }

        Texture2D texture = new Texture2D(textureResolution, textureResolution);

        for (int i = 0; i < textureResolution; i++)
        {
            for (int j = 0; j < textureResolution; j++)
            {
                // Get delta average of heightmap, then set corresponding color
                Tuple<int, int> heightmapIndex = GetHeightmapIndex(new Tuple<int, int>(i, j));
                texture.SetPixel(j, textureResolution - i, GetColor(heightmapIndex));
            }
        }

        texture.Apply();
        return texture;
    }

}
