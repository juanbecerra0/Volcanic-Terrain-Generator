﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BiomeGen : MonoBehaviour
{
    private static int BiomeDimensions;
    private int SeedSpacing;
    private uint Water, Sand, Grass, Mountain, Snow;

    public void Init(int biomeDimensions, int seedSpacing, uint waterSymbol, uint sandSymbol, uint grassSymbol, uint mountainSymbol, uint snowSymbol)
    {
        BiomeDimensions = biomeDimensions;
        SeedSpacing = seedSpacing;
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
    }

    public void WriteAsTexture(uint[,] Biome, String filepath)
    {
        Texture2D image = new Texture2D(BiomeDimensions, BiomeDimensions);

        for (int i = 0; i < BiomeDimensions; i++)
        {
            for (int j = 0; j < BiomeDimensions; j++)
            {
                if (Biome[i, j] == 3)
                    image.SetPixel(i, j, Color.green);
                else if (Biome[i, j] == 4)
                    image.SetPixel(i, j, Color.grey);
                else if (Biome[i, j] == 5)
                    image.SetPixel(i, j, Color.white);
                else
                    image.SetPixel(i, j, Color.red);
            }
        }

        byte[] _bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(filepath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + filepath);

        //GUI.DrawTexture(new Rect(0, 0, BiomeDimensions, BiomeDimensions), image);
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

            int cycle = UnityEngine.Random.Range(1, 8);
            SeedAgent newAgent = SeedAgent.Create(0, -1, -1);

            for (int i = 0; i < 8; i++)
            {
                switch (cycle)
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
                    break;
                }

                if (cycle < 8)
                    cycle++;
                else
                    cycle = 1;

            }
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

        // Enqueue several mountain agents
        SeedAgent mountainAgent1 = SeedAgent.Create(Mountain, snowAgent.GetX() + SeedSpacing, snowAgent.GetY());
        SeedAgent mountainAgent2 = SeedAgent.Create(Mountain, snowAgent.GetX(), snowAgent.GetY() + SeedSpacing);
        SeedAgent mountainAgent3 = SeedAgent.Create(Mountain, snowAgent.GetX() - SeedSpacing, snowAgent.GetY());
        SeedAgent mountainAgent4 = SeedAgent.Create(Mountain, snowAgent.GetX(), snowAgent.GetY() - SeedSpacing);

        // Verify mountain agents, then generate/verify grass agents
        // TODO add sand and water agents
        if (mountainAgent1 != null)
        {
            AgentQueue.Enqueue(mountainAgent1);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX(), mountainAgent1.GetY() + SeedSpacing);
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent2 != null)
        {
            AgentQueue.Enqueue(mountainAgent2);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX() + SeedSpacing, mountainAgent1.GetY());
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent3 != null)
        {
            AgentQueue.Enqueue(mountainAgent3);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX(), mountainAgent1.GetY() - SeedSpacing);
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent4 != null)
        {
            AgentQueue.Enqueue(mountainAgent4);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX() - SeedSpacing, mountainAgent1.GetY());
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }

        return AgentQueue;
    }


}
