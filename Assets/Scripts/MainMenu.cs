using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main menu functions and difficulty
/// </summary>
public class MainMenu : MonoBehaviour
{
    ///<para>1=easy, 2=medium, 3=hard</para>
    public static int difficulty = 2;

    ///<summary>
    /// Loads up the main scene in the selected difficulty
    ///</summary>
    /// <param name="choice"> Difficulty chosen </param>
    public void PlayGame(int choice)
    {
        difficulty = choice;
        SceneManager.LoadScene("Main Scene");
    }

    ///<summary>
    /// Quits the game
    ///</summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}