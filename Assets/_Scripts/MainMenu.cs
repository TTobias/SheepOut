using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject creditsOverlay;


    // Start is called before the first frame update
    void Start()
    {
        creditsOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {
        creditsOverlay.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsOverlay.SetActive(false);
    }
}
