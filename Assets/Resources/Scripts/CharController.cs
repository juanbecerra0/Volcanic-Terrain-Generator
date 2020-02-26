using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public float speed = 10.0f;
    public float height = 40.0f;

    private float translation;
    private float straffe;
    private bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;

        // Turn off cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Use input acis to get user input
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        UpdateKeys();

        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }

    private void UpdateKeys()
    {
        if (Input.GetKeyDown("escape"))
        {
            ToggleMouseMode();
        }

        if (Input.GetKeyDown("]"))
        {
            speed += 10.0f;
        }

        if (Input.GetKeyDown("["))
        {
            speed -= 10.0f;
        }

        if (Input.GetKeyDown("="))
        {
            height += 5.0f;
        }

        if (Input.GetKeyDown("-"))
        {
            height -= 5.0f;
        }
    }

    private void ToggleMouseMode()
    {
        if(Cursor.lockState.ToString() == "Locked")
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }

    public bool getCanMove()
    {
        return canMove;
    }
}
