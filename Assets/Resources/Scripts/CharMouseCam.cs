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

    // Line rendering variables
    private float frustDist;
    private int segments;
    private float radius;

    // Line objects
    private GameObject bottomLeftLine;
    private GameObject topLeftLine;
    private GameObject topRightLine;
    private GameObject bottomRightLine;
    private GameObject radialLine;

    // Get incremental value of mouse moving
    private Vector2 mouseLook;
    // Smooth mouse moving
    private Vector2 smoothV;

    // Flags
    private bool canTransformYView;

    // Start is called before the first frame update
    void Start()
    {
        // Setup up container and camera components
        character = this.transform.parent.gameObject;
        charCamera = this.GetComponentsInChildren<Camera>()[0];
        overviewCamera = this.GetComponentsInChildren<Camera>()[1];
        canTransformYView = true;

        // Sets up camera line renderers
        void SetupLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.startColor = Color.blue;
            render.endColor = Color.blue;
            render.startWidth = 0.2f;
            render.endWidth = 0.2f;
        }

        void SetupRadialLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.positionCount = segments + 1;
            render.useWorldSpace = false;
            render.startColor = Color.blue;
            render.endColor = Color.blue;
            render.startWidth = 0.2f;
            render.endWidth = 0.2f;
        }

        // Set up line variables
        frustDist = 200.0f;
        segments = 180;
        radius = 150.0f;

        // Create and setup camera lines
        bottomLeftLine = new GameObject("Bottom-Left Frust Line");
        topLeftLine = new GameObject("Top-Left Frust Line");
        topRightLine = new GameObject("Top-Right Frust Line");
        bottomRightLine = new GameObject("Bottom-Right Frust Line");
        radialLine = new GameObject("Radial Line");

        SetupLine(bottomLeftLine);
        SetupLine(topLeftLine);
        SetupLine(topRightLine);
        SetupLine(bottomRightLine);
        SetupRadialLine(radialLine);
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

        // Toggle overview camera
        if (Input.GetKeyDown("tab"))
        {
            ToggleCamera();
        }

        // Zoom overview camera in or out
        if (!canTransformYView)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
            {
                overviewCamera.transform.localPosition = new Vector3(
                    overviewCamera.transform.localPosition.x,
                    overviewCamera.transform.localPosition.y - 5.0f,
                    overviewCamera.transform.localPosition.z
                );
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
            {
                overviewCamera.transform.localPosition = new Vector3(
                    overviewCamera.transform.localPosition.x,
                    overviewCamera.transform.localPosition.y + 5.0f,
                    overviewCamera.transform.localPosition.z
                );
            }
        }
    }

    // Casts out camera frustum rays. Used in generating new blocks
    private void CastCameraRay()
    {
        // Renders a single line from the character origin to the specified end point
        void RenderLine(GameObject line, Vector3 endPoint)
        {
            line.transform.position = transform.position;

            line.GetComponent<LineRenderer>().SetPositions(new Vector3[] {
                transform.position,
                endPoint
            });
        }

        void RenderCircle(GameObject line)
        {
            line.transform.position = transform.position;

            for(int i = 0; i < segments + 1; i++)
            {
                float rad = Mathf.Deg2Rad * (i * 360f / segments);
                line.GetComponent<LineRenderer>().SetPosition(i, new Vector3(
                    Mathf.Sin(rad) * radius,
                    0,
                    Mathf.Cos(rad) * radius
                ));
            }

        }

        // Render lines based of the four frustum rays
        RenderLine(bottomLeftLine, charCamera.ViewportPointToRay(new Vector3(0, 0, 0)).GetPoint(frustDist));
        RenderLine(topLeftLine, charCamera.ViewportPointToRay(new Vector3(0, 1, 0)).GetPoint(frustDist));
        RenderLine(topRightLine, charCamera.ViewportPointToRay(new Vector3(1, 1, 0)).GetPoint(frustDist));
        RenderLine(bottomRightLine, charCamera.ViewportPointToRay(new Vector3(1, 0, 0)).GetPoint(frustDist));
        RenderCircle(radialLine);
    }
}
