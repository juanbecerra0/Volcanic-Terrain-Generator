using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharController : MonoBehaviour
{
    private float speed;
    private float height;

    private float IncrementSpeed;
    private float IncrementHeight;

    private float translation;
    private float straffe;

    public void Init(float startSpeed, float startHeight, float incrementSpeed, float incrementHeight)
    {
        Cursor.lockState = CursorLockMode.Locked;
        speed = startSpeed;
        height = startHeight;
        IncrementSpeed = incrementSpeed;
        IncrementHeight = incrementHeight;
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
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown("]"))
        {
            speed += IncrementSpeed;
        }

        if (Input.GetKeyDown("["))
        {
            speed -= IncrementSpeed;
        }

        if (Input.GetKeyDown("="))
        {
            height += IncrementHeight;
        }

        if (Input.GetKeyDown("-"))
        {
            height -= IncrementHeight;
        }
    }
}
