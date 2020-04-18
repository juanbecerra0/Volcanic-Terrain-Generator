using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGen : MonoBehaviour
{
    private static int BiomeDimensions;
    private uint Water, Sand, Grass, Mountain, Snow;

    public void Init(int biomeDimensions, uint waterSymbol, uint sandSymbol, uint grassSymbol, uint mountainSymbol, uint snowSymbol)
    {
        BiomeDimensions = biomeDimensions;
        Water = waterSymbol;
        Sand = sandSymbol;
        Grass = grassSymbol;
        Mountain = mountainSymbol;
        Snow = snowSymbol;
    }

    private static uint[,] Biome;

    private class SeedAgent
    {
        private uint BiomeType;
        private int X;
        private int Y;

        private SeedAgent(uint BiomeType, int X, int Y)
        {
            this.BiomeType = BiomeType;
            this.X = X;
            this.Y = Y;

            Biome[X, Y] = BiomeType;
        }

        public static SeedAgent Create(uint BiomeType, int X, int Y)
        {
            if (X >= 0 && X < BiomeDimensions && Y >= 0 && Y < BiomeDimensions)
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
        
        // BFS the AgentQueue, marking adjacent coordinates of Biome
        while(AgentQueue.Count > 0)
        {
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
        SeedAgent mountainAgent1 = SeedAgent.Create(Mountain, snowAgent.GetX() + 2, snowAgent.GetY());
        SeedAgent mountainAgent2 = SeedAgent.Create(Mountain, snowAgent.GetX(), snowAgent.GetY() + 2);
        SeedAgent mountainAgent3 = SeedAgent.Create(Mountain, snowAgent.GetX() - 2, snowAgent.GetY());
        SeedAgent mountainAgent4 = SeedAgent.Create(Mountain, snowAgent.GetX(), snowAgent.GetY() - 2);

        // Verify mountain agents, then generate/verify grass agents
        // TODO add sand and water agents
        if (mountainAgent1 != null)
        {
            AgentQueue.Enqueue(mountainAgent1);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX(), mountainAgent1.GetY() + 2);
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent2 != null)
        {
            AgentQueue.Enqueue(mountainAgent2);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX() + 2, mountainAgent1.GetY());
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent3 != null)
        {
            AgentQueue.Enqueue(mountainAgent3);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX(), mountainAgent1.GetY() - 2);
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }
        if (mountainAgent4 != null)
        {
            AgentQueue.Enqueue(mountainAgent4);
            SeedAgent grassAgent = SeedAgent.Create(Grass, mountainAgent1.GetX() - 2, mountainAgent1.GetY());
            if (grassAgent != null)
                AgentQueue.Enqueue(grassAgent);
        }

        return AgentQueue;
    }


}
