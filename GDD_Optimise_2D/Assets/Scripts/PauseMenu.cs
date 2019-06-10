using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;        // Static variable so that other scripts can reference this

    public static bool ChangeSprites;               // Reference for other scripts to take
    private int spritesIntBool;                     // ChangeSprites boolean in int form

    public GameManager gameManager;
    public GameObject pauseMenu;                    // Reference to the Pause Menu group
    public GameObject optionsMenu;                  // Reference to the Options Menu group
    public GameObject pauseButton;                  // Reference to the pause button
    public GameObject restartButton;                // Reference to the restart button

    public void Start()
    {
        spritesIntBool = PlayerPrefs.GetInt("ChangeTheme", 1);
    }

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
        restartButton.SetActive(true);

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
        restartButton.SetActive(false);

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

    public void OptionsToPauseMenu()
    {
        // These set the relating groups of UI elements active or not
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    // Changes the sprite
    public void swapSprites()
    {
        // Since playerPrefs do not take in boolean, i had to resort to using int as a way to store "bool"; 0 = false, 1 = true
        if(spritesIntBool == 0)
        {
            // Sets sprite change to false
            ChangeSprites = false;
            // Sets intBool to "true", as it is an if statement
            spritesIntBool = 1;

            // Calls the function to update all the frames in GameManager
            gameManager.UpdateAllFrames();
            
            // Saves the data into PlayerPrefs
            PlayerPrefs.SetInt("ChangeTheme", spritesIntBool);

        }
        else
        {
            // Sets sprite change to true
            ChangeSprites = true;
            // Sets intBool to "false", as it is an if statement
            spritesIntBool = 0;

            // Calls the function to update all the frames in GameManager
            gameManager.UpdateAllFrames();

            // Saves the data into PlayerPrefs
            PlayerPrefs.SetInt("ChangeTheme", spritesIntBool);
        }
    }
}
