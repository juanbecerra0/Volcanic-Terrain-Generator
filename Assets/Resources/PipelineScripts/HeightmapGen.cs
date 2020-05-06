using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HeightmapGen : MonoBehaviour
{
    // Map database script
    private MapDatabase MapDatabaseScript;

    // Variables
    private int HeightmapDimensions;

    private static uint Water, Sand, Grass, Mountain, Snow;
    private static float WaterBase, SandBase, GrassBase, MountainBase, SnowBase;
    private static float WaterDisp, SandDisp, GrassDisp, MountainDisp, SnowDisp;

    public void Init(float heightmapBaseN, Tuple<uint, uint, uint, uint, uint> biomeTuple, Tuple<float, float, float, float, float> bases, Tuple<float, float, float, float, float> displacements)
    {
        MapDatabaseScript = GameObject.FindObjectOfType(typeof(MapDatabase)) as MapDatabase;

        HeightmapDimensions = (int)Mathf.Pow(2, heightmapBaseN) + 1;

        Water = biomeTuple.Item1;
        Sand = biomeTuple.Item2;
        Grass = biomeTuple.Item3;
        Mountain = biomeTuple.Item4;
        Snow = biomeTuple.Item5;

        WaterBase = bases.Item1;
        SandBase = bases.Item2;
        GrassBase = bases.Item3;
        MountainBase = bases.Item4;
        SnowBase = bases.Item5;

        WaterDisp = displacements.Item1;
        SandDisp = displacements.Item2;
        GrassDisp = displacements.Item3;
        MountainDisp = displacements.Item4;
        SnowDisp = displacements.Item5;
    }

    private static Tuple<bool, bool, bool, bool> adjacentTruthTable;
    private static uint[,] BiomeMap;

    // Generates and adds heightmap to dictionary 
    // based on initial cartesian coordinates
    public float[,] GenerateHeightmap(int x, int y, uint[,] subBiome, float[,] gradient)
    {
        // Create 2D array of noise values
        float[,] heightmap = new float[HeightmapDimensions, HeightmapDimensions];

        // Assign this biome map
        BiomeMap = subBiome;

        // Check for adjacent heightmaps
        adjacentTruthTable = new Tuple<bool, bool, bool, bool>(
            MapDatabaseScript.IsFilled(x, y + 1),
            MapDatabaseScript.IsFilled(x + 1, y),
            MapDatabaseScript.IsFilled(x, y - 1),
            MapDatabaseScript.IsFilled(x - 1, y)
        );

        // Copy over top values
        if (adjacentTruthTable.Item1)
        {
            float[,] hm = MapDatabaseScript.GetHeightmap(x, y + 1);
            for (int i = 0; i < HeightmapDimensions; i++)
            {
                heightmap[0, i] = hm[HeightmapDimensions - 1, i];
            }
        }

        // Copy over right values
        if (adjacentTruthTable.Item2)
        {
            float[,] hm = MapDatabaseScript.GetHeightmap(x + 1, y);
            for (int i = 0; i < HeightmapDimensions; i++)
            {
                heightmap[i, HeightmapDimensions - 1] = hm[i, 0];
            }
        }

        // Copy over bottom values
        if (adjacentTruthTable.Item3)
        {
            float[,] hm = MapDatabaseScript.GetHeightmap(x, y - 1);
            for (int i = 0; i < HeightmapDimensions; i++)
            {
                heightmap[HeightmapDimensions - 1, i] = hm[0, i];
            }
        }

        // Copy over left values
        if (adjacentTruthTable.Item4)
        {
            float[,] hm = MapDatabaseScript.GetHeightmap(x - 1, y);
            for (int i = 0; i < HeightmapDimensions; i++)
            {
                heightmap[i, 0] = hm[i, HeightmapDimensions - 1];
            }
        }

        /*
        // Set values to uninitialized verticies
        if (!adjacentTruthTable.Item1 && !adjacentTruthTable.Item4)
            heightmap[0, 0] = GetBiomeVertex(0, 0);

        if (!adjacentTruthTable.Item3 && !adjacentTruthTable.Item4)
            heightmap[HeightmapDimensions - 1, 0] = GetBiomeVertex(HeightmapDimensions - 1, 0);

        if (!adjacentTruthTable.Item1 && !adjacentTruthTable.Item2)
            heightmap[0, HeightmapDimensions - 1] = GetBiomeVertex(0, HeightmapDimensions - 1);

        if (!adjacentTruthTable.Item2 && !adjacentTruthTable.Item3)
            heightmap[HeightmapDimensions - 1, HeightmapDimensions - 1] = GetBiomeVertex(HeightmapDimensions - 1, HeightmapDimensions - 1);
        */

        //DiamondSquareGen(heightmap, 0, HeightmapDimensions - 1, 0, HeightmapDimensions - 1);

        //ZeroGen(HeightmapDimensions, heightmap);

        //BasicBiomeGen(HeightmapDimensions, heightmap);

        AdvancedBiomeGen(HeightmapDimensions, heightmap, gradient);

        MapDatabaseScript.AddHeightmap(x, y, heightmap);
        return heightmap;
    }

    private static void ZeroGen(int dim, float[,] heightmap)
    {
        for(int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                heightmap[i, j] = 0f;
            }
        }
    }

    private static float GetBiomeVertex(int x, int y/*,float delta*/)
    {
        float GetHeight(float Base, /*float Delta,*/ float Disp) { return Base /*+ Delta*/ + UnityEngine.Random.Range(-Disp / 2, Disp / 2); }

        if (BiomeMap[x, y] == Water)
            return GetHeight(WaterBase, /*delta,*/ WaterDisp);
        else if (BiomeMap[x, y] == Sand)
            return GetHeight(SandBase, /*delta,*/ SandDisp);
        else if (BiomeMap[x, y] == Grass)
            return GetHeight(GrassBase, /*delta,*/ GrassDisp);
        else if (BiomeMap[x, y] == Mountain)
            return GetHeight(MountainBase, /*delta,*/ MountainDisp);
        else if (BiomeMap[x, y] == Snow)
            return GetHeight(SnowBase, /*delta,*/ SnowDisp);
        else
            return -10000f;
    }

    private static float GetBiomeVertex(int x, int y, float delta)
    {
        float GetHeight(float Base, float Delta, float Disp) { return Base + Delta + UnityEngine.Random.Range(-Disp, Disp); }

        if (BiomeMap[x, y] == Water)
            return GetHeight(WaterBase, delta, WaterDisp);
        else if (BiomeMap[x, y] == Sand)
            return GetHeight(SandBase, delta, SandDisp);
        else if (BiomeMap[x, y] == Grass)
            return GetHeight(GrassBase, delta, GrassDisp);
        else if (BiomeMap[x, y] == Mountain)
            return GetHeight(MountainBase, delta, MountainDisp);
        else if (BiomeMap[x, y] == Snow)
            return GetHeight(SnowBase, delta, SnowDisp);
        else
            return -10000f;
    }

    private static void BasicBiomeGen(int dim, float[,] heightmap)
    {
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (heightmap[i, j] != 0)
                    heightmap[i, j] = GetBiomeVertex(i, j);
            }
        }
    }

    private static void AdvancedBiomeGen(int dim, float[,] heightmap, float[,] gradient)
    {
        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                if(heightmap[i, j] == 0)
                    heightmap[i, j] = GetBiomeVertex(i, j, gradient[i,j]);
            }
        }
    }

    // Recursively performs the diamond-square algorithm to generate terrain
    private static void DiamondSquareGen(float[,] heightmap, int xMin, int xMax, int yMin, int yMax)
    {
        float GetRandomDisplacement() {
            return UnityEngine.Random.Range(-1.0f, 1.0f);
        }

        // Diamond step
        Tuple<int, int> diamondIndex = getDiamondIndex(xMin, xMax, yMin, yMax);
        float cornerAverage = (heightmap[xMin, yMin] + heightmap[xMax, yMin] + heightmap[xMin, yMax] + heightmap[xMax, yMax]) / 4;

        heightmap[diamondIndex.Item1, diamondIndex.Item2] = cornerAverage + GetRandomDisplacement();

        // Square step
        Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>, Tuple<int, int>> squareIndicies = getSquareIndices(xMin, xMax, yMin, yMax);

        float topAverage = (heightmap[xMin, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMin, yMax]) / 3;
        float rightAverage = (heightmap[xMin, yMax] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMax]) / 3; ;
        float bottomAverage = (heightmap[xMax, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMax]) / 3; ;
        float leftAverage = (heightmap[xMin, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMin]) / 3; ;

        if (!adjacentTruthTable.Item1 || squareIndicies.Item1.Item1 != 0)
            heightmap[squareIndicies.Item1.Item1, squareIndicies.Item1.Item2] = topAverage + GetRandomDisplacement();

        if (!adjacentTruthTable.Item2 || squareIndicies.Item2.Item2 != heightmap.GetLength(0) - 1)
            heightmap[squareIndicies.Item2.Item1, squareIndicies.Item2.Item2] = rightAverage + GetRandomDisplacement();

        if (!adjacentTruthTable.Item3 || squareIndicies.Item3.Item1 != heightmap.GetLength(0) - 1)
            heightmap[squareIndicies.Item3.Item1, squareIndicies.Item3.Item2] = bottomAverage + GetRandomDisplacement();

        if (!adjacentTruthTable.Item4 || squareIndicies.Item4.Item2 != 0)
            heightmap[squareIndicies.Item4.Item1, squareIndicies.Item4.Item2] = leftAverage + GetRandomDisplacement();

        // Determine if recursive step is required
        if (xMax - xMin <= 2 && yMax - yMin <= 2)
        {
            return; // Base case
        }
        else
        {
            // Recursive calls on sub-problems
            DiamondSquareGen(heightmap, xMin, (xMax / 2) + (xMin / 2), yMin, (yMax / 2) + (yMin / 2));    // Top-left
            DiamondSquareGen(heightmap, (xMax / 2) + (xMin / 2), xMax, yMin, (yMax / 2) + (yMin / 2));    // Top-right
            DiamondSquareGen(heightmap, (xMax / 2) + (xMin / 2), xMax, (yMax / 2) + (yMin / 2), yMax);    // Bottom-right
            DiamondSquareGen(heightmap, xMin, (xMax / 2) + (xMin / 2), (yMax / 2) + (yMin / 2), yMax);    // Bottom-left
        }
    }

    // Returns the diamond index for DS-algorithm
    // given min/max x and y index values
    private static Tuple<int, int> getDiamondIndex(int xMin, int xMax, int yMin, int yMax)
    {
        return new Tuple<int, int>(
            (xMin + ((xMax - xMin) / 2)),
            (yMin + ((yMax - yMin) / 2))
        );
    }

    // Returns an ordered list of the square indicies for DS-algorithm
    // given min/max x and y index values
    private static Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>, Tuple<int, int>> getSquareIndices(int xMin, int xMax, int yMin, int yMax)
    {
        // This is horrible. Everything is horrible.
        return new Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>, Tuple<int, int>>(
            new Tuple<int, int>(
                (xMin),
                (yMin + ((yMax - yMin) / 2))
            ),
            new Tuple<int, int>(
                (xMin + ((xMax - xMin) / 2)),
                (yMax)
            ),
            new Tuple<int, int>(
                (xMax),
                (yMin + ((yMax - yMin) / 2))
            ),
            new Tuple<int, int>(
                (xMin + ((xMax - xMin) / 2)),
                (yMin)
            )
        );
    }
}
