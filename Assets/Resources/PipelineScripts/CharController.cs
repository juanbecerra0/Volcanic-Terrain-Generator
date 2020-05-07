using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public float speed;
    public float height;

    private float translation;
    private float straffe;

    // Start is called before the first frame update
    void Start()
    {
        // Turn off cursor
        Cursor.lockState = CursorLockMode.Locked;
        height = 120000.0f;
        speed = 60000f;
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
        void ToggleMouseMode()
        {
            if (Cursor.lockState.ToString() == "Locked")
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown("escape"))
        {
            ToggleMouseMode();
        }

        if (Input.GetKeyDown("]"))
        {
            speed += 10000.0f;
        }

        if (Input.GetKeyDown("["))
        {
            speed -= 10000.0f;
        }

        if (Input.GetKeyDown("="))
        {
            height += 20000.0f;
        }

        if (Input.GetKeyDown("-"))
        {
            height -= 20000.0f;
        }
    }
}
