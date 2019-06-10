using System.Collections;
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

    // Changes: Renamed the variables
    public GameObject mouse;                        // Background sprite that oscillates left to right 
    public GameObject rabbit;                       // Background sprite that oscillates left to right 
    public GameObject panda;                        // Background sprite that oscillates left to right 
    
    // This is where I place all the variables I have changed/declared outside their functions.
    //***********************************************************************************************************************
    // Changes:
    public Object[] sprites;                        // This is the array in which all the sprites/ is kept
    public Object[] sprites2;
    private Object[] themeChange;
    private GameObject frameToBeChanged;
    private int randomIndex;                        // This is the random value that chooses which sprite in the array to use
    private Camera mainCam;                         // Reference to Camera.main
    private Vector3 mousePos;                       // Reference to current mousePos in the Game view
    private float time;                             // Reference for background sprites function in Update()
    private float gap;                              // The gap between the frames
    private float newX;                             // X position for new Frames
    private GameObject newFrame;                    // Reference to the new frame
    private Vector3 failParticleLocalScale;         // Vector3 scale for fail particles
    private Vector3 successParticleLocalScale;      // Vector3 scale for success particles
    public GameObject pauseButton;                  // Reference to the pause button

    private float numChildren;                      // The number of children in the top and bottom of frame 
                                                    //(which through extensive testing is always 6)

    // UpdateFrames() variables
    private GameObject top;                         // Top half of the frame
    private GameObject bottom;                      // Bottom half of the frame
    private int rand;                               // This is the numerical value that determines whether the frames are matched or not matched.
    private bool mirror;                            // Boolean value that determines whether the frames should be mirrored or not

    // For hitframe checks in Update()
    private Transform hitFrameTop;                  // Top half of the frame that has been hit
    private Transform hitFrameBottom;               // Top half of the frame that has been hit
    private string topSpriteName;                   // Name of each sprite in the top half
    private string bottomSpriteName;                // Name of each sprite in the top half

    private bool matched;                           // The boolean value that determines whether the frames are matched or not

    // MoveFrames() variables
    private Vector3 speedUp;                        // Vector3 for speeding up the frame
    private Vector3 normalSpeed;                    // Vector3 for the frames' normal speed

    // Raycasting related variables
    private Vector3 raypos;                         // Positioning of the ray
    private RaycastHit hit;                         // Whatever has been hit will be referenced with this in the Raycast function
    private GameObject hitFrame;                    // GameObject version of "hit", so that it can actually be used in Update()

    // PlaySound() variables;
    private AudioSource audioSource;                // AudioSource Component that will be added to the GameObject on Start()
    private AudioClip clipToBePlayed;               // Clip that will be played 
    private AudioClip clipCorrect;                  // AudioClip for the CORRECT sound
    private AudioClip clipWrong;                    // AudioClip for the WRONG sound

    // CheckHitFrame() variables
    private GameObject checkHitFrame;               // Appropriately named variable for checkHitFrame, reference for the frame that has been hit

    // GetDistanceToNeighbours
    private Vector3 currentFramePos;                // Reference to current iterated frame position
    private bool leftFrame;                         // Reference to the left most frame
    private float distanceToNeighbour;              // Renamed the variable for easier identification, also used in MoveFrames()

    //CheckRespawnFrames(), Start() in while loop
    private GameObject frame;                       // Cached reference for frame
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

    private float frameWidth;                       // The width of a frame of sprites
    private List<GameObject> framesList;            // Created frames are stored in this list
    private bool initialised = false;               // When initialised is true, the game can start

    int score = 0;
    int seconds = 0;
    float counterTime = 0;                          // Used for the Seconds UI display
    float backgroundTime = 0;                       // Used for the background sprites oscillation

    bool showBackgroundCharacters = false;          // The background sprites are shown intermittently, i.e. on/off

    // When a new frame is created, it must be placed behind the rightmost, or end, frame. endFrame
    // is the rightmost frame. This is updated every time a new frame is created and placed at the end.
    private GameObject endFrame;

    private void Start()
    {
        // Changes: Placing all the images into an array right at the start of the game
        sprites = Resources.LoadAll("Images/animals", typeof(Sprite));
        sprites2 = Resources.LoadAll("Images/fruit", typeof(Sprite));

        // Changes: MoveFrames() variables
        speedUp = new Vector3(speed * 10f, 0f, 0f);
        normalSpeed = new Vector3(speed, 0f, 0f);

        // Changes: The audio source will now be added to the GameObject GameManager at the start instead of each and every time a
        //          sound is played.
        audioSource = gameObject.GetComponent<AudioSource>();

        // Changes: Loaded the clips once in Start()
        clipCorrect = (AudioClip)Resources.Load("Audio/CORRECT");
        clipWrong = (AudioClip)Resources.Load("Audio/WRONG");

        // Changes: Added this since all numChildren in the script return the same value as all frames have the same structure
        numChildren = framePrefab.transform.GetChild(0).childCount;

        // Changes: Added a camera.main reference so that a reference of it isn't constantly created like previously
        mainCam = Camera.main;

        // Changes: Vector3 for localScale of particle systems
        failParticleLocalScale = new Vector3(15, 15, 1);
        successParticleLocalScale = new Vector3(10, 10, 1);

        Application.targetFrameRate = 70;


        // Find the height and width of the game view in world units
        //
        gameHeight = mainCam.orthographicSize * 2f;
        gameWidth = mainCam.aspect * gameHeight;

        // Calculate the X axis values for frame removal and the positioning of new frames

        // Changes: I reduced the "true" gamewidth aka the size of the area where the sprites spawn so that it doesn't have unneccessary 
        // frames that the player don't actually see.
        leftExtent = -gameWidth*1f;
        rightExtent = gameWidth*1f;

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
            frame = Instantiate(framePrefab);
            // Changes: Added this so that when the game starts, the frames are already randomised, instead of the default all rats frame
            UpdateFrame(frame);
            frame.name = "Frame_" + n++;

            frame.transform.position = currPos;

            // The box collider has been sized to fit the boundaries of the sprite in the Frame prefab.
            //
            frameWidth = frame.GetComponent<BoxCollider>().bounds.size.x;

            // The gap between one frame and the next: |     |<-gap->|     |
            gap = frameWidth * 0.1f;

            currX += frameWidth + gap;
            framesList.Add(frame);
        }
        endFrame = framesList[framesList.Count-1];  // endFrame is the last frame added to the list
        initialised = true;                         // Set to true so that the game can start
    }

    // Creates a new frame. This is done when all the initial frames are created before the
    // game starts, and also when a frame is destroyed after it moves past leftExtent and a
    // new frame is created to take its place.

    // Changes: Replaced the name with UpdateFrame to illustrate accurately what the function does
    public GameObject UpdateFrame(GameObject frame)
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
        newFrame = frame;

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
        top = newFrame.gameObject.transform.GetChild(0).gameObject;

        // Changes:
        if (PauseMenu.ChangeSprites)
        {
            themeChange = sprites2;
        }
        else
        {
            themeChange = sprites;
            Debug.Log("theme was changed back to normal");
        }

        // Loop across all the top children in the new frame
        //
        for (int i = 0; i < numChildren; i++)
        {
            // Each sprite gameobject in the new frame must be replaced with a new sprite gameobject.
            // Choose a random sprite from the sprites array.
            //
            randomIndex = Random.Range(0, themeChange.Length); //Move this out of the loop 

            // Changes: This gets the top category's child(i).gameObject and gets the Sprite Renderer component and changes the sprite to
            // a random one in the Resource array.
            top.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)themeChange[randomIndex];
        }

        // Now we replace the default bottom sprites with new sprites
        //
        bottom = newFrame.gameObject.transform.GetChild(1).gameObject;

        // Get a random number between 1 and 10. For integers, the Random.Range function is not
        // inclusive of the max value argument, which is why it is 11.
        //
        rand = Random.Range(1, 11);

        // Check the value of the random number. If it is >=5, then set this new frame to be
        // mirrored (top and bottom sprites are the same). If rand is <=4, then set the frame
        // to be unmirrored. If the frame is mirrored, then the each bottom sprite will be a
        // copy of its corresponding top sprite.
        //
        mirror = rand > 0 ? true : false;

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
                // Changes: Changes the sprites instead of spawning them of the older one.
                bottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = top.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else
            {
                // The frame is not mirrored, so the bottom sprites must be different from the top
                // sprites.
                //
                randomIndex = Random.Range(0, themeChange.Length);

                // Changes: Now it changes the sprites instead of replacing them.
                bottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)themeChange[randomIndex];
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
            distanceToNeighbour = GetDistanceToNeighbour(frame);

            if (distanceToNeighbour > frameWidth * 0.5f)
            {
                // Changes: Cached the Vectors
                frame.GetComponent<Rigidbody>().velocity = speedUp;
            }
            else
            {
                // Changes: Cached the Vectors
                frame.GetComponent<Rigidbody>().velocity = normalSpeed;
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
        // Renamed the variable to be more appropriate
        distanceToNeighbour = 0f;

        // Set up the ray parameters
        //
        // Changes: Cached the variables
        currentFramePos = frame.gameObject.transform.position;
        raypos = new Vector3(currentFramePos.x - frameWidth, 0f, currentFramePos.z);
        

        // Cast the ray. Check for a collision, then check if the collided object's name 
        // starts with "Frame". If so, them set distance to the hit distance.
        //
        if (Physics.Raycast(raypos, Vector3.left, out hit, gameWidth*2f))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name.StartsWith("Frame"))
                {
                    distanceToNeighbour = hit.distance;
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
            // Changes: Cached the variable leftFrame
            leftFrame = true;
            if (leftFrame == true && frame.gameObject.transform.position.x > -gameWidth/2f)
            {
                distanceToNeighbour = gameWidth * 2f; ;
            }
        }

        return distanceToNeighbour;
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
        frame = null;

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
                newX = endFrame.transform.position.x + frameWidth;

                gap = frameWidth * 0.1f;

                // Set the new frame's position so that it is at the end of the row of 
                // frames, with a gap between the new frame and the current end frame
                // (the new frame will become the current end frame in the next line)
                //
                frame.transform.position = new Vector3(newX + gap, 0f, 0f);

                // Update endFrame to be the new frame
                //
                endFrame = frame;

                // Changes: Removed some old code as they are not needed anymore since object pooling is implemented here.
                // It used to be that a new frame was created here, and then the new frame was the endframe, and the old frame was removed
                // from the list and destroyed and the newFrame was added to the list. Now i simply set the current iterated frame x value to 
                // the current endFrame x value and then set the current iterated frame as the new endFrame.
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
        // Changes: Cached the variable and renamed it to be more obvious what it contains
        checkHitFrame = null;

        if (Input.GetMouseButtonDown(0))
        {
            // Changes: Cached the variable and RaycastHit hit is now declared outside the function
            raypos = mainCam.ScreenToWorldPoint(Input.mousePosition);

            if (Physics.Raycast(raypos, Vector3.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider != null)
                {
                    checkHitFrame = hit.collider.gameObject; //returns the frame hit

                }
            }
        }
        return checkHitFrame;
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
            // Changes: Declared the variable globally, and loaded the audioClip once in start.
            clipToBePlayed = clipCorrect;
        }
        else
        {
            // Load the WRONG.wav file
            // Changes: Declared the variable globally, and loaded the audioClip once in start.
            clipToBePlayed = clipWrong;
        }
        // Play the sound
        // Changes: Added the audioSource component to the GameObject in Start(), essentially "caching" it.
        audioSource.PlayOneShot(clipToBePlayed);
    }

    private void Update()
    {
        // Make sure all the frames have been created before the game can be played
        //
        if(initialised)
        {
            // Changes: Removed the RotateSprites() function as now it is under a new Script, SpriteManager.

            // Animate the frames to the left
            //
            MoveFrames();

            // Changes: Removed CheckRespawnFrames() as it is already called in MoveFrames() every frame anyway;

            // Check if the player has selected any of the frames
            //
            // Changes: Cached the variable and added this if statement to prevent hit registers when the game is paused
            if (!PauseMenu.GameIsPaused)
            {
                hitFrame = CheckHitFrame();
            }

            // If a frame has been selected, check if it is mirrored or not (check if all the bottom
            // sprites match their corresponding top sprites). Process the frames accordingly.
            //
            if (hitFrame)
            {
                matched = true;

                // Changes: Cached these variables and renamed them to better illustrate what they contain
                hitFrameTop = hitFrame.transform.GetChild(0);
                hitFrameBottom = hitFrame.transform.GetChild(1);
                for (int i = 0; i < numChildren; i++)
                {
                    // Get the name of each sprite
                    //
                    // Changes: Renamed the variable
                    topSpriteName = hitFrameTop.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite.name;
                    bottomSpriteName = hitFrameBottom.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite.name;

                    // If the names are not the same, then the frame is not mirrored
                    //
                    if(topSpriteName != bottomSpriteName)
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
                    // Changes: Removed some old code that was not needed anymore due to updated code in CreateFrames()

                    // Create the new frame that will replace frameToDelete. This is much the same
                    // as in the CheckRespawnFrames function, so it is guaranteed to work.
                    //
                    // Changes: Cached these 3 variables
                    newX = endFrame.transform.position.x + frameWidth;
                    gap = frameWidth * 0.1f;
                    newFrame = UpdateFrame(hitFrame);

                    newFrame.transform.position = new Vector3(newX + gap, 0f, 0f);
                    // Changes: Sets the new updated frame to be the endFrame
                    endFrame = newFrame;

                    // Play a particle system for success or failure. The particle system to be
                    // instantiated is a pulic property set in the Inspector.
                    //
                    ParticleSystem ps = Instantiate(successParticles);
                    // Changes: Cached the vector3 scale
                    ps.transform.localScale = successParticleLocalScale;
                    mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    ps.transform.position = mousePos;

                    // Play a success sound
                    //
                    PlaySound(true);

                    // Increment score, since the player has scored a point
                    //
                    score++;
                    // Changes: Moved function so that it is only called when the player scores, instead of calling it every frame
                    UpdateScore();
                }
                else
                {
                    // Play the fail particle system

                    ParticleSystem ps = Instantiate(failParticles);
                    // Changes: Cached the vector3 scale
                    ps.transform.localScale = failParticleLocalScale;
                    mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                    ps.transform.position = mousePos;

                    // Play a fail sound
                    //
                    PlaySound(false);
                }
            }

            // Update the positions of the oscillating background sprites. This is done using
            // the PingPong function
            // Changes: Cached the time variable
            time = Mathf.PingPong(Time.time * 1f, 1);
            mouse.transform.position = Vector3.Lerp(bk1PositionA, bk1PositionB, time);
            rabbit.transform.position = Vector3.Lerp(bk2PositionA, bk2PositionB, time);
            panda.transform.position = Vector3.Lerp(bk3PositionA, bk3PositionB, time);

            UpdateTime();
            ShowBackgroundCharacters();
        }

    }

    // Changes:
    public void UpdateAllFrames()
    {
        for (int i = 0; i < framesList.Count; i++)
        {
            frameToBeChanged = framesList[i]; 
            UpdateFrame(frameToBeChanged);
        }

    }

}