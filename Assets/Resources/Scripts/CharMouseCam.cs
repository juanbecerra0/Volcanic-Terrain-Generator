using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMouseCam : MonoBehaviour
{
    [SerializeField]
    public float sensitivity = 5.0f;
    [SerializeField]
    public float smoothing = 2.0f;

    // Character components
    public GameObject character;
    private Camera charCamera;
    private Camera overviewCamera;

    // Get incremental value of mouse moving
    private Vector2 mouseLook;
    // Smooth mouse moving
    private Vector2 smoothV;

    // Flags
    private bool canTransformYView;

    // Start is called before the first frame update
    void Start()
    {
        character = this.transform.parent.gameObject;
        charCamera = this.GetComponentsInChildren<Camera>()[0];
        overviewCamera = this.GetComponentsInChildren<Camera>()[1];

        canTransformYView = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse delta
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        // Interpolated float result between the two float values
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);

        if (canTransformYView)
        {
            smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
            mouseLook += smoothV;
        } else
        {
            smoothV.y = 0.0f;
            mouseLook.x += smoothV.x;
            mouseLook.y = 0.0f;
        }

        // x-axis = vec3.right
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);

        CastCameraRay();
        UpdateKeys();
    }

    private void UpdateKeys()
    {
        void ToggleCamera()
        {
            transform.localRotation = Quaternion.identity;
            character.transform.localRotation = Quaternion.identity;

            if(canTransformYView)
            {
                charCamera.depth = 0;
                overviewCamera.depth = 1;
                canTransformYView = false;
            } else
            {
                charCamera.depth = 1;
                overviewCamera.depth = 0;
                canTransformYView = true;
            }
        }

        if (Input.GetKeyDown("tab"))
        {
            ToggleCamera();
        }
    }

    // Casts out camera frustum rays. Used in generating new blocks
    // TODO may be used to optimize culled geometry rendering
    private void CastCameraRay()
    {
        // Get frustrum rays
        Ray bottomLeft = charCamera.ViewportPointToRay(new Vector3(0, 0, 0));
        Ray topLeft = charCamera.ViewportPointToRay(new Vector3(0, 1, 0));
        Ray topRight = charCamera.ViewportPointToRay(new Vector3(1, 1, 0));
        Ray bottomRight = charCamera.ViewportPointToRay(new Vector3(1, 0, 0));

        
    }
}
