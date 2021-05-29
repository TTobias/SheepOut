using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Image gameoverOverlay;
    public static GameManager instance;
    public static bool WaitForContinueKey;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameoverOverlay.enabled = false;
    }

    private void Update()
    {
        if(WaitForContinueKey && Input.anyKeyDown)
        {
            WaitForContinueKey = false;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }

    public static void GameOver()
    {
        instance.gameoverOverlay.enabled = true;
        Time.timeScale = 0;
        WaitForContinueKey = true;
    }
}
