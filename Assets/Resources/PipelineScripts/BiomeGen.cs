﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BiomeGen : MonoBehaviour
{
    private static int BiomeDimensions;
    private int SeedSpacing;
    private int RadialSeeds;
    private uint Water, Sand, Grass, Mountain, Snow;

    public void Init(int biomeDimensions, int seedSpacing, int radialSeeds, uint waterSymbol, uint sandSymbol, uint grassSymbol, uint mountainSymbol, uint snowSymbol)
    {
        BiomeDimensions = biomeDimensions;
        SeedSpacing = seedSpacing;
        RadialSeeds = radialSeeds;
        Water = waterSymbol;
        Sand = sandSymbol;
        Grass = grassSymbol;
        Mountain = mountainSymbol;
        Snow = snowSymbol;
    }

    private static uint[,] Biome;

    public void WriteAsTextFile(uint[,] Biome, String filepath)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < BiomeDimensions; i++)
        {
            for (int j = 0; j < BiomeDimensions; j++)
            {
                sb.Append(Biome[i, j] + " ");
            }
            sb.Append("\n");
        }

        System.IO.File.WriteAllText(filepath, sb.ToString());
        Debug.Log("Text file saved to " + filepath);
    }

    public void WriteAsTexture(uint[,] Biome, String filepath)
    {
        Texture2D image = new Texture2D(BiomeDimensions, BiomeDimensions);

        for (int i = 0; i < BiomeDimensions; i++)
        {
            for (int j = 0; j < BiomeDimensions; j++)
            {
                if (Biome[i, j] == 1)
                    image.SetPixel(i, j, Color.blue);
                else if (Biome[i, j] == 2)
                    image.SetPixel(i, j, Color.yellow);
                else if (Biome[i, j] == 3)
                    image.SetPixel(i, j, Color.green);
                else if (Biome[i, j] == 4)
                    image.SetPixel(i, j, Color.grey);
                else if (Biome[i, j] == 5)
                    image.SetPixel(i, j, Color.white);
                else
                    image.SetPixel(i, j, Color.red);
            }
        }

        byte[] bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(filepath, bytes);
        Debug.Log("Image saved to " + filepath);
    }

    private class SeedAgent
    {
        private readonly uint BiomeType;
        private readonly int X;
        private readonly int Y;

        private SeedAgent(uint BiomeType, int X, int Y)
        {
            this.BiomeType = BiomeType;
            this.X = X;
            this.Y = Y;

            Biome[X, Y] = BiomeType;
        }

        public static SeedAgent Create(uint BiomeType, int X, int Y)
        {
            if (X >= 0 && X < BiomeDimensions && Y >= 0 && Y < BiomeDimensions && Biome[X, Y] == 0)
                return new SeedAgent(BiomeType, X, Y);
            else
                return null;
        }

        public bool IsViable()
        {
            return (
                (X - 1 >= 0 && Biome[X - 1, Y] == 0) ||
                (X - 1 >= 0 && Y + 1 < BiomeDimensions && Biome[X - 1, Y + 1] == 0) ||
                (Y + 1 < BiomeDimensions && Biome[X, Y + 1] == 0) ||
                (X + 1 < BiomeDimensions && Y + 1 < BiomeDimensions && Biome[X + 1, Y + 1] == 0) ||
                (X + 1 < BiomeDimensions && Biome[X + 1, Y] == 0) ||
                (X + 1 < BiomeDimensions && Y - 1 >= 0 && Biome[X + 1, Y - 1] == 0) ||
                (Y - 1 >= 0 && Biome[X, Y - 1] == 0) ||
                (X - 1 >= 0 && Y - 1 >= 0 && Biome[X - 1, Y - 1] == 0)
            );
        }

        public uint GetBiomeType() { return BiomeType; }
        public int GetX() { return X; }
        public int GetY() { return Y; }
    }

    public uint[,] GenerateBiome()
    {
        Biome = new uint[BiomeDimensions, BiomeDimensions];

        Queue<SeedAgent> AgentQueue = GetSeedAgentQueue();
        int writes = 0;

        // BFS the AgentQueue, marking adjacent coordinates of Biome
        while (AgentQueue.Count > 0)
        {
            writes++;
            SeedAgent agent = AgentQueue.Dequeue();

            //int cycle = UnityEngine.Random.Range(1, 9);
            SeedAgent newAgent = SeedAgent.Create(0, -1, -1);

            for (int i = 1; i <= 8; i++)
            {
                /*
                if (UnityEngine.Random.Range(1, 3) == 2)
                    continue;
                    */

                switch (i)
                {
                    // Up
                    case 1:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() - 1, agent.GetY());
                        break;
                    // Up-right
                    case 2:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() - 1, agent.GetY() + 1);
                        break;
                    // Right
                    case 3:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX(), agent.GetY() + 1);
                        break;
                    // Down-right
                    case 4:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() + 1, agent.GetY() + 1);
                        break;
                    // Down
                    case 5:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() + 1, agent.GetY());
                        break;
                    // Down-left
                    case 6:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() + 1, agent.GetY() - 1);
                        break;
                    // Left
                    case 7:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX(), agent.GetY() - 1);
                        break;
                    // Up-left
                    case 8:
                        newAgent = SeedAgent.Create(agent.GetBiomeType(), agent.GetX() - 1, agent.GetY() - 1);
                        break;
                    // Uh oh
                    default:
                        Debug.Log("frig");
                        break;
                }

                if (newAgent != null)
                {
                    AgentQueue.Enqueue(newAgent);
                    //break;
                }

                /*
                if (cycle < 8)
                    cycle++;
                else
                    cycle = 1;
                    */

            }

            /*
            if (agent.IsViable())
                AgentQueue.Enqueue(agent);
                */

        }

        Debug.Log(writes);
        return Biome;
    }

    public uint[,] GenerateBiome(uint[,] Top, uint[,] Right, uint[,] Bottom, uint[,] Left)
    {
        uint[,] Biome = new uint[BiomeDimensions, BiomeDimensions];

        // TODO

        return Biome;
    }

    private Queue<SeedAgent> GetSeedAgentQueue()
    {
        // Create a queue of SeedAgents
        Queue<SeedAgent> AgentQueue = new Queue<SeedAgent>();

        // Enqueue the snow agent
        SeedAgent snowAgent = SeedAgent.Create(Snow, UnityEngine.Random.Range(BiomeDimensions / 4, (BiomeDimensions / 4) * 3), UnityEngine.Random.Range(BiomeDimensions / 4, (BiomeDimensions / 4) * 3));
        AgentQueue.Enqueue(snowAgent);

        int AgentCount = 0;

        // Setup radial agent generation
        int centerX = snowAgent.GetX();
        int centerY = snowAgent.GetY();

        // Generate mountain agents
        int radius = SeedSpacing;
        for (float t = 0; t < 2 * Math.PI; t += 0.01f)
        {
            var x = Math.Sin(t) * radius;
            var y = Math.Cos(t) * radius;

            SeedAgent mountainAgent = SeedAgent.Create(Mountain, (int) x + centerX, (int) y + centerY);
            if (mountainAgent != null)
            {
                Biome[(int)x + centerX, (int)y + centerY] = 1;
                AgentQueue.Enqueue(mountainAgent);
                AgentCount++;
            }
        }

        Debug.Log(AgentCount);
        AgentCount = 0;

        // Generate grass agents
        radius = SeedSpacing * 2;
        for (float t = 0; t < 2 * Math.PI; t += 0.01f)
        {
            var x = Math.Sin(t) * radius;
            var y = Math.Cos(t) * radius;

            SeedAgent grassAgent = SeedAgent.Create(Grass, (int)x + centerX, (int)y + centerY);
            if (grassAgent != null)
            {
                Biome[(int)x + centerX, (int)y + centerY] = 1;
                AgentQueue.Enqueue(grassAgent);
                AgentCount++;
            }
        }

        Debug.Log(AgentCount);
        AgentCount = 0;

        // Generate sand agents
        radius = SeedSpacing * 3;
        for (float t = 0; t < 2 * Math.PI; t += 0.01f)
        {
            var x = Math.Sin(t) * radius;
            var y = Math.Cos(t) * radius;

            SeedAgent sandAgent = SeedAgent.Create(Sand, (int)x + centerX, (int)y + centerY);
            if (sandAgent != null)
            {
                Biome[(int)x + centerX, (int)y + centerY] = 1;
                AgentQueue.Enqueue(sandAgent);
                AgentCount++;
            }
        }

        Debug.Log(AgentCount);
        AgentCount = 0;

        // Generate water agents
        radius = SeedSpacing * 4;
        for (float t = 0; t < 2 * Math.PI; t += 0.01f)
        {
            var x = Math.Sin(t) * radius;
            var y = Math.Cos(t) * radius;

            SeedAgent waterAgent = SeedAgent.Create(Water, (int)x + centerX, (int)y + centerY);
            if (waterAgent != null)
            {
                Biome[(int)x + centerX, (int)y + centerY] = 1;
                AgentQueue.Enqueue(waterAgent);
                AgentCount++;
            }
        }

        Debug.Log(AgentCount);

        return AgentQueue;
    }


}
