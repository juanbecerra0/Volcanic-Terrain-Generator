using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeshPlacer : MonoBehaviour
{
    public int vertexCount = 20;
    public int initialBlockRadius = 2; 
    //int blockCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = (GameObject) Resources.Load("Prefabs/MeshGenerator");

        if (!prefab)
        {
            Debug.Log("Prefab could not be found");
            return;
        }

        Quaternion orientation = Quaternion.identity;

        // Iterates through each sequence in the block radius
        for (int i = 1, sequenceLength = 1; i <= initialBlockRadius; i++, sequenceLength += 2)
        {
            // Iterates through each shell sequence
            for(int j = 0; j < 4; j++)
            {
                // Iterates through shell sequence lengths
                for (int k = 0; k < sequenceLength; k++)
                {
                    float xLocation = 0, zLocation = 0;

                    if (j == 0)  // Top-left shell
                    {
                        xLocation = -(vertexCount * ((sequenceLength / 2) + 1)) + (k * vertexCount) + (vertexCount / 2);
                        zLocation = (vertexCount * ((sequenceLength / 2) + 1)) - (vertexCount / 2);
                    }
                    else if (j == 1)  // Top-right shell
                    {
                        xLocation = (vertexCount * ((sequenceLength / 2) + 1)) - (vertexCount / 2);
                        zLocation = (vertexCount * ((sequenceLength / 2) + 1)) - (k * vertexCount) - (vertexCount / 2);
                    }
                    else if (j == 2)  // Bottom-right shell
                    {
                        xLocation = (vertexCount * ((sequenceLength / 2) + 1)) - (k * vertexCount) - (vertexCount / 2);
                        zLocation = -(vertexCount * ((sequenceLength / 2) + 1)) + (vertexCount / 2);
                    }
                    else if (j == 3)    // Bottom-left shell
                    {
                        xLocation = -(vertexCount * ((sequenceLength / 2) + 1)) + (vertexCount / 2);
                        zLocation = -(vertexCount * ((sequenceLength / 2) + 1)) + (k * vertexCount) + (vertexCount / 2);
                    }

                    // Calculate location
                    Vector3 position = new Vector3(xLocation, 0, zLocation);

                    // Generate instance of prefab
                    GameObject prefabInstance = (GameObject)GameObject.Instantiate(prefab, position, orientation);

                    // Generate mesh for instance
                    MeshGenerator script = prefabInstance.GetComponent<MeshGenerator>();
                    script.GenerateMesh(vertexCount);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
