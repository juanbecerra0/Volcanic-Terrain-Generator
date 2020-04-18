using System;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    // Databases
    private Dictionary<Tuple<int, int>, float[,]> HeightmapDatabase;
    private Dictionary<Tuple<int, int>, Texture2D> BiomeDatabase;
    private int BiomeHMContentsWidth;

    public void Init(int biomeHMContentsWidth)
    {
        BiomeHMContentsWidth = biomeHMContentsWidth;
        HeightmapDatabase = new Dictionary<Tuple<int, int>, float[,]>();
        BiomeDatabase = new Dictionary<Tuple<int, int>, Texture2D>();
    }

    public bool IsVacent(int x, int z)
    {
        return !HeightmapDatabase.ContainsKey(new Tuple<int, int>(x, z));
    }

    public bool IsFilled(int x, int z)
    {
        return HeightmapDatabase.ContainsKey(new Tuple<int, int>(x, z));
    }

    public void AddHeightmap(int x, int z, float[,] heightmap)
    {
        if(IsFilled(x, z))
            Debug.Log("WARNING: Overwriting heightmap at (" + x + ", " + z + ")!");

        // TODO query biome database

        HeightmapDatabase.Add(new Tuple<int, int>(x, z), heightmap);
    }

    public float[,] GetHeightmap(int x, int z)
    {
        if (IsVacent(x, z))
        {
            Debug.Log("ERROR: Heightmap at (" + x + ", " + z + ") does not exist! Returning null.");
            return null;
        }

        return HeightmapDatabase[new Tuple<int, int>(x, z)];
    }
}
