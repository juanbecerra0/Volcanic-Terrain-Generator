using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightmapGen : MonoBehaviour
{
    // Map database script
    private MapDatabase MapDatabaseScript;

    // Variables
    private int HeightmapDimensions;
    private float HeightmapCornerMin;
    private float HeightmapCornerMax;
    private static float HeightmapDisplacementMin;
    private static float HeightmapDisplacementMax;

    public void Init(float heightmapBaseN, float cornerMin, float cornerMax, float displacementMin, float displacementMax)
    {
        MapDatabaseScript = GameObject.FindObjectOfType(typeof(MapDatabase)) as MapDatabase;

        HeightmapDimensions = (int)Mathf.Pow(2, heightmapBaseN) + 1;
        HeightmapCornerMin = cornerMin;
        HeightmapCornerMax = cornerMax;
        HeightmapDisplacementMin = displacementMin;
        HeightmapDisplacementMax = displacementMax;
    }

    // Generates and adds heightmap to dictionary 
    // based on initial cartesian coordinates
    public float[,] GenerateHeightmap(int x, int y, uint[,] subBiome)
    {
        // Create 2D array of noise values
        float[,] heightmap = new float[HeightmapDimensions, HeightmapDimensions];

        // Check for adjacent heightmaps
        Tuple<bool, bool, bool, bool> adjacentTruthTable = new Tuple<bool, bool, bool, bool>(
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

        // Set random value to each of the four corners of the heightmap
        if (!adjacentTruthTable.Item1 && !adjacentTruthTable.Item4)
            heightmap[0, 0] = UnityEngine.Random.Range(HeightmapCornerMin, HeightmapCornerMax);

        if (!adjacentTruthTable.Item3 && !adjacentTruthTable.Item4)
            heightmap[HeightmapDimensions - 1, 0] = UnityEngine.Random.Range(HeightmapCornerMin, HeightmapCornerMax);

        if (!adjacentTruthTable.Item1 && !adjacentTruthTable.Item2)
            heightmap[0, HeightmapDimensions - 1] = UnityEngine.Random.Range(HeightmapCornerMin, HeightmapCornerMax);

        if (!adjacentTruthTable.Item2 && !adjacentTruthTable.Item3)
            heightmap[HeightmapDimensions - 1, HeightmapDimensions - 1] = UnityEngine.Random.Range(HeightmapCornerMin, HeightmapCornerMax);

        // Recursive diamond-square terrain generation algorithm
        DiamondSquareGen(heightmap, 0, HeightmapDimensions - 1, 0, HeightmapDimensions - 1, adjacentTruthTable);

        // Perform fractal brownian motion
        DistortionGen(heightmap);

        MapDatabaseScript.AddHeightmap(x, y, heightmap);
        return heightmap;
    }

    // Recursively performs the diamond-square algorithm to generate terrain
    private static void DiamondSquareGen(float[,] heightmap, int xMin, int xMax, int yMin, int yMax, Tuple<bool, bool, bool, bool> adjacentTruthTable)
    {
        // Diamond step
        Tuple<int, int> diamondIndex = getDiamondIndex(xMin, xMax, yMin, yMax);
        float cornerAverage = (heightmap[xMin, yMin] + heightmap[xMax, yMin] + heightmap[xMin, yMax] + heightmap[xMax, yMax]) / 4;

        heightmap[diamondIndex.Item1, diamondIndex.Item2] = cornerAverage + getRandomDisplacement();

        // Square step
        Tuple<Tuple<int, int>, Tuple<int, int>, Tuple<int, int>, Tuple<int, int>> squareIndicies = getSquareIndices(xMin, xMax, yMin, yMax);

        float topAverage = (heightmap[xMin, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMin, yMax]) / 3;
        float rightAverage = (heightmap[xMin, yMax] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMax]) / 3; ;
        float bottomAverage = (heightmap[xMax, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMax]) / 3; ;
        float leftAverage = (heightmap[xMin, yMin] + heightmap[diamondIndex.Item1, diamondIndex.Item2] + heightmap[xMax, yMin]) / 3; ;

        if (!adjacentTruthTable.Item1 || squareIndicies.Item1.Item1 != 0)
            heightmap[squareIndicies.Item1.Item1, squareIndicies.Item1.Item2] = topAverage + getRandomDisplacement();

        if (!adjacentTruthTable.Item2 || squareIndicies.Item2.Item2 != heightmap.GetLength(0) - 1)
            heightmap[squareIndicies.Item2.Item1, squareIndicies.Item2.Item2] = rightAverage + getRandomDisplacement();

        if (!adjacentTruthTable.Item3 || squareIndicies.Item3.Item1 != heightmap.GetLength(0) - 1)
            heightmap[squareIndicies.Item3.Item1, squareIndicies.Item3.Item2] = bottomAverage + getRandomDisplacement();

        if (!adjacentTruthTable.Item4 || squareIndicies.Item4.Item2 != 0)
            heightmap[squareIndicies.Item4.Item1, squareIndicies.Item4.Item2] = leftAverage + getRandomDisplacement();

        // Determine if recursive step is required
        if (xMax - xMin <= 2 && yMax - yMin <= 2)
        {
            return; // Base case
        }
        else
        {
            // Recursive calls on sub-problems
            DiamondSquareGen(heightmap, xMin, (xMax / 2) + (xMin / 2), yMin, (yMax / 2) + (yMin / 2), adjacentTruthTable);    // Top-left
            DiamondSquareGen(heightmap, (xMax / 2) + (xMin / 2), xMax, yMin, (yMax / 2) + (yMin / 2), adjacentTruthTable);    // Top-right
            DiamondSquareGen(heightmap, (xMax / 2) + (xMin / 2), xMax, (yMax / 2) + (yMin / 2), yMax, adjacentTruthTable);    // Bottom-right
            DiamondSquareGen(heightmap, xMin, (xMax / 2) + (xMin / 2), (yMax / 2) + (yMin / 2), yMax, adjacentTruthTable);    // Bottom-left
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

    // Returns a random float between min and max displacement values (inclusive)
    private static float getRandomDisplacement()
    {
        return UnityEngine.Random.Range(HeightmapDisplacementMin, HeightmapDisplacementMax);
    }

    // Distorts a generated noisemap to seem more organic
    private static void DistortionGen(float[,] heightmap)
    {
        float[,] originalHeightmapValues = (float[,])heightmap.Clone();

        for (int i = 0; i < heightmap.GetLength(0); i++)
        {
            for (int j = 0; j < heightmap.GetLength(1); j++)
            {
                heightmap[i, j] = GetDistortionValue(originalHeightmapValues, i, j);
            }
        }
    }

    // Given a 2D index of the heightmap, returns a new value of applied swirling
    private static float GetDistortionValue(float[,] originalHeightmapValues, int x, int y)
    {
        return originalHeightmapValues[x, y];
    }
}
