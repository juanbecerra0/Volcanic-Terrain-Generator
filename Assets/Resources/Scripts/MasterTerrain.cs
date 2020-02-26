using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MasterTerrain : MonoBehaviour
{
    private Mesh masterMesh;
    private Queue<Mesh> meshQueue;

    public void Initialize()
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
        masterMesh.RecalculateBounds();
        masterMesh.RecalculateNormals();
        masterMesh.Optimize();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if queue is empty
        if (meshQueue.Count == 0)
            return;

        CombineInstance[] combine = new CombineInstance[meshQueue.Count];
        int i = 0;

        while(meshQueue.Count != 0)
        {
            combine[i].mesh = meshQueue.Dequeue();
            combine[i].transform = transform.localToWorldMatrix;
            i++;
        }

        masterMesh.CombineMeshes(combine);
        UpdateMesh();
    }
}
