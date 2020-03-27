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
    private float lineWidth;
    private Color lineColor;
    private float frustDist;
    private int segments;
    private float radius;

    // Line objects
    private GameObject leftLine;
    private GameObject rightLine;
    private GameObject centerLine;
    private GameObject radialLine;

    // Get incremental value of mouse moving
    private Vector2 mouseLook;
    // Smooth mouse moving
    private Vector2 smoothV;

    // Flags
    private bool canTransformYView;

    // Procedural generation scripts/variables
    HeightmapGenerator hgScript;
    MeshPlacer mpScript;
    private int linePoints;
    private int circlePoints;

    // Start is called before the first frame update
    void Start()
    {
        // Setup up variables
        character = this.transform.parent.gameObject;
        charCamera = this.GetComponentsInChildren<Camera>()[0];
        overviewCamera = this.GetComponentsInChildren<Camera>()[1];
        
        hgScript = GameObject.FindObjectOfType(typeof(HeightmapGenerator)) as HeightmapGenerator;
        mpScript = GameObject.FindObjectOfType(typeof(MeshPlacer)) as MeshPlacer;
        linePoints = 3;
        circlePoints = 8;

        canTransformYView = true;

        // Sets up camera line renderers
        void SetupLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.startColor = lineColor;
            render.endColor = lineColor;
            render.startWidth = lineWidth;
            render.endWidth = lineWidth;
            render.enabled = false;
        }

        void SetupRadialLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.positionCount = segments + 1;
            render.useWorldSpace = false;
            render.startColor = lineColor;
            render.endColor = lineColor;
            render.startWidth = lineWidth;
            render.endWidth = lineWidth;
            render.enabled = false;
        }

        // Set up line variables
        lineWidth = 1.0f;
        lineColor = Color.red;
        frustDist = 200.0f;
        segments = 180;
        radius = 150.0f;

        // Create and setup camera lines
        leftLine = new GameObject("Left Line");
        rightLine = new GameObject("Right Line");
        centerLine = new GameObject("Center Line");
        radialLine = new GameObject("Radial Line");

        SetupLine(leftLine);
        SetupLine(rightLine);
        SetupLine(centerLine);
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

        CastCameraRays();

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

                leftLine.GetComponent<LineRenderer>().enabled = true;
                rightLine.GetComponent<LineRenderer>().enabled = true;
                centerLine.GetComponent<LineRenderer>().enabled = true;
                radialLine.GetComponent<LineRenderer>().enabled = true;

                canTransformYView = false;
            } else
            {
                charCamera.depth = 1;
                overviewCamera.depth = 0;

                leftLine.GetComponent<LineRenderer>().enabled = false;
                rightLine.GetComponent<LineRenderer>().enabled = false;
                centerLine.GetComponent<LineRenderer>().enabled = false;
                radialLine.GetComponent<LineRenderer>().enabled = false;

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
    private void CastCameraRays()
    {
        // Running list of cartesian coordinate spaces to possibly render
        List<(int, int)> coordList = new List<(int, int)>();

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

        // Calculate rays
        Ray leftRay = new Ray(transform.position, (charCamera.ViewportPointToRay(new Vector3(0, 0, 0)).direction + charCamera.ViewportPointToRay(new Vector3(0, 1, 0)).direction) / 2);
        Ray rightRay = new Ray(transform.position, (charCamera.ViewportPointToRay(new Vector3(1, 1, 0)).direction + charCamera.ViewportPointToRay(new Vector3(1, 0, 0)).direction) / 2);
        Ray centerRay = new Ray(transform.position, (leftRay.direction + rightRay.direction) / 2);

        // Render lines based of the four frustum rays
        // Also render circle around player
        RenderLine(
            leftLine, 
            leftRay.GetPoint(frustDist)
        );
        RenderLine(
            rightLine, 
            rightRay.GetPoint(frustDist)
        );
        RenderLine(
            centerLine, 
            centerRay.GetPoint(frustDist)
        );
        RenderCircle(radialLine);

        // Finally, use current line renders to detect if new blocks must be generated
        ProjectAndFillBlocks(coordList);
    }

    private void ProjectAndFillBlocks(List<(int, int)> coordList)
    {
        // TODO hgScript.IsEmpty(0,0)
        for(int i = 0; i < coordList.Count; i++)
        {
            // If cartesian coordinate space is empty, generate new block instance
            if (hgScript.IsEmpty(coordList[i].Item1, coordList[i].Item2))
                mpScript.GenerateBlockInstance(coordList[i].Item1, coordList[i].Item2);
        }
    }
}
