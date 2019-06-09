using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private Vector3 rotationVector; //Reference for Vector for rotation.

    // Start is called before the first frame update
    void Start()
    {
        rotationVector = new Vector3(0f, 0f, 5f); //Sets the new vector once in start, rather than every frame.
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if the game is paused, if it isn't, then rotate the sprites.
        if (!PauseMenu.GameIsPaused)
        {
            transform.Rotate(rotationVector);
        }

    }
}
