using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private Button startButton;
    private Button exitButton;
    private Text loading;

    private void Start()
    {
        startButton = GetComponentsInChildren<Button>()[0];
        exitButton = GetComponentsInChildren<Button>()[1];
        loading = GetComponentsInChildren<Text>(true)[6];

        startButton.onClick.AddListener(StartButtonPressed);
        exitButton.onClick.AddListener(ExitButtonPressed);
    }
    
    private void StartButtonPressed()
    {
        loading.color = new Color(1f, 1f, 1f);
        SceneManager.LoadScene(1);
    }

    private void ExitButtonPressed()
    {
        Application.Quit();
    }
}
