using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the pause menu functions
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    ///<summary>
    /// Resumes the game
    ///</summary>
    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        TimeController.UnpauseTime();
        gameIsPaused = false;
    }

    ///<summary>
    /// Pauses the game
    ///</summary>
    void Pause()
    {
        PauseMenuUI.SetActive(true);
        TimeController.PauseTime();
        gameIsPaused = true;
    }

    ///<summary>
    /// Goes to the Main Menu
    ///</summary>
    public void LoadMenu()
    {
        Resume();
        SceneManager.LoadScene("Main Menu");
    }

    ///<summary>
    /// Quits the game
    ///</summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    ///<summary>
    /// Restarts the scene
    ///</summary>
    public void Restart()
    {
        SceneManager.LoadScene("Main Scene");
    }
}