using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuNav : MonoBehaviour
{
    private AsyncOperation sceneLoad;       // Reference to the "operation" of loading the scene.
    public GameObject loadingBar;           // Reference to the loading bar object
    public Slider loadingSlider;            // Reference to the loading slider UI element

    public GameObject mainMenu;             // Reference to the main menu UI element group
    public GameObject optionsMenu;          // Reference to the Options menu UI element group


    public void GameStart(int sceneIndex)
    {
        // Starts the Async load coroutine.
        StartCoroutine(Loading(sceneIndex));

        // Sets the relevant UI elements active and inactive.
        loadingBar.SetActive(true);
        mainMenu.SetActive(false);
    }

    IEnumerator Loading(int sceneIndex)
    {
        // Loads the next scene asynchronously
        sceneLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Loops while the loading of the next scene is not done.
        while (!sceneLoad.isDone)
        {
            // This forces the loading value to be able to hit 1f as the originally the value always stops at 0.9f
            float progress = Mathf.Clamp01(sceneLoad.progress / .9f);

            // Updates the loading bar progress visually
            loadingSlider.value = progress;

            // Waits for the next frame.
            yield return null;
        }
    }

    public void ToMainMenu()
    {
        // These set the relating groups of UI elements active or not
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void ToOptions()
    {
        // These set the relating groups of UI elements active or not
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }


}
