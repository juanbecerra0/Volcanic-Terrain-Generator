using System;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    // Databases
    private Dictionary<Tuple<int, int>, float[,]> HeightmapDatabase;
    private Dictionary<Tuple<int, int>, Texture2D> BiomeDatabase;

    public void Init()
    {
        HeightmapDatabase = new Dictionary<Tuple<int, int>, float[,]>();
        BiomeDatabase = new Dictionary<Tuple<int, int>, Texture2D>();
    }

    public bool IsVacent(int x, int z)
    {
        return !HeightmapDatabase.ContainsKey(new Tuple<int, int>(x, z));
    }

    public void AddHeightmap(int x, int z, float[,] heightmap)
    {
        if(!IsVacent(x, z))
            Debug.Log("WARNING: Overwriting heightmap at (" + x + ", " + z + ")!");

        // TODO query biome database

        HeightmapDatabase.Add(new Tuple<int, int>(x, z), heightmap);
    }
}
