using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterTerrain : MonoBehaviour
{
    // Material/shader applied to all block objects
    private Material SeamBlendMaterial;

    // Start is called before the first frame update
    void Start()
    {
        SeamBlendMaterial = (Material) Resources.Load("Shaders/SeamBlendMaterial");
    }

    // Update a new child component's material to SeamBlendMaterial/Shader
    public void UpdateMaterial(int childIndex)
    {
        GameObject thisChild = transform.GetChild(childIndex).gameObject;
        thisChild.GetComponent<MeshRenderer>().material = SeamBlendMaterial;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
