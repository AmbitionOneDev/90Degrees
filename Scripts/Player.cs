using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    /*********<defaults>************/
    #region
    // The default running speed used for initial force addition
    public static float runSpeed = 12.5f;

    // The default jumping force used if the user presses a jump key.
    public readonly float jumpForce = 12.5f;

    // The default force applied after the player triggers a jump pad.
    private readonly float bounceForce = 40f;

    // Player's default speed
    private Vector2 velocityVector = Vector2.up * runSpeed;

    Vector3 startingCameraPosition = new Vector3(0f, 4f, -7f);
    #endregion
    /*********</defaults>************/

    /*********<booleans>************/
    #region
    // Gets set to true/false when the player
    // Enters/Exits the jump pad trigger
    private bool isJumpPadTriggered;

    // Used to check if the player has picked up a speedup pickup
    private bool hasSpeedUpPickupActive = false;

    // Used to check if the player has picked up a slowdown pickup
    private bool hasSlowDownPickupActive = false;

    bool isGrounded = true;

    bool canDoubleJump = false;

    bool isFacingLeft = false;

    public static bool hasDied = false;

    private bool isRotating = false;
    #endregion
    /*********</booleans>************/

    /*********<constants>************/
    #region
    // Used to eliminate any possible floating point decimal errors.
    private const double FLOAT_CALCULATION_ERROR = 1E-8f;

    // Time to reload scene after death
    private const float SCENE_RESET_TIME = 2f;

    // Speed up factor
    private const float SPEED_UP_FACTOR = 1.5f;
    
    // Slowdown factor 
    private const float SLOW_DOWN_FACTOR = 0.5f;

    // Calculated in Unity, imported as only a float number.
    // The speed that the player has
    // after it has been lauched off a jump pad.
    private const float JUMP_PAD_LAUNCH_SPEED = 20f;

    // Speed-related pickups duration in seconds
    private const float SPEED_CHANGE_DURATION = 5f;

    // Used to cancel out the force in case of double jump 
    // and add a slight boost for the second jump
    private float DOUBLE_JUMP_FORCE_MULTIPLIER = 0.75f;

    private float MAX_POSITION = 1.5815f;
    #endregion
    /*********</constants>************/

    /*********<auxillaries>************/
    #region
    // Referencing player object
    public Rigidbody2D player;

    // Referencing FollowPlayer script
    public FollowPlayer FollowPlayer;

    public LevelGeneratorScript LGscript;

    public GameObject clearer;

    // used for restraining position
    private Vector2 jumpPadPosition;

    // Stores the factor which is then later used
    // to add wanted force to the player
    // VIEW: AddForceBasedOnPickup
    private float speedJumpFactor;

    public GameStarter gameStarterScript;

    public Camera cameraMain;

    Vector3 startingPosition;

    private bool isEligibleForJump = true;

    private int direction = 0;


    #endregion
    /*********</auxillaries>************/

    public void Start()
    {
        startingPosition = new Vector3(-MAX_POSITION, -0.45f, 0);
        isGrounded = (Math.Abs(player.transform.position.x) - MAX_POSITION) < FLOAT_CALCULATION_ERROR;
    }

    public void OnEnable()
    {
        // Add starting force
        player.AddForce(velocityVector, ForceMode2D.Impulse);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    { 

        // If player lands on a jump pad
        if (collision.CompareTag("JumpPad"))
        {
            // Flag the jump pad as triggered
            isJumpPadTriggered = true;
            isEligibleForJump = false;
            isGrounded = false;
            // Store the current jumppad position for further checks
            jumpPadPosition = collision.transform.position;
            return;
        }

        /********************************************************
         * ako je lik pokupio neki od pickupa
         * pa nakon toga pokupio suprotni pickup
         * makni efekte tog pickupa
         * i dodaj efekte tog novopokupljenog 
         * TO MORAS DODAT, JOS NIJE DODANO
         ********************************************************/
        switch (collision.tag)
        {
            // If the player picks up the SpeedUp pickup
            case "SpeedUp":

                // If SpeedUp is already active
                // Do not allow another pickup of it
                if (hasSpeedUpPickupActive)
                {
                    break;
                }

                // If SpeedUp is not active
                else
                {
                    // Set the current speed jump factor to a square of speedup factors
                    speedJumpFactor = SPEED_UP_FACTOR * SPEED_UP_FACTOR;

                    // Set vertical speed to 0
                    // to allow proper force addition
                    ResetSpeed();

                    // Destroy the picked up object
                    Destroy(collision.gameObject);

                    // let the game know
                    // that the player picked up
                    // a SpeedUp pickup
                    hasSpeedUpPickupActive = true;

                    // Add amplified force to the player
                    player.AddForce(Vector2.up * runSpeed * SPEED_UP_FACTOR, ForceMode2D.Impulse);

                    // Let the pickup last for a set amount of time
                    // Before resetting the speed back to normal
                    Invoke("ResetSpeed", SPEED_CHANGE_DURATION);
                    break;
                }

            // If the player picks up the SlowDown pickup
            case "SlowDown":

                // Set the current speed jump factor to a square of slowdown factors
                speedJumpFactor = SLOW_DOWN_FACTOR * SLOW_DOWN_FACTOR;

                // If SlowDown is already active
                if (hasSlowDownPickupActive)
                {
                    break;
                }

                // If SlowDown is not active
                else
                {
                    // Set vertical speed to 0
                    // to allow proper force addition
                    ResetSpeed();

                    // Destroy the picked up object
                    Destroy(collision.gameObject);

                    // let the game know
                    // that the player picked up
                    // a SlowDown pickup
                    hasSlowDownPickupActive = true;

                    // Add muffled force to the player
                    player.AddForce(Vector2.up * runSpeed * SLOW_DOWN_FACTOR, ForceMode2D.Impulse);

                    // Let the pickup last for a set amount of time
                    // Before resetting the speed back to normal
                    Invoke("ResetSpeed", SPEED_CHANGE_DURATION);
                    break;
                }

            // In case the player encounters an obstacle 
            case "Obstacle":
                // Freeze it's position
                player.constraints = RigidbodyConstraints2D.FreezePosition;

                // Freeze the rotation
                isRotating = false;
                /*************add a death animation****************/

                player.velocity = Vector2.zero;

                // Disable the camera follow script (FollowPlayer.cs)
                FollowPlayer.enabled = false;

                isEligibleForJump = true;

                hasDied = true;

                // Call ReloadScene after a set amount of seconds has passed
                Invoke("ReloadScene", SCENE_RESET_TIME);
                break;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the player gets bounced off the jump pad
        if (collision.CompareTag("JumpPad"))
            // Revert the trigger state to not triggered
            isJumpPadTriggered = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        String collidedObject = collision.collider.ToString();
        if (collidedObject.StartsWith("Wall"))
        {
            isRotating = false;
            isEligibleForJump = true;
            player.velocity = new Vector2(0f, player.velocity.y);
            player.transform.position = (player.transform.position.x < 0) ? new Vector2(-MAX_POSITION, player.transform.position.y) : new Vector2(MAX_POSITION, player.transform.position.y);
        }
    }

    public void Update()
    {
        bool leftJump = Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightJump = Input.GetKeyDown(KeyCode.RightArrow);


        if (leftJump)
            direction = 1;
        else if(rightJump)
            direction = -1;

        isGrounded = (MAX_POSITION - Math.Abs(player.transform.position.x)) < FLOAT_CALCULATION_ERROR;

        // If the player has already picked up a speed pickup
        // Destroy all the ones that come up
        // To remove possible confusion and free up some memory
        if (hasSlowDownPickupActive)
            DestroySpeedPickups("SlowDown");
        if (hasSpeedUpPickupActive)
            DestroySpeedPickups("SpeedUp");

        // Assign true or false depending on keypress state
        bool isPressed = Input.GetKeyDown(KeyCode.LeftArrow) ||
                         Input.GetKeyDown(KeyCode.RightArrow);

        if (hasDied)
            isPressed = false;

        /***************** <Double jump code> *****************/


        #region 
        if (isPressed && isEligibleForJump)
        {
            isRotating = true;
            
            if (isGrounded) {

                if (leftJump && player.transform.position.x > 0) {
                    player.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
                    isFacingLeft = true;
                    isGrounded = false;
                    canDoubleJump = true;
                }

                if (rightJump && player.transform.position.x < 0) {
                    player.AddForce(Vector2.right * jumpForce, ForceMode2D.Impulse);
                    isFacingLeft = false;
                    isGrounded = false;
                    canDoubleJump = true;
                }
            }
            else
            {
                if (canDoubleJump)
                {
                    if (leftJump)
                    {
                        if (isFacingLeft)
                        {
                            player.AddForce(Vector2.left * jumpForce * DOUBLE_JUMP_FORCE_MULTIPLIER, ForceMode2D.Impulse);
                        }
                        else
                        {
                            player.velocity = new Vector2(0f, player.velocity.y);
                            player.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
                        }
                        isFacingLeft = true;
                    }

                    if (rightJump)
                    {
                        if (!isFacingLeft)
                        {
                            player.AddForce(Vector2.right * jumpForce * DOUBLE_JUMP_FORCE_MULTIPLIER, ForceMode2D.Impulse);
                        }
                        else
                        {
                            player.velocity = new Vector2(0f, player.velocity.y);
                            player.AddForce(Vector2.right * jumpForce, ForceMode2D.Impulse);
                        }
                        isFacingLeft = false;
                    }
                    canDoubleJump = false;
                }
            }
        }

        if (!isGrounded)
        {
            if (isRotating)
            {
                Rotate(direction);
            }
        }
        else
            StopRotating();
        

        #endregion
        /***************** </Double jump code> *****************/

        /***************** <Jump pad code> *****************/
        if (isJumpPadTriggered) 
        {
            // If the player was not previously flipped
            // but is within the proper position flip it
            if ((player.transform.localScale.x > 0 && player.transform.position.x < 0) ||
                (player.transform.localScale.x < 0 && player.transform.position.x > 0))
                Flip();

            // Stop any horizontal movement
            player.velocity = new Vector2(0f, player.velocity.y);

            // Check whether the JumpPad is on the right side
            if (jumpPadPosition.x > 0)
                player.AddForce(new Vector2(-bounceForce, runSpeed * 0.05f), ForceMode2D.Impulse);

            // Check whether the JumpPad is on the left side
            else
                player.AddForce(new Vector2(bounceForce, runSpeed * 0.05f), ForceMode2D.Impulse);
        }
        /***************** </Jump pad code> *****************/

        // Player gets "stopped" randomly at times, fixed with this (hopefully)
        if (player.velocity.y > 0.01f && player.velocity.y < 0.5f)
        {
            player.AddForce(velocityVector, ForceMode2D.Impulse);
        }
    }

    // Methods
    #region                                              

    public void ResetPlayer()
    {
        player.velocity = new Vector2(0f, 0f);
        player.transform.position = startingPosition;
        gameObject.transform.GetChild(0).gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    /// <summary>
    /// Used to flip the player model when necessary,
    /// e.g. when the jump button is pressed.
    /// </summary>
    public void Flip()
    {
        // Rotate the current player scale by 180 degrees
        Vector2 newScale = player.transform.localScale;
        newScale.x *= -1;
        player.transform.localScale = newScale;
    }

    /// <summary>
    /// Method used to properly apply speed after the player picks up a speed pickup.
    /// It stops the player from moving vertically, then adds the default force in order to reset the movement speed back to the starting value.
    /// Finally, resets both speed related flags to false.
    /// </summary>
    public void ResetSpeed()
    {
        // remove any vertical velocity
        player.velocity = new Vector2(player.velocity.x, 0f);

        // reset the movement to default
        player.AddForce(Vector2.up * runSpeed);

        // reset both speed pickup booleans
        hasSpeedUpPickupActive = false;
        hasSlowDownPickupActive = false;
    }

    /// <summary>
    /// Method used to reload the current scene.
    /// Sets currentScene to the current active scene, then uses scene manager to load the mentioned scene.
    /// </summary>
    public void ReloadScene()
    {
        if (GameStarter.isStarted) {
            LGscript.RegenerateStartingLevels();

            // set to default starting clearer position 
            clearer.transform.position = new Vector2(0f, -10.45f);
            ResetPlayer();
            ResetCamera();
            DestroyGeneratedLevels("LevelPrefab");
            isGrounded = true;
            // Add starting force
            player.AddForce(velocityVector, ForceMode2D.Impulse);
            FollowPlayer.enabled = true;
            player.constraints = RigidbodyConstraints2D.None;
            player.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void ResetCamera()
    {
        cameraMain.transform.position = startingCameraPosition;
    }


    /// <summary>
    /// Used to check how much force the game should add to the player model.
    /// Takes into consideration the current status of speed pickups.
    /// If the player does not have a pickup, set its side to side movement speed to default.
    /// Otherwise, add the default force multiplied by the current pickup's speed factor.
    /// </summary>
    /// 
    /// <param name="forceOrientation">
    /// Takes in a Vector2 to indicate in which direction the force should be applied.
    /// Preferably use Vector2.left or Vector2.right for side to side consistent movement.
    /// </param>
    public void AddForceBasedOnPickup(Vector2 forceOrientation)
    {
        // If the player is moving at a normal speed 
        // Add normal jump force
        if (!hasSpeedUpPickupActive && !hasSlowDownPickupActive)
            // Add leftward force
            player.AddForce(forceOrientation * jumpForce);

        // If the player has a SpeedUp pickup
        // Make it more responsive
        else
            player.AddForce(forceOrientation * jumpForce * speedJumpFactor);
    }

    /// <summary>
    /// Used to remove all possible speed pickups after a player has picked one up.
    /// NOTE: if a player has picked up a slowdown pickup, the speed up pickups are stil going to spawn.
    /// Therefore, in Update() check for both possible pickups.
    /// </summary>
    /// <param name="pickupTag">Takes in a string parameter which represents the ingame tag of the speed pickup you wish to destroy.</param>
    public void DestroySpeedPickups(String pickupTag)
    {
        GameObject[] speedPickups = GameObject.FindGameObjectsWithTag(pickupTag);

        foreach (GameObject obj in speedPickups)
            Destroy(obj);
    }

    public void DestroyGeneratedLevels(String levelTag) {
        GameObject[] generatedLevels = GameObject.FindGameObjectsWithTag(levelTag);
        foreach (GameObject obj in generatedLevels)
            Destroy(obj);
    }

    // -1 represents leftward rotation, 1 represents rightward
    public void Rotate(int direction)
    {
        GameObject parent = gameObject;
        parent.transform.GetChild(0).gameObject.transform.Rotate(0f,0f,1.5f * direction, Space.Self);
    }

    public void StopRotating()
    {
        GameObject parent = gameObject;
        Vector3 rotation = parent.transform.GetChild(0).gameObject.transform.rotation.eulerAngles;
        rotation.z = Mathf.Round(rotation.z / 90f) * 90f;
        parent.transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(rotation);
    }
    #endregion
}
