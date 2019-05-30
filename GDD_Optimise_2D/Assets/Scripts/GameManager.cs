﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject framePrefab;                  // Used to create a new frame of sprites
    public float speed = -5f;
    public TextMeshProUGUI scoreText;               // Increments when a mirrored frame is selected
    public TextMeshProUGUI timeText;                // Updates every second
    public ParticleSystem successParticles;         // Particle system to run when a match is correct
    public ParticleSystem failParticles;            // Particle system to run when a match is wrong
    public GameObject mouse;                        // Background sprite that oscillates left to right 
    public GameObject rabbit;                       // Background sprite that oscillates left to right 
    public GameObject panda;                        // Background sprite that oscillates left to right 
    
    //***********************************************************************************************************************
    // Changes:
    public Object[] sprites;                        // This is the array in which all the sprites/ is kept
    //private Camera mainCam;

    // PlaySound() variables;
    private AudioSource audioSource;                // AudioSource Component that will be added to the GameObject on Start()
    private AudioClip clipToBePlayed;               // Clip that will be played 
    private AudioClip clipCorrect;                  // AudioClip for the CORRECT sound
    private AudioClip clipWrong;                    // AudioClip for the WRONG sound

    private GameObject hitFrame;
    private Vector3 mousePos;

    //***********************************************************************************************************************


    public float gameWidth { get; private set; }    // Width of the game view
    public float gameHeight { get; private set; }   // Height of the game view

    // When a frame moves left, once it reaches leftExtent it will be destroyed
    //
    private float leftExtent;            
    
    // When frames are created at the start of the game, they are placed from left
    // to right. When the frames reach rightExtent on the X axis, no more frames
    // are created, and the game can start.
    //
    private float rightExtent;                      

    // The background sprites oscillate from left to right continuously. The A and B
    // positions are the extreme left and right X positions of the oscillation for
    // each sprite
    //
    private Vector3 bk1PositionA, bk1PositionB;
    private Vector3 bk2PositionA, bk2PositionB;
    private Vector3 bk3PositionA, bk3PositionB;

    private float frameWidth;               // The width of a frame of sprites
    private List<GameObject> framesList;        // Created frames are stored in this list
    private bool initialised = false;       // When initialised is true, the game can start

    int score = 0;
    int seconds = 0;
    float counterTime = 0;                  // Used for the Seconds UI display
    float backgroundTime = 0;               // Used for the background sprites oscillation

    //GameObject mouse, rabbit, panda;        // The oscillating background sprites
    bool showBackgroundCharacters = false;  // The background sprites are shown intermittently, i.e. on/off

    // When a new frame is created, it must be placed behind the rightmost, or end, frame. endFrame
    // is the rightmost frame. This is updated every time a new frame is created and placed at the end.
    private GameObject endFrame;

    private void Start()
    {
        // Changes: Placing all the images into an array right at the start of the game
        sprites = Resources.LoadAll("Images/animals", typeof(Sprite));

        // Changes: The audio source will now be added to the GameObject GameManager at the start instead of each and every time a
        //          sound is played.
        audioSource = gameObject.AddComponent<AudioSource>();
        // Changes: Loaded the clips once in Start()
        clipCorrect = (AudioClip)Resources.Load("Audio/CORRECT");
        clipWrong = (AudioClip)Resources.Load("Audio/WRONG");



        // Changes: Added a camera.main reference so that a reference of it isn't created like previously
        //mainCam = Camera.main;
        // Find the height and width of the game view in world units
        //
        gameHeight = Camera.main.orthographicSize * 2f;
        gameWidth = Camera.main.aspect * gameHeight;

        // Calculate the X axis values for frame removal and the positioning of new frames
        //
        leftExtent = -gameWidth*1.3f;
        rightExtent = gameWidth*1.3f;

        // Get a reference to each background sprite for oscillation
        //
        //mouse = GameObject.Find("MouseBackground");
        //rabbit = GameObject.Find("RabbitBackground");
        //panda = GameObject.Find("PandaBackground");

        // Calculate the left and right X axis values for the background sprite oscillations.
        // This is set to be 5 units to the left and right of each sprite's default position.
        //
        bk1PositionA = new Vector3(mouse.transform.position.x - 5, mouse.transform.position.y, mouse.transform.position.z);
        bk1PositionB = new Vector3(mouse.transform.position.x + 5, mouse.transform.position.y, mouse.transform.position.z);
        bk2PositionA = new Vector3(rabbit.transform.position.x - 5, rabbit.transform.position.y, rabbit.transform.position.z);
        bk2PositionB = new Vector3(rabbit.transform.position.x + 5, rabbit.transform.position.y, rabbit.transform.position.z);
        bk3PositionA = new Vector3(panda.transform.position.x - 5, panda.transform.position.y, panda.transform.position.z);
        bk3PositionB = new Vector3(panda.transform.position.x + 5, panda.transform.position.y, panda.transform.position.z);

        // Hide the oscillating sprites. These will be switched on and off at regular intervals
        //
        mouse.SetActive(false);
        rabbit.SetActive(false);
        panda.SetActive(false);

        framesList = new List<GameObject>();

        // Create all the frames for the start of the game.
        //
        // Set the current X position to leftExtent, then work from left to right placing new frames
        // one after the other along the X axis until rightExtent is reached.
        //
        // This is how it works:
        //
        //  leftExtent                                        rightExtent
        //      frame frame frame frame frame frame frame frame frame  
        //
        int n = 0;
        float currX = leftExtent;
        while (currX < rightExtent)
        {
            Vector3 currPos = new Vector3(currX, 0f, 0f);
            GameObject frame = CreateFrame();
            frame.name = "Frame_" + n++;

            frame.transform.position = currPos;

            // The box collider has been sized to fit the boundaries of the sprite in the Frame prefab.
            //
            frameWidth = frame.GetComponent<BoxCollider>().bounds.size.x;

            // The gap between one frame and the next: |     |<-gap->|     |
            float gap = frameWidth * 0.1f;

            currX += frameWidth + gap;
            framesList.Add(frame);
        }
        endFrame = framesList[framesList.Count-1];  // endFrame is the last frame added to the list
        initialised = true;                 // Set to true so that the game can start
    }

    // Creates a new frame. This is done when all the initial frames are created before the
    // game starts, and also when a frame is destroyed after it moves past leftExtent and a
    // new frame is created to take its place.
    //
    private GameObject CreateFrame()
    {
        // Instantiate a new frame from the frame prefab. Note that the frame prefab has 
        // pre-existing top and bottom sprites already. These default pre-existing sprites  
        // must be replaced by new sprites in the for loop below. 
        //
        // The reason for the frame prefab having pre-existing default sprites is to make 
        // it easier to position the new sprites when they are created. The new sprite 
        // positions are simply set to the pre-exisiting default sprite positions, and 
        // then the default sprites are destroyed. 
        //
        // HINT: What problem is being solved here, and is this a good way to solve it?
        //
        GameObject newFrame = Instantiate(framePrefab);

        // Each frame has a set of top and bottom sprites. All the top and bottom sprites must 
        // match to score a point. The top and bottom sprites are children of a top or a
        // bottom empty parent gameobject, which in turn are children of the frame gameobject.
        // The structure is:
        // 
        // frame
        //     top
        //         s0 s1 s2 s3 s4 s5
        //     bottom
        //         s0 s1 s2 s3 s4 s5

        // Get a reference to the top parent
        //
        GameObject top = newFrame.gameObject.transform.GetChild(0).gameObject;

        // Get the number of children of top. This is the number of sprites for the top part
        // of the frame. 
        //
        int numChildren = top.transform.childCount; //Always returns 6, maybe it can be called in awake instead?

        // Loop across all the top children in the new frame
        //
        for (int i = 0; i < numChildren; i++)
        {
            // Each sprite gameobject in the new frame must be replaced with a new sprite gameobject.
            // Choose a random sprite from the sprites array.
            //
            int randomIndex = Random.Range(0, sprites.Length); //Move this out of the loop 
            top.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)sprites[randomIndex];

        }

        // Now we replace the default bottom sprites with new sprites
        //
        GameObject bottom = newFrame.gameObject.transform.GetChild(1).gameObject;

        // Get a random number between 1 and 10. For integers, the Random.Range function is not
        // inclusive of the max value argument, which is why it is 11.
        //
        int rand = Random.Range(1, 11);

        // Check the value of the random number. If it is >=5, then set this new frame to be
        // mirrored (top and bottom sprites are the same). If rand is <=4, then set the frame
        // to be unmirrored. If the frame is mirrored, then the each bottom sprite will be a
        // copy of its corresponding top sprite.
        //
        bool mirror = rand > 0 ? true : false;

        // Loop over all the bottom sprites
        //
        for (int i = 0; i < numChildren; i++)
        {            
            // If the frame is set to mirrored, create a copy of the corresponding top sprite. 
            // IMPORTANT! Even though the old default sprites have been "destroyed" in the code
            // above, they won't actually be destroyed until the end of the current frame.
            // At this point in the code, they still exist! The top parent's children look like 
            // this, with their child numbers indicated:
            //
            //  top
            //      old old old old old old new new new new new new
            //       0   1   2   3   4   5   6   7   8   9   10  11
            //
            // So, to get the first new sprite, we have to use index 0 + numChildren, which is 6.
            //
            if (mirror)
            {
                //sprite = Instantiate(top.transform.GetChild(i + numChildren).gameObject);

                bottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = top.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                // The frame is not mirrored, so the bottom sprites must be different from the top
                // sprites.
                //
                int randomIndex = Random.Range(0, sprites.Length);
                bottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)sprites[randomIndex];
            }
        }      
        // Return the newly created frame back to the calling code
        //
        return newFrame;
    }

    // Animate the frames from right to left. Each frame has a RigidBody component, which is
    // used to move it across the screen. RigidBody is used instead of RigidBody2D because 
    // 2D physics sometimes has bugs. Each frame also has a BoxCollider on it. The BoxCollider
    // is used to check for player interaction.
    //
    private void MoveFrames()
    {
        // Loop over all the frames in the frames list
        //
        foreach (GameObject frame in framesList)
        {
            // If a mirrored frame is selected, it will be destroyed. This leaves a gap in the 
            // row of frames. All the frames to the right should speed up to close the gap. So
            // for each frame, find the distance to the frame on its left. If the distance is
            // greater than half a framewidth, increase the frame's animation speed.
            //
            float distance = GetDistanceToNeighbour(frame);

            if (distance > frameWidth * 0.5f)
            {
                frame.GetComponent<Rigidbody>().velocity = new Vector3(speed * 10f, 0f, 0f);
            }
            else
            {
                frame.GetComponent<Rigidbody>().velocity = new Vector3(speed, 0f, 0f);
            }
        }

        // Now check if any of the frames have gone past leftExtent on the X axis. If so, they
        // must be destroyed and a new frame created at the end of the row of frames
        //
        CheckRespawnFrames();
    }

    // Returns the distance from a frame to its left neighbouring frame. This is done by casting a
    // ray from the frame's position towards the left. The distance between the frames' x position
    // and the x position of the hit object is calculated.
    //
    float GetDistanceToNeighbour(GameObject frame)
    {
        // Get the width of the frame
        //
        float frameWidth = frame.gameObject.GetComponent<BoxCollider>().bounds.size.x;

        float distance = 0f;

        // Set up the ray parameters
        //
        Vector3 pos = frame.gameObject.transform.position;
        Vector3 raypos = new Vector3(pos.x - frameWidth, 0f, pos.z);
        RaycastHit hit;

        // Cast the ray. Check for a collision, then check if the collided object's name 
        // starts with "Frame". If so, them set distance to the hit distance.
        //
        if (Physics.Raycast(raypos, Vector3.left, out hit, gameWidth*2f))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.StartsWith("Frame"))
                {
                    distance = hit.distance;
                }
            }
        }
        else
        {
            // There is no hit. This means that the frame has no left neighbour. In this
            // case, check if it is the leftmost frame. If it is, and if there is a gap
            // between the frame and the left side of the game view, then the frame must 
            // be speeded up to close the gap. So return a large value for distance, in
            // this case twice the width of the game view.
            //
            // HINT: Why is this function doing two different things?
            //
            bool leftFrame = true;
            foreach(GameObject f in framesList)
            {
                if(frame.gameObject.transform.position.x > f.gameObject.transform.position.x)
                {
                    leftFrame = false;
                }
            }
            if (leftFrame == true && frame.gameObject.transform.position.x > -gameWidth/2f)
            {
                distance = gameWidth * 2f; ;
            }
        }

        return distance;
    }

    // Check if a frame needs to be destroyed and a new one spawned to take its place. 
    // Loop through all the frames in the frames list. If any frame's X position is 
    // less than leftExtent it must be destroyed. We must loop backwards over the list,
    // since C# doesn't allow a list element to be removed when iterating over it from
    // front to back (this is because all the remaining list elements will be shuffled
    // up in the list, which messes up the iteration). If iterating from back to front,
    // the iteration will not be affected.
    //
    private void CheckRespawnFrames()
    {
        GameObject frame = null;

        // Loop backwards over the frames list
        //
        for(int i = framesList.Count-1; i >= 0; i--)
        {
            frame = framesList[i];

            // Check if the frame's X position is less then leftExtent. If yes, then it
            // will be destroyed.
            //
            if(frame.transform.position.x < leftExtent)
            {
                // Set the X position for the new frame that wil be created
                //
                float newX = endFrame.transform.position.x + frameWidth;

                float gap = frameWidth * 0.1f;

                // Set the new frame's position so that it is at the end of the row of 
                // frames, with a gap between the new frame and the current end frame
                // (the new frame will become the current end frame in the next line)
                //
                frame.transform.position = new Vector3(newX + gap, 0f, 0f);

                // Update endFrame to be the new frame
                //
                endFrame = frame;

                // Changes: Removed some old code as they are not needed anymore since object pooling is implemented here.
            }
        }
    }

    // Check if the user has selected a frame (mousedown or touch). This is done by
    // casting a ray into the game view from the input position (after converting
    // from screen to world coordinates).
    //
    // Returns the frame that was selected, if any.
    //
    private GameObject CheckHitFrame()
    {
        GameObject frame = null;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 raypos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(raypos, Vector3.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    frame = hit.collider.gameObject; //returns the frame hit
                }
            }
        }
        return frame;
    }

    // Update the Seconds text in the UI
    //
    private void UpdateTime()
    {
        counterTime += Time.deltaTime;

        if (counterTime > 1f)
        {
            timeText.text = "Seconds: " + ++seconds;
            counterTime = 0;
        }
    }

    // Update the Score text in the UI
    //
    private void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    // Displays or hides the background sprites at regular intervals, in this case 
    // every 3 seconds
    //
    private void ShowBackgroundCharacters()
    {
        backgroundTime += Time.deltaTime;

        if (backgroundTime > 3f)
        {
            backgroundTime = 0;
            showBackgroundCharacters = !showBackgroundCharacters;

            mouse.SetActive(showBackgroundCharacters);
            rabbit.SetActive(showBackgroundCharacters);
            panda.SetActive(showBackgroundCharacters);         
        }
    }

    // Play a sound when the player selects a frame. If it is a mirrored frame, play the
    // CORRECT.wav file, otherwise play WRONG.wav. These are in the Resources/Audio folder.
    //
    private void PlaySound(bool matched)
    {
        if (matched)
        {
            // Load the CORRECT.wav file
            //
            clipToBePlayed = clipCorrect;
        }
        else
        {
            // Load the WRONG.wav file
            //
            clipToBePlayed = clipWrong;
        }
        // Play the sound
        //
        audioSource.PlayOneShot(clipToBePlayed);
    }

    private void Update()
    {
        // Make sure all the frames have been created before the game can be played
        //
        if(initialised)
        {
            // Animate the frames to the left
            //
            MoveFrames();

            // Once the frames have animated to the left, check if any need to be respawned.
            // Frame destroying and removal won't happen until the end of the current frame
            // (game frame, not gameobject frame)
            //
            CheckRespawnFrames();

            // Check if the player has selected any of the frames
            //
            hitFrame = CheckHitFrame();

            // Changes
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // If a frame has been selected, check if it is mirrored or not (check if all the bottom
            // sprites match their corresponding top sprites). Process the frames accordingly.
            //
            if (hitFrame)
            {
                bool matched = true;
                int numChildren = hitFrame.transform.GetChild(0).transform.childCount;
                for (int i = 0; i < numChildren; i++)
                {
                    // Get each top sprite (s1) and its corresponding bottom sprite (s2)
                    //
                    GameObject s1 = hitFrame.transform.GetChild(0).transform.GetChild(i).gameObject;
                    GameObject s2 = hitFrame.transform.GetChild(1).transform.GetChild(i).gameObject;

                    // Get the name of each sprite
                    //
                    string s1Name = s1.GetComponent<SpriteRenderer>().sprite.name;
                    string s2Name = s2.GetComponent<SpriteRenderer>().sprite.name;

                    // If the names are not the same, then the frame is not mirrored
                    //
                    if(s1Name != s2Name)
                    {
                        matched = false;
                    }
                }

                // If all the sprites match, loop across the frames list to find the frame to be 
                // deleted. This should be the same as the frame that has just been checked, i.e.
                // hitFrame.
                //
                if (matched == true)
                {
                    GameObject frameToDelete = null;

                    // Find the frame to delete in the frames list
                    //
                    foreach(GameObject f in framesList)
                    {
                        if(f == hitFrame)
                        {
                            frameToDelete = hitFrame;
                            Debug.Log(hitFrame);
                        }
                    }

                    // Create the new frame that will replace frameToDelete. This is much the same
                    // as in the CheckRespawnFrames function, so it is guaranteed to work.
                    //
                    float newX = endFrame.transform.position.x + frameWidth;
                    float gap = frameWidth * 0.1f;
                    GameObject newFrame = CreateFrame();
                    newFrame.name = frameToDelete.name;
                    newFrame.transform.position = new Vector3(newX + gap, 0f, 0f);

                    endFrame = newFrame;
                    framesList.Add(newFrame);

                    framesList.Remove(frameToDelete);
                    Destroy(frameToDelete);
                    
                    // Play a particle system for success or failure. The particle system to be
                    // instantiated is a pulic property set in the Inspector.
                    //
                    ParticleSystem ps = Instantiate(successParticles);
                    ps.transform.localScale = new Vector3(10, 10, 1);
                    //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ps.transform.position = mousePos;

                    // Play a success sound
                    //
                    PlaySound(true);

                    // Increment score, since the player has scored a point
                    //
                    score++;
                    UpdateScore();
                }
                else
                {
                    // Play the fail particle system

                    ParticleSystem ps = Instantiate(failParticles);
                    ps.transform.localScale = new Vector3(15, 15, 1);
                    //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    ps.transform.position = mousePos;

                    // Play a fail sound
                    //
                    PlaySound(false);
                }
            }

            // Update the positions of the oscillating background sprites. This is done using
            // the PingPong function
            //
            float time = Mathf.PingPong(Time.time * 1f, 1);
            mouse.transform.position = Vector3.Lerp(bk1PositionA, bk1PositionB, time);
            rabbit.transform.position = Vector3.Lerp(bk2PositionA, bk2PositionB, time);
            panda.transform.position = Vector3.Lerp(bk3PositionA, bk3PositionB, time);

            UpdateTime();
            ShowBackgroundCharacters();
        }

    }

}