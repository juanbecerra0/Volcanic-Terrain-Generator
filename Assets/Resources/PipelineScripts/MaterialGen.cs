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

    // Textures
    private Texture2D SandTexture;
    private Texture2D GrassTexture;
    private Texture2D MountainTexture;
    private Texture2D LavaTexture;

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

        Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        SandTexture = duplicateTexture(Resources.Load("PipelinePrefabs/Textures/sand1") as Texture2D);
        GrassTexture = duplicateTexture(Resources.Load("PipelinePrefabs/Textures/grass1") as Texture2D);
        MountainTexture = duplicateTexture(Resources.Load("PipelinePrefabs/Textures/mountain1") as Texture2D);
        LavaTexture = duplicateTexture(Resources.Load("PipelinePrefabs/Textures/lava1") as Texture2D);
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

        Color GetColor(Tuple<int, int> heightmapIndex, int x, int z)
        {
            uint index = biomeMap[heightmapIndex.Item1, heightmapIndex.Item2];

            if (index == WaterIndex)
                return WaterColor;
            else if (index == SandIndex)
                return SandTexture.GetPixel(x, z);
            else if (index == GrassIndex)
                return GrassTexture.GetPixel(x, z);
            else if (index == MountainIndex)
                return MountainTexture.GetPixel(x, z);
            else if (index == SnowIndex)
                return LavaTexture.GetPixel(x, z);
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
                texture.SetPixel(j, textureResolution - i, GetColor(heightmapIndex, j, textureResolution - i));
            }
        }

        texture.Apply();
        return texture;
    }

}
