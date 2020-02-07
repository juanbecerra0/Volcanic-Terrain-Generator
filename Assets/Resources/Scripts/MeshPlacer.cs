using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPlacer : MonoBehaviour
{
    public int vertexCount = 20;
    public int blockCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = (GameObject) Resources.Load("Prefabs/MeshGenerator");

        if (!prefab)
        {
            Debug.Log("Prefab could not be found");
            return;
        }

        if (blockCount % 2 != 0)
        {
            blockCount--;
        }

        for (int i = 0; i < blockCount; i++)
        {
            for (int j = 0; j < blockCount; j++)
            {
                Vector3 position = new Vector3(i * vertexCount - (vertexCount * (blockCount / 2)), 0, j * vertexCount - (vertexCount * (blockCount / 2)));
                Quaternion orientation = Quaternion.identity;

                GameObject prefabInstance = (GameObject) GameObject.Instantiate(prefab, position, orientation);

                MeshGenerator script = prefabInstance.GetComponent<MeshGenerator>();
                script.GenerateMesh(vertexCount);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
