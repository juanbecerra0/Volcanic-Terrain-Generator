using System;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    // Databases
    private Dictionary<Tuple<int, int>, float[,]> HeightmapDatabase;
    private Dictionary<Tuple<int, int>, uint[,]> BiomeDatabase;

    BiomeGen BiomeGenScript;
    private int BiomeHMContentsWidth;

    public void Init(int biomeHMContentsWidth)
    {
        HeightmapDatabase = new Dictionary<Tuple<int, int>, float[,]>();
        BiomeDatabase = new Dictionary<Tuple<int, int>, uint[,]>();

        BiomeGenScript = GameObject.FindObjectOfType(typeof(BiomeGen)) as BiomeGen;
        BiomeHMContentsWidth = biomeHMContentsWidth;
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

        Tuple<int, int> BiomeCoordinates = HeightmapToBiomeCoord(x, z);

        Tuple<int, int> TopCoord = new Tuple<int, int>(BiomeCoordinates.Item1, BiomeCoordinates.Item2 + BiomeHMContentsWidth);
        Tuple<int, int> RightCoord = new Tuple<int, int>(BiomeCoordinates.Item1 + BiomeHMContentsWidth, BiomeCoordinates.Item2);
        Tuple<int, int> BottomCoord = new Tuple<int, int>(BiomeCoordinates.Item1, BiomeCoordinates.Item2 - BiomeHMContentsWidth);
        Tuple<int, int> LeftCoord = new Tuple<int, int>(BiomeCoordinates.Item1 - BiomeHMContentsWidth, BiomeCoordinates.Item2);

        uint[,] TopBiome = BiomeDatabase.ContainsKey(TopCoord) ? BiomeDatabase[TopCoord] : null;
        uint[,] RightBiome = BiomeDatabase.ContainsKey(RightCoord) ? BiomeDatabase[RightCoord] : null;
        uint[,] BottomBiome = BiomeDatabase.ContainsKey(BottomCoord) ? BiomeDatabase[BottomCoord] : null;
        uint[,] LeftBiome = BiomeDatabase.ContainsKey(LeftCoord) ? BiomeDatabase[LeftCoord] : null;

        if (!BiomeDatabase.ContainsKey(BiomeCoordinates))
            BiomeDatabase.Add(BiomeCoordinates, BiomeGenScript.GenerateBiome(TopBiome, RightBiome, BottomBiome, LeftBiome));

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

    private Tuple<int, int> HeightmapToBiomeCoord(int x, int z)
    {
        int xCoord, zCoord;

        if (x >= 0)
            xCoord = x - (x % BiomeHMContentsWidth);
        else
            xCoord = x - (BiomeHMContentsWidth + (x % BiomeHMContentsWidth));

        if(z >= 0)
            zCoord = z - (z % BiomeHMContentsWidth);
        else
            zCoord = z - (BiomeHMContentsWidth + (z % BiomeHMContentsWidth));

        return new Tuple<int, int>(xCoord, zCoord);
    }
}
