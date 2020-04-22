using System;
using System.Collections.Generic;
using UnityEngine;

public class MapDatabase : MonoBehaviour
{
    // Databases
    private Dictionary<Tuple<int, int>, float[,]> HeightmapDatabase;
    private Dictionary<Tuple<int, int>, uint[,]> BiomeDatabase;

    private HashSet<Tuple<int, int>> HeightmapProcessed;
    private HashSet<Tuple<int, int>> BiomeProcessed;

    BiomeGen BiomeGenScript;
    private int BiomeHMContentsWidth;
    private int BiomePartitionWidth;

    public void Init(int heightmapBaseN, int biomeHMContentsWidth)
    {
        HeightmapDatabase = new Dictionary<Tuple<int, int>, float[,]>();
        BiomeDatabase = new Dictionary<Tuple<int, int>, uint[,]>();

        HeightmapProcessed = new HashSet<Tuple<int, int>>();
        BiomeProcessed = new HashSet<Tuple<int, int>>();

        BiomeGenScript = GameObject.FindObjectOfType(typeof(BiomeGen)) as BiomeGen;
        BiomeHMContentsWidth = biomeHMContentsWidth;
        BiomePartitionWidth = (int)Mathf.Pow(2, heightmapBaseN) + 1;
    }

    public bool IsVacent(int x, int z)
    {
        return !HeightmapProcessed.Contains(new Tuple<int, int>(x, z));
    }

    public bool IsFilled(int x, int z)
    {
        return HeightmapProcessed.Contains(new Tuple<int, int>(x, z));
    }

    public void AddHeightmap(int x, int z, float[,] heightmap)
    {
        if(IsFilled(x, z))
            Debug.Log("WARNING: Overwriting heightmap at (" + x + ", " + z + ")!");

        HeightmapProcessed.Add(new Tuple<int, int>(x, z));
        HeightmapDatabase.Add(new Tuple<int, int>(x, z), heightmap);
        CleanHeightmap(x + 1, z);
        CleanHeightmap(x - 1, z);
        CleanHeightmap(x, z + 1);
        CleanHeightmap(x, z - 1);
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

    public void GeneratePossibleBiome(int x, int z)
    {
        Tuple<int, int> BiomeCoordinates = HeightmapToBiomeCoord(x, z);

        if (BiomeDatabase.ContainsKey(BiomeCoordinates))
            return;

        Tuple<int, int> TopCoord = new Tuple<int, int>(BiomeCoordinates.Item1, BiomeCoordinates.Item2 + BiomeHMContentsWidth);
        Tuple<int, int> RightCoord = new Tuple<int, int>(BiomeCoordinates.Item1 + BiomeHMContentsWidth, BiomeCoordinates.Item2);
        Tuple<int, int> BottomCoord = new Tuple<int, int>(BiomeCoordinates.Item1, BiomeCoordinates.Item2 - BiomeHMContentsWidth);
        Tuple<int, int> LeftCoord = new Tuple<int, int>(BiomeCoordinates.Item1 - BiomeHMContentsWidth, BiomeCoordinates.Item2);

        uint[,] TopBiome = BiomeDatabase.ContainsKey(TopCoord) ? BiomeDatabase[TopCoord] : null;
        uint[,] RightBiome = BiomeDatabase.ContainsKey(RightCoord) ? BiomeDatabase[RightCoord] : null;
        uint[,] BottomBiome = BiomeDatabase.ContainsKey(BottomCoord) ? BiomeDatabase[BottomCoord] : null;
        uint[,] LeftBiome = BiomeDatabase.ContainsKey(LeftCoord) ? BiomeDatabase[LeftCoord] : null;

        BiomeDatabase.Add(BiomeCoordinates, BiomeGenScript.GenerateBiome(TopBiome, RightBiome, BottomBiome, LeftBiome));
    }

    public uint[,] GetSubBiome(int x, int z)
    {
        Tuple<int, int> biomeCoordinates = HeightmapToBiomeCoord(x, z);

        uint[,] correspondingBiome = BiomeDatabase[biomeCoordinates];
        int biomeDimensions = correspondingBiome.GetLength(0);

        // Starting indexes for copying
        int LRIndex = (int)(biomeDimensions * (
            (x >= 0) ? 
                ((float)(x - biomeCoordinates.Item1) / BiomeHMContentsWidth) : 
                (-((float)biomeCoordinates.Item1 - x) / BiomeHMContentsWidth)
            ));

        int UDIndex = (int)(biomeDimensions * (
            (z >= 0) ?
                ((float)((biomeCoordinates.Item2 + BiomeHMContentsWidth) - (z + 1)) / BiomeHMContentsWidth) :
                (-((float)((z + 1) + (biomeCoordinates.Item2 + BiomeHMContentsWidth))) / BiomeHMContentsWidth)
            ));

        if (LRIndex + BiomePartitionWidth > biomeDimensions)
            LRIndex = biomeDimensions - BiomePartitionWidth;

        if (UDIndex + BiomePartitionWidth > biomeDimensions)
            UDIndex = biomeDimensions - BiomePartitionWidth;

        uint[,] subBiome = new uint[BiomePartitionWidth, BiomePartitionWidth];

        for (int i = 0; i < BiomePartitionWidth; i++)
        {
            for (int j = 0; j < BiomePartitionWidth; j++)
            {
                subBiome[i, j] = correspondingBiome[UDIndex + i, LRIndex + j];
            }
        }

        return subBiome;
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

    private void CleanHeightmap(int x, int z)
    {
        if (HeightmapProcessed.Contains(new Tuple<int, int>(x, z + 1)) &&
            //HeightmapProcessed.Contains(new Tuple<int, int>(x + 1, z + 1)) &&
            HeightmapProcessed.Contains(new Tuple<int, int>(x + 1, z)) &&
            //HeightmapProcessed.Contains(new Tuple<int, int>(x + 1, z - 1)) &&
            HeightmapProcessed.Contains(new Tuple<int, int>(x, z - 1)) &&
            //HeightmapProcessed.Contains(new Tuple<int, int>(x - 1, z - 1)) &&
            HeightmapProcessed.Contains(new Tuple<int, int>(x - 1, z)) //&&
            //HeightmapProcessed.Contains(new Tuple<int, int>(x - 1, z + 1))
            )
        {
            //Debug.Log(x + " " + z + " cleaned!");
            HeightmapDatabase.Remove(new Tuple<int, int>(x, z));
        }
    }

    private void CleanBiome(int x, int z)
    {

    }
}
