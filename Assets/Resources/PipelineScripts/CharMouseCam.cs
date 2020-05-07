using System;
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
    private float frustDist = 500000.0f;
    private int segments = 90;
    private float radius = 380000.0f;

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
    MapDatabase MapDatabaseScript;
    MasterGen MasterGenScript;
    private int blockSize;

    // Start is called before the first frame update
    void Start()
    {
        // Setup up variables
        character = this.transform.parent.gameObject;
        charCamera = this.GetComponentsInChildren<Camera>()[0];
        overviewCamera = this.GetComponentsInChildren<Camera>()[1];

        MapDatabaseScript = GameObject.FindObjectOfType(typeof(MapDatabase)) as MapDatabase;
        MasterGenScript = GameObject.FindObjectOfType(typeof(MasterGen)) as MasterGen;
        blockSize = MasterGenScript.block_VertexWidth;

        canTransformYView = true;

        // Sets up camera line renderers
        void SetupLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.startWidth = lineWidth;
            render.endWidth = lineWidth;
            render.enabled = false;

            line.transform.parent = this.transform;
        }

        void SetupRadialLine(GameObject line)
        {
            line.AddComponent<LineRenderer>();

            LineRenderer render = line.GetComponent<LineRenderer>();

            render.positionCount = segments + 1;
            render.useWorldSpace = false;
            render.startWidth = lineWidth;
            render.endWidth = lineWidth;
            render.enabled = false;

            line.transform.parent = this.transform;
        }

        // Set up line variables
        lineWidth = overviewCamera.transform.position.y / 50f;

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

        void UpdateLineWidth()
        {
            float newLineWidth = overviewCamera.transform.position.y / 50f;

            leftLine.GetComponent<LineRenderer>().startWidth = newLineWidth;
            leftLine.GetComponent<LineRenderer>().endWidth = newLineWidth;
            rightLine.GetComponent<LineRenderer>().startWidth = newLineWidth;
            rightLine.GetComponent<LineRenderer>().endWidth = newLineWidth;
            centerLine.GetComponent<LineRenderer>().startWidth = newLineWidth;
            centerLine.GetComponent<LineRenderer>().endWidth = newLineWidth;
            radialLine.GetComponent<LineRenderer>().startWidth = newLineWidth;
            radialLine.GetComponent<LineRenderer>().endWidth = newLineWidth;
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
                    overviewCamera.transform.localPosition.y - 1000.0f,
                    overviewCamera.transform.localPosition.z
                );
                UpdateLineWidth();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
            {
                overviewCamera.transform.localPosition = new Vector3(
                    overviewCamera.transform.localPosition.x,
                    overviewCamera.transform.localPosition.y + 1000.0f,
                    overviewCamera.transform.localPosition.z
                );
                UpdateLineWidth();
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

            for (float i = 0f; i <= 1.0f; i += 0.02f)
            {
                // Translate coordinate into simplified integer coordinate system
                Tuple<int, int> coordinate = new Tuple<int, int>(
                    Convert.ToInt32((Mathf.Lerp(transform.position.x, endPoint.x, i) - (blockSize / 2)) / blockSize),
                    Convert.ToInt32((Mathf.Lerp(transform.position.z, endPoint.z, i) - (blockSize / 2)) / blockSize)
                );

                // Enqueue coordinate into list if it does not contain it
                if (!coordList.Contains((coordinate.Item1, coordinate.Item2)))
                    coordList.Add((coordinate.Item1, coordinate.Item2));
            }
            line.transform.rotation = Quaternion.identity;
        }

        void RenderCircle(GameObject line)
        {
            line.transform.position = transform.position;

            for(int i = 0; i < segments + 1; i++)
            {
                float rad = Mathf.Deg2Rad * (i * 360f / segments);

                float xPoint = Mathf.Sin(rad) * radius;
                float zPoint = Mathf.Cos(rad) * radius;

                line.GetComponent<LineRenderer>().SetPosition(i, new Vector3(
                    xPoint,
                    0,
                    zPoint
                ));

                    // Translate this point in the radial line to world coordinates
                    Vector3 worldPoint = transform.position + (new Vector3(xPoint, 0, zPoint));

                    // Translate coordinate into simplified integer coordinate system
                    Tuple<int, int> coordinate = new Tuple<int, int>(
                        Convert.ToInt32((worldPoint.x - (blockSize / 2)) / blockSize),
                        Convert.ToInt32((worldPoint.z - (blockSize / 2)) / blockSize)
                    );

                    // Enqueue coordinate into list if it does not contain it
                    if (!coordList.Contains((coordinate.Item1, coordinate.Item2)))
                        coordList.Add((coordinate.Item1, coordinate.Item2));

            }

            line.transform.rotation = Quaternion.identity;
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
        for(int i = 0; i < coordList.Count; i++)
        {
            // If cartesian coordinate space is empty, generate new block instance
            if (MapDatabaseScript.IsVacent(coordList[i].Item1, coordList[i].Item2))
                MasterGenScript.GenerateBlockInstance(coordList[i].Item1, coordList[i].Item2);
        }
    }
}
