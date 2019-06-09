using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;        // Static variable so that other scripts can reference this

    public GameObject pauseMenu;                    // Reference to the Pause Menu group
    public GameObject optionsMenu;                  // Reference to the Options Menu group
    public GameObject pauseButton;                  // Reference to the pause button

    public void Update()
    {
        // This is to check if the game is supposed to be paused when starting as when quitting the game, the game is paused, and needs to be resumed again when started.
        if (!GameIsPaused)
        {
            Resume();
        }
    }

    // This method resumes the game, and is called when the resume button in the pause menu is pressed
    public void Resume()
    {
        // These set the relating groups of UI elements active or not
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        // Sets the timeScale back to 1, right back to the normal speed of time.
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    // This pauses the game, and is called when the pause button is pressed.
    public void Pause()
    {
        // These set the relating groups of UI elements active or not
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        //Sets the timeScale to 0, so that "game time" effectively stops.
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Options()
    {
        // These set the relating groups of UI elements active or not
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ToMenu()
    {
        // This needs to be here as the game has to be unpaused when quitting and starting the game again.
        GameIsPaused = false;

        // Loads the first scene in the hierarchy (the main menu basically) in build settings
        SceneManager.LoadScene(0);
    }

    public void ToPauseMenu()
    {
        // These set the relating groups of UI elements active or not
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
