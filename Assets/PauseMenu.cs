using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject PauseMenuUI;
    private float previousTimeScale;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else {Pause();}
        }
    }
    
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale=previousTimeScale;
        gameIsPaused=false;
    }
    
    void Pause()
    {
        PauseMenuUI.SetActive(true);
        previousTimeScale = Time.timeScale;
        Time.timeScale=0f;
        gameIsPaused=true;
    }

    public void LoadMenu()
    {
        Time.timeScale=previousTimeScale;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
   
}