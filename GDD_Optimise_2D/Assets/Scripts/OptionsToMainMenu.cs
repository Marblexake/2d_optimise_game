using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsToMainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;

    public void ToMainMenu()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
