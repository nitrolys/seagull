using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public void StartingGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void EndingGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}