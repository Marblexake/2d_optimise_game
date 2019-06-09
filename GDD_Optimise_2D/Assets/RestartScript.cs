using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScript : MonoBehaviour
{
  
    public void Restart()
    {
        // Loads the scene with the index of 1 in the hierarchy 
        SceneManager.LoadScene(1);
    }
}
