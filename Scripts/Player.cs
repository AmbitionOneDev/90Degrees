using System;
using UnityEngine;
public class Player : MonoBehaviour
{
    /*--------------------------------------<defaults>--------------------------------------*/
    #region

    private static readonly float runSpeed = 12.5f;
    private readonly float jumpPadLaunchForce = 40f;
    private readonly float jumpForce = 14.5f;
    private Vector2 defaultSpeed = Vector2.up * runSpeed;
    private Vector3 startingCameraPosition = new Vector3(0f, 4f, -7f);

    #endregion
    /*--------------------------------------</defaults>--------------------------------------*/

    /*--------------------------------------<booleans>--------------------------------------*/
    #region

    // used externally in LevelGenerator
    public static bool hasDied = false;
    public bool isPaused;

    private bool isJumpPadTriggered;
    private bool hasSpeedUpPickupActive = false;
    private bool hasSlowDownPickupActive = false;
    private bool isGrounded = true;
    private bool canDoubleJump = false;
    private bool isFacingLeft = false;
    private bool isRotating = false;

    #endregion
    /*--------------------------------------</booleans>--------------------------------------*/

    /*--------------------------------------<constants>--------------------------------------*/
    #region

    private const double FLOAT_CALCULATION_ERROR = 1E-8f;
    private const float SCENE_RESET_TIME = 2f;
    private const float SPEED_UP_FACTOR = 1.5f;
    private const float SLOW_DOWN_FACTOR = 0.5f;
    private const float SPEED_PICKUP_DURATION = 5f;
    private readonly float DOUBLE_JUMP_FORCE_MULTIPLIER = 0.75f;
    private readonly float MAX_POSITION = 1.5815f;

    #endregion
    /*--------------------------------------</constants>--------------------------------------*/

    /*--------------------------------------<auxillaries>--------------------------------------*/
    #region

    public Rigidbody2D player;
    public GameObject deathAnimObject;
    public FollowPlayer FollowPlayer;
    public LevelGenerator LGscript;
    public GameObject clearer;
    public Animator deathAnimator;
    public Animator outlineAnimator;
    public GameStarter gameStarterScript;
    public Camera cameraMain;

    private enum Rotation {
        RIGHT = -1,
        LEFT = 1
    }

    private enum Directions
    {
        LEFT = -1,
        RIGHT = 1
    }
    private int rotationDirection;
    private int directionBeforePause;
    private bool isEligibleForJump = true;
    private float speedJumpFactor;
    private Vector2 jumpPadPosition;
    private Vector3 startingPosition;
    private bool isPressed;
    private bool leftJump;
    private bool rightJump;


    #endregion
    /*--------------------------------------</auxillaries>--------------------------------------*/

    public void Start()
    {
        startingPosition = new Vector3(-MAX_POSITION, -0.45f, 0);
        isGrounded = (Math.Abs(player.transform.position.x) - MAX_POSITION) < FLOAT_CALCULATION_ERROR;
    }

    public void OnEnable()
    {
        if(hasSpeedUpPickupActive)
            player.AddForce(Vector2.up * runSpeed * SPEED_UP_FACTOR, ForceMode2D.Impulse);

        else if(hasSlowDownPickupActive)
            player.AddForce(Vector2.up * runSpeed * SLOW_DOWN_FACTOR, ForceMode2D.Impulse);

        else
            // Add starting force
            player.AddForce(defaultSpeed, ForceMode2D.Impulse);

        // deactivate unnecessary death animation object and its animator component
        deathAnimObject.SetActive(false);
        deathAnimator.enabled = false;
        outlineAnimator.Play("ResetColorAnim");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        // If player lands on a jump pad
        if (collision.CompareTag("JumpPad"))
        {
            isJumpPadTriggered = true;

            isEligibleForJump = false;

            isGrounded = false;

            // Store the current jumppad position for further checks
            jumpPadPosition = collision.transform.position;

            // exit the function, important!
            return;
        }

        /*------------------------------------------------------------------------------------------------------------------********
         * ako je lik pokupio neki od pickupa
         * pa nakon toga pokupio suprotni pickup
         * makni efekte tog pickupa
         * i dodaj efekte tog novopokupljenog 
         * TO MORAS DODAT, JOS NIJE DODANO
         ------------------------------------------------------------------------------------------------------------------********/
        switch (collision.tag)
        {
            case "SpeedUp":
                outlineAnimator.Play("SpeedUpAnim");

                // If SpeedUp is already active
                // Do not allow another pickup of it
                if (hasSpeedUpPickupActive)
                    break;
                else
                {
                    // Set the current speed jump factor to a square of speedup factors
                    speedJumpFactor = SPEED_UP_FACTOR * SPEED_UP_FACTOR;

                    // Set vertical speed to 0
                    // to allow proper force addition
                    RemoveSpeedEffects();

                    // Destroy the picked up object
                    Destroy(collision.gameObject);
                    hasSpeedUpPickupActive = true;

                    // Add amplified force to the player
                    player.AddForce(Vector2.up * runSpeed * SPEED_UP_FACTOR, ForceMode2D.Impulse);

                    // Let the pickup last for a set amount of time
                    // Before resetting the speed back to normal
                    Invoke("RemoveSpeedEffects", SPEED_PICKUP_DURATION);
                    break;
                }

            case "SlowDown":
                outlineAnimator.Play("SlowDownAnim");

                // Set the current speed jump factor to a square of slowdown factors
                speedJumpFactor = SLOW_DOWN_FACTOR * SLOW_DOWN_FACTOR;

                // If SlowDown is already active
                if (hasSlowDownPickupActive)
                    break;
                else
                {
                    RemoveSpeedEffects();
                    Destroy(collision.gameObject);
                    hasSlowDownPickupActive = true;

                    // Add muffled force to the player
                    player.AddForce(Vector2.up * runSpeed * SLOW_DOWN_FACTOR, ForceMode2D.Impulse);

                    Invoke("RemoveSpeedEffects", SPEED_PICKUP_DURATION);
                    break;
                }

            //In case the player encounters an obstacle
            case "Obstacle":
                // Freeze it's position
                player.constraints = RigidbodyConstraints2D.FreezePosition;

                // Freeze the rotation
                isRotating = false;

                PlayDeathAnimation();

                player.velocity = Vector2.zero;

                // Disable the camera follow script (FollowPlayer.cs)
                FollowPlayer.enabled = false;

                isEligibleForJump = true;

                hasDied = true;

                hasSpeedUpPickupActive = false;
                hasSlowDownPickupActive = false;

                // Reload the scene
                Invoke("ReloadScene", SCENE_RESET_TIME);
                break;

            default:
                break;
        }
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("JumpPad"))
            // Disable further jump pad launches
            isJumpPadTriggered = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isRotating = false;
            isEligibleForJump = true;
            player.velocity = new Vector2(0f, player.velocity.y);

            // stick the player to the wall
            player.transform.position = (player.transform.position.x < 0)
                                            ? new Vector2(-MAX_POSITION, player.transform.position.y)
                                            : new Vector2(MAX_POSITION, player.transform.position.y);
        }
    }

    public void Update()
    {
        isGrounded = (MAX_POSITION - Math.Abs(player.transform.position.x)) < FLOAT_CALCULATION_ERROR;

        rightJump = Input.GetKeyDown(KeyCode.RightArrow);
        leftJump = Input.GetKeyDown(KeyCode.LeftArrow);
        isPressed = leftJump || rightJump;

        // prevent jumping into the wall you're on
        if (isGrounded && ((rightJump && player.transform.position.x > 0) || (leftJump && player.transform.position.x < 0))) return;

        /*-------------------------------------- <Double jump code> --------------------------------------*/
        #region 
        if (isPressed && isEligibleForJump)
        {
            isRotating = true;

            // used for rotation, -1 = left rotation, +1 = right rotation
            if (leftJump)
                rotationDirection = (int)Rotation.LEFT;
            else if (rightJump)
                rotationDirection = (int)Rotation.RIGHT;

            if (isGrounded)
            {

                if (leftJump && player.transform.position.x > 0)
                {
                    player.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
                    isFacingLeft = true;
                    isGrounded = false;
                    canDoubleJump = true;
                }

                else if (rightJump && player.transform.position.x < 0)
                {
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
                            player.AddForce(Vector2.left * jumpForce * DOUBLE_JUMP_FORCE_MULTIPLIER, ForceMode2D.Impulse);
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
                            player.AddForce(Vector2.right * jumpForce * DOUBLE_JUMP_FORCE_MULTIPLIER, ForceMode2D.Impulse);
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

        if (!isGrounded && isRotating)
            Rotate(rotationDirection);
        else
            StopRotating();

        #endregion
        /*-------------------------------------- </Double jump code> --------------------------------------*/

        /*-------------------------------------- <Jump pad code> --------------------------------------*/
        #region
        if (isJumpPadTriggered)
        {
            // Stop any horizontal movement
            player.velocity = new Vector2(0f, player.velocity.y);

            // Check whether the JumpPad is on the right side
            if (jumpPadPosition.x > 0)
                player.AddForce(new Vector2(-jumpPadLaunchForce, 0f), ForceMode2D.Impulse);
            else
                player.AddForce(new Vector2(jumpPadLaunchForce, 0f), ForceMode2D.Impulse);
        }
        #endregion
        /*-------------------------------------- </Jump pad code> --------------------------------------*/
    }

    public void FixedUpdate()
    {

        if (hasDied)
            isPressed = false;

        // If the player has already picked up a speed pickup
        // Destroy all the ones that come up
        // To remove possible confusion and free up some memory
        if (hasSlowDownPickupActive)
            DestroySpeedPickups("SlowDown");
        if (hasSpeedUpPickupActive)
            DestroySpeedPickups("SpeedUp");
        else
            outlineAnimator.Play("ResetColorAnim");

        // Player gets "stopped" randomly at times, fixed with this (hopefully)
        if (player.velocity.y > -0.5f && player.velocity.y < 0.5f) player.AddForce(defaultSpeed, ForceMode2D.Impulse);
    }

    // Methods
    #region                                              

    private void ResetPlayer()
    {
        // activate the player after the death animation has finished
        player.gameObject.SetActive(true);

        // reset velocity, position and rotation
        player.velocity = new Vector2(0f, 0f);

        // randomize left or right as starting side
        var a = new System.Random();
        int startingSide = (a.NextDouble() > 0.5) ? -1 : 1;

        startingPosition = new Vector3(startingSide * MAX_POSITION, -0.45f, 0);
        player.transform.position = startingPosition;

        gameObject.transform.GetChild(0).gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
    }

    /// <summary>
    /// Method used to properly apply speed after the player picks up a speed pickup.
    /// It stops the player from moving vertically, then adds the default force in order to reset the movement speed back to the starting value.
    /// Finally, resets both speed related flags to false.
    /// </summary>
    private void RemoveSpeedEffects()
    {

            // remove any vertical velocity
            player.velocity = new Vector2(player.velocity.x, 0f);

            // reset the movement to default
            player.AddForce(Vector2.up * runSpeed);

            // reset both speed pickup booleans
            hasSpeedUpPickupActive = false;
            hasSlowDownPickupActive = false;
            
            outlineAnimator.Play("ResetColorAnim");
    }

    private void ResetSpeed()
    {
        if (!isPaused) {
            // remove any vertical velocity
            player.velocity = new Vector2(player.velocity.x, 0f);

            // reset the movement to default
            player.AddForce(Vector2.up * runSpeed);
        }
    }

    /// <summary>
    /// Method used to reload the current scene.
    /// Sets currentScene to the current active scene, then uses scene manager to load the mentioned scene.
    /// </summary>
    private void ReloadScene()
    {
        if (GameStarter.isStarted)
        {
            // Regenerate the starting section
            LGscript.RegenerateStartingLevels();

            // set to default starting clearer position 
            clearer.transform.position = new Vector2(0f, -10.45f);

            // deactivate the death animation animator and gameObject
            deathAnimObject.SetActive(false);
            deathAnimator.enabled = false;

            ResetPlayer();

            ResetCamera();

            DestroyGeneratedLevels("LevelPrefab");

            // ground the player
            isGrounded = true;

            ResetSpeed();

            // activate the camera followage script
            FollowPlayer.enabled = true;

            // reset the starting conditions
            player.constraints = RigidbodyConstraints2D.None;
            player.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    /// <summary>
    ///     Resets the World Camera to its original position.
    /// </summary>
    private void ResetCamera()
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
    private void AddForceBasedOnPickup(Vector2 forceOrientation)
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
    /// <param name="pickupTag">
    ///     Takes in a string parameter which represents the ingame tag of the speed pickup you wish to destroy.
    /// </param>
    private void DestroySpeedPickups(String pickupTag)
    {
        GameObject[] speedPickups = GameObject.FindGameObjectsWithTag(pickupTag);

        foreach (GameObject obj in speedPickups)
            Destroy(obj);
    }

    /// <summary>
    ///     Destroys all created instances of level presets for the selected level difficulty
    /// </summary>
    /// <param name="levelDifficulty">
    ///     Difficulty/Theme of the level which the player is currently playing
    /// </param>
    private void DestroyGeneratedLevels(String levelDifficulty)
    {
        GameObject[] generatedLevels = GameObject.FindGameObjectsWithTag(levelDifficulty);
        foreach (GameObject obj in generatedLevels)
            Destroy(obj);
    }

    /// <summary>
    ///     Rotates the player's sprite represented as the child object in the wanted direction.
    /// </summary>
    /// <param name="direction">
    ///     Direction of the rotation.
    ///     Negative represents leftward rotation, positive represents rightward rotation.
    /// </param>
    private void Rotate(int direction)
    {
        GameObject parent = gameObject;

        //Rotate the child sprites of player object
        parent.transform.GetChild(0).gameObject.transform.Rotate(0f, 0f, 1.75f * direction, Space.Self);
        parent.transform.GetChild(1).gameObject.transform.Rotate(0f, 0f, 1.75f * direction, Space.Self);
    }

    /// <summary>
    ///     Stops any further rotation being done on the Player Sprite object by "sticking" it to the nearest side.
    /// </summary>
    private void StopRotating()
    {
        GameObject parent = gameObject;

        // Get current rotation angle and transform it into euler angles (from Quaternion)
        Vector3 rotation = parent.transform.GetChild(0).gameObject.transform.rotation.eulerAngles;

        // Round to nearest 90 degree side
        rotation.z = Mathf.Round(rotation.z / 90f) * 90f;

        // set the new angle
        parent.transform.GetChild(0).gameObject.transform.rotation = Quaternion.Euler(rotation);
        parent.transform.GetChild(1).gameObject.transform.rotation = Quaternion.Euler(rotation);
   }


    /// <summary>
    ///     Plays the death animation.
    /// </summary>
    private void PlayDeathAnimation()
    {
        // save the position of collision
        Vector2 deathPos = player.transform.position;

        // deactivate the player
        player.gameObject.SetActive(false);

        // enable the death animation object and its animator component
        deathAnimObject.SetActive(true);
        deathAnimator.enabled = true;

        // place the object where the player died and play animation
        deathAnimObject.transform.position = deathPos;
        deathAnimator.Play("DeathAnim");
    }

    public void StopMovement()
    {
        directionBeforePause = (player.velocity.x > 0) ? (int)Directions.RIGHT : (int)Directions.LEFT;
        player.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void ResetMovement()
    {
        // if the player is not moving because the game was paused after he died
        // reset his speed when resumed
        if (player.velocity.y < 1f) 
            player.AddForce(defaultSpeed, ForceMode2D.Impulse);

        player.constraints = RigidbodyConstraints2D.None; 
        Rotate(rotationDirection);

        if (!isGrounded)
        {
            player.velocity = new Vector2(0f, player.velocity.y);

            // if the player jumped on its own
            if (isEligibleForJump)
            {

                if (directionBeforePause == (int)Directions.LEFT)
                    player.AddForce(Vector2.left * jumpForce, ForceMode2D.Impulse);
                else if (directionBeforePause == (int)Directions.RIGHT)
                    player.AddForce(Vector2.right * jumpForce, ForceMode2D.Impulse);
            }

            // if it was launched off of a jumppad
            else
            {
                if (jumpPadPosition.x > 0)
                    player.AddForce(new Vector2(-jumpPadLaunchForce, 0f), ForceMode2D.Impulse);
                else
                    player.AddForce(new Vector2(jumpPadLaunchForce, 0f), ForceMode2D.Impulse);
            }
            
        }

    }
    #endregion
}
