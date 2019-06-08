using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToOptions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;

    public void ToOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
}
