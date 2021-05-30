using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image winOverlay;
    [SerializeField] Image gameoverOverlay;
    public static GameManager instance;
    public static bool WaitForContinueKey;
    [SerializeField] Transform pauseHUD;
    public static bool Paused = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameoverOverlay.enabled = false;
        instance.winOverlay.enabled = false;
        Paused = false;
    }

    private void Update()
    {
        if(WaitForContinueKey && Input.anyKeyDown)
        {
            WaitForContinueKey = false;
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if(Application.isEditor && !Paused && !WaitForContinueKey)
        {
            if (Input.GetKey(KeyCode.F1))
                Time.timeScale = 4.0f;
            else
                Time.timeScale = 1.0f;
        }
    }

    public static void GameOver()
    {
        instance.gameoverOverlay.enabled = true;
        Time.timeScale = 0;
        WaitForContinueKey = true;
    }

    public static void Win()
    {
        Time.timeScale = 0;
        WaitForContinueKey = true;
        instance.winOverlay.enabled = true;
    }

    public void TogglePause()
    {
        Paused = !Paused;

        if (Paused)
        {
            Time.timeScale = 0;
            pauseHUD.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            pauseHUD.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
