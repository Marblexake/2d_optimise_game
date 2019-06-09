using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsToMainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;

    public void ToMainMenu()
    {
        // These set the relating groups of UI elements active or not
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
