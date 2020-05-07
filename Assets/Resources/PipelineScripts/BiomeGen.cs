using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BiomeGen : MonoBehaviour
{
    private static int BiomeDimensions;
    private int SeedSpacing;
    private float DisplacementDiv;
    private uint Water, Sand, Grass, Mountain, Snow;

    public void Init(int biomeDimensions, int seedSpacing, float displacementDiv, Tuple<uint, uint, uint, uint, uint> biomeTuple)
    {
        BiomeDimensions = biomeDimensions;
        SeedSpacing = seedSpacing;
        DisplacementDiv = displacementDiv;
        Water = biomeTuple.Item1;
        Sand = biomeTuple.Item2;
        Grass = biomeTuple.Item3;
        Mountain = biomeTuple.Item4;
        Snow = biomeTuple.Item5;
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
        Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
        {
            Color32[] original = originalTexture.GetPixels32();
            Color32[] rotated = new Color32[original.Length];
            int w = originalTexture.width;
            int h = originalTexture.height;

            int iRotated, iOriginal;

            for (int j = 0; j < h; ++j)
            {
                for (int i = 0; i < w; ++i)
                {
                    iRotated = (i + 1) * h - j - 1;
                    iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                    rotated[iRotated] = original[iOriginal];
                }
            }

            Texture2D rotatedTexture = new Texture2D(h, w);
            rotatedTexture.SetPixels32(rotated);
            rotatedTexture.Apply();
            return rotatedTexture;
        }

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

        image = rotateTexture(image, true);
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

    public Tuple<uint[,], Tuple<int, int>> GenerateBiome(uint[,] Top, uint[,] Right, uint[,] Bottom, uint[,] Left)
    {
        Biome = new uint[BiomeDimensions, BiomeDimensions];

        // Make the agent queue
        Queue<SeedAgent> AgentQueue = new Queue<SeedAgent>();

        // Enqueue all adjacent biome seeds
        for(int Selection = 1; Selection <= 4; Selection++)
        {
            SeedAgent newAgent = SeedAgent.Create(0, -1, -1);
            switch (Selection)
            {
                // Top
                case 1:
                    if (Top == null)
                        continue;
                    for(int i = 0; i < BiomeDimensions; i++)
                    {
                        newAgent = SeedAgent.Create(Top[BiomeDimensions - 1, i], 0, i);
                        if (newAgent != null)
                            AgentQueue.Enqueue(newAgent);
                    }
                    continue;
                // Right
                case 2:
                    if (Right == null)
                        continue;
                    for (int i = 0; i < BiomeDimensions; i++)
                    {
                        newAgent = SeedAgent.Create(Right[i, 0], i, BiomeDimensions - 1);
                        if (newAgent != null)
                            AgentQueue.Enqueue(newAgent);
                    }
                    continue;
                // Down
                case 3:
                    if (Bottom == null)
                        continue;
                    for (int i = 0; i < BiomeDimensions; i++)
                    {
                        newAgent = SeedAgent.Create(Bottom[0, i], BiomeDimensions - 1, i);
                        if (newAgent != null)
                            AgentQueue.Enqueue(newAgent);
                    }
                    continue;
                // Left
                case 4:
                    if (Left == null)
                        continue;
                    for (int i = 0; i < BiomeDimensions; i++)
                    {
                        newAgent = SeedAgent.Create(Left[i, BiomeDimensions - 1], i, 0);
                        if (newAgent != null)
                            AgentQueue.Enqueue(newAgent);
                    }
                    continue;
                default:
                    break;
            }
        }

        // Create and enqueue all init seed agents
        Tuple<int, int> center = new Tuple<int, int>(UnityEngine.Random.Range(2 * (BiomeDimensions / 5), 3 * (BiomeDimensions / 5)), UnityEngine.Random.Range(2 * (BiomeDimensions / 5), 3 * (BiomeDimensions / 5)));
        Queue<SeedAgent> SeedAgentQueue = GetSeedAgentQueue(center);
        while(SeedAgentQueue.Count > 0)
            AgentQueue.Enqueue(SeedAgentQueue.Dequeue());

        // BFS the AgentQueue, marking adjacent coordinates of Biome
        while (AgentQueue.Count > 0)
        {
            SeedAgent agent = AgentQueue.Dequeue();

            //int cycle = UnityEngine.Random.Range(1, 9);
            SeedAgent newAgent = SeedAgent.Create(0, -1, -1);

            for (int i = 1; i <= 8; i++)
            {
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
                        break;
                }

                if (newAgent != null)
                {
                    AgentQueue.Enqueue(newAgent);
                }
            }
        }

        return new Tuple<uint[,], Tuple<int, int>>(Biome, center);
    }

    private Queue<SeedAgent> GetSeedAgentQueue(Tuple<int, int> center)
    {
        int GetRandomDisplacement() { return UnityEngine.Random.Range((int)(-SeedSpacing / DisplacementDiv), (int)(SeedSpacing / DisplacementDiv)); }

        // Create a queue of SeedAgents
        Queue<SeedAgent> AgentQueue = new Queue<SeedAgent>();

        // Enqueue the snow agent
        SeedAgent snowAgent = SeedAgent.Create(Snow, center.Item1, center.Item2);
        AgentQueue.Enqueue(snowAgent);

        void GenerateRadialAgents(uint BiomeType, int radius, int centerX, int centerY)
        {
            for (float t = 0; t < 2 * Math.PI; t += 0.01f)
            {
                var x = Math.Sin(t) * radius;
                var y = Math.Cos(t) * radius;

                SeedAgent agent = SeedAgent.Create(BiomeType, (int)x + centerX + GetRandomDisplacement(), (int)y + centerY + GetRandomDisplacement());
                if (agent != null)
                {
                    AgentQueue.Enqueue(agent);
                }
            }
        }

        // Generate radial mountain, grass, sand, and water agents
        GenerateRadialAgents(Mountain, SeedSpacing, snowAgent.GetX(), snowAgent.GetY());
        GenerateRadialAgents(Grass, SeedSpacing * 2, snowAgent.GetX(), snowAgent.GetY());
        GenerateRadialAgents(Grass, SeedSpacing * 3, snowAgent.GetX(), snowAgent.GetY());
        GenerateRadialAgents(Sand, SeedSpacing * 4, snowAgent.GetX(), snowAgent.GetY());
        GenerateRadialAgents(Water, SeedSpacing * 5, snowAgent.GetX(), snowAgent.GetY());

        return AgentQueue;
    }
}
