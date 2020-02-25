using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MasterTerrain : MonoBehaviour
{
    private Mesh masterMesh;
    private Queue<Mesh> meshQueue;

    // Start is called before the first frame update
    void Start()
    {
        masterMesh = new Mesh();
        meshQueue = new Queue<Mesh>();
        UpdateMesh();
    }

    public void EnqueueMesh(Mesh m)
    {
        meshQueue.Enqueue(m);
    } 

    private void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = masterMesh;
    }



    // Update is called once per frame
    void Update()
    {
        // Check if queue is empty
        if (meshQueue.Count == 0)
            return;

        CombineInstance[] combine = new CombineInstance[meshQueue.Count];
        int i = 0;
        foreach (Mesh m in meshQueue)
        {
            combine[i].mesh = meshQueue.Dequeue();
            combine[i].transform = transform.localToWorldMatrix;
        }

        masterMesh.CombineMeshes(combine);
        UpdateMesh();
    }
}
