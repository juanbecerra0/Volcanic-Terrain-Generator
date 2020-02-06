using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPlacer : MonoBehaviour
{
    public int scale = 20;
    public int count = 10;

    // Start is called before the first frame update
    void Start()
    {
        GameObject prefab = (GameObject) Resources.Load("Prefabs/TerrainMeshGenerator");

        if (!prefab)
        {
            Debug.Log("Prefab could not be found\n");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {
                GameObject prefabInstance = Instantiate(prefab, new Vector3(i * scale - (scale * (count / 2)), 0, j * scale - (scale * (count / 2))), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
