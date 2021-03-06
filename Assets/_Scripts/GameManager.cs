using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] Image winOverlay;
    [SerializeField] Image gameoverOverlay;
    public static GameManager instance;
    public static bool WaitForContinueKey;
    [SerializeField] Transform pauseHUD;
    public static bool Paused = false;
    public static bool Won = false;
    [SerializeField] private Transform hornOverlay;
    [SerializeField] private List<Transform> woolOverlay;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameoverOverlay.enabled = false;
        instance.winOverlay.enabled = false;
        
        hornOverlay.gameObject.SetActive(false);
        woolOverlay.ForEach(t => t.gameObject.SetActive(false));
        
        Paused = false;
    }

    private void Update()
    {
        if(WaitForContinueKey && Input.anyKeyDown)
        {
            WaitForContinueKey = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(Won ? 0 : 1);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!Paused)
                TogglePause();
        }

        if(Application.isEditor && !Paused && !WaitForContinueKey)
        {
            if (Input.GetKey(KeyCode.F1))
                Time.timeScale = 4.0f;
            else
                Time.timeScale = 1.0f;
        }

        if(Paused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public static void GameOver()
    {
        instance.gameoverOverlay.enabled = true;
        Time.timeScale = 0;
        WaitForContinueKey = true;
        Won = false;
    }

    public static void Win()
    {
        Time.timeScale = 0;
        WaitForContinueKey = true;
        instance.winOverlay.enabled = true;
        Won = true;
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
        SceneManager.LoadScene(0);
    }

    public void AddPickup(InteractivePickup.InteractivePickupTypes type)
    {
        switch (type)
        {
            case InteractivePickup.InteractivePickupTypes.Horns:
                hornOverlay.gameObject.SetActive(true);
                break;
            case InteractivePickup.InteractivePickupTypes.Coconuts:
                FindObjectOfType<WolfController>().EnableFootstepSounds();
                break;
            case InteractivePickup.InteractivePickupTypes.Wool:
                if (woolOverlay.Count > 0)
                {
                    woolOverlay.Take(1).First()?.gameObject.SetActive(true);
                    woolOverlay = woolOverlay.Skip(1).ToList();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
