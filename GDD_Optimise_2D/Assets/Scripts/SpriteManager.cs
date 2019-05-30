using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private Vector3 rotationVector;

    // Start is called before the first frame update
    void Start()
    {
        rotationVector = new Vector3(0f, 0f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationVector);
    }
}
