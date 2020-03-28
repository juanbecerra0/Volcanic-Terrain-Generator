using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    private int resolution;

    public void Initialize(int resolution)
    {
        this.resolution = resolution;
    }

    public Texture2D GenerateTexture(float[,] heightmap)
    {
        Texture2D texture = new Texture2D(resolution, resolution);

        for(int i = 0; i < resolution; i++)
        {
            for(int j = 0; j < resolution; j++)
            {
                texture.SetPixel(i, j, Color.red);
            }
        }

        texture.Apply();
        return texture;
    }
}
