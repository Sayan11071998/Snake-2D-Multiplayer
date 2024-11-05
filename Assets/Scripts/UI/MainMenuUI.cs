using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartSinglePlayerGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StartMultiplayerPlayerGame()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}