using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreNumber;
    public TextMeshProUGUI scoreMultiplier;
    public TextMeshProUGUI pickupNotifier;
    public TextMeshProUGUI highScoreNumber;
    public TextMeshProUGUI highScoreText;
    public Animator outlineAnimator;
    public SpriteRenderer outlineSprite;

    private bool hasDoubleScorePickupActive = false;
    private bool hasHalfScorePickupActive = false;
    private bool isOutlineTransparent = false;
    private bool fadeOutPickupNotifier = false;
    private bool isPickupNotifierTextVisible;

    private int score;
    private int pickupsCollectedCounter;
    private int lastKnownPosition;
    private int highScore = 0;

    //private float timeElapsed = 0f;
    //private float startingTime;


    // Score-related pickups duration in seconds
    private const float SCORE_MULTIPLIER_CHANGE_DURATION = 5f;


    public void Start()
    {
        scoreNumber.text = "0";
        pickupNotifier.text = "";
        lastKnownPosition = 0;
        pickupsCollectedCounter = 0;
        highScoreNumber.enabled = false;
        highScoreText.enabled = false;
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "DoubleScore":
                // Destroy the picked up object
                Destroy(collision.gameObject);

                // clear opposite pickups if there are any
                if (hasHalfScorePickupActive) {
                    hasHalfScorePickupActive = false;
                    pickupsCollectedCounter = 0;
                }

                hasDoubleScorePickupActive = true;

                // change the notification text
                pickupNotifier.text = "+5s double score";

                // should the notification fade out? yes
                fadeOutPickupNotifier = true;
                ResetColor();

                // increase the collected pickups
                pickupsCollectedCounter++;

                // call the coroutine-based function to check when to remove pickups
                // handles stacking pickups
                RemoveAPickup();
                break;

                // Same as DoubleScore
            case "HalfScore":
                Destroy(collision.gameObject);

                if (hasDoubleScorePickupActive) {
                    hasDoubleScorePickupActive = false;
                    pickupsCollectedCounter = 0;
                }

                hasHalfScorePickupActive = true;

                pickupNotifier.text = "+5s half score";
                
                fadeOutPickupNotifier = true;
                ResetColor();

                pickupsCollectedCounter++;

                RemoveAPickup();
                break;

            case "Obstacle":
                if (score > highScore)
                {
                    highScore = score;
                    highScoreText.text = "New high score!";
                }
                else
                    highScoreText.text = "High score:";
                highScoreNumber.text = highScore.ToString();
                highScoreNumber.enabled = true;
                highScoreText.enabled = true;
                Invoke("ResetScore", 2f);
                break;
        }

    }

    public void FixedUpdate()
    {
        score = int.Parse(scoreNumber.text);
        
        int currentPosition = (int)transform.position.y;


        // if there is an outline but the player has no active pickups
        if (!isOutlineTransparent && !hasDoubleScorePickupActive && !hasHalfScorePickupActive)
        {
            // set outline to transparent
            outlineAnimator.Play("ResetColorAnim");
            isOutlineTransparent = true;
        }

        // if the pickup Noti should fade out, fade it out
        if (fadeOutPickupNotifier) FadeOutText();


        if (!isPickupNotifierTextVisible)
        {
            pickupNotifier.text = "";
            ResetColor();
            isPickupNotifierTextVisible = true;
        }

        // if there are no pickups, remove the "xSomething" text
        if (pickupsCollectedCounter == 0) scoreMultiplier.text = "";

        if (hasDoubleScorePickupActive)
        {
            isOutlineTransparent = false;
            // play the outline animation
            outlineAnimator.Play("DoubleScoreAnim");

            // update the text to show the multiplier
            scoreMultiplier.text = "x" + 2 * pickupsCollectedCounter;

            // add a doubled point every 1 meter (or whatever that is)
            if (currentPosition > lastKnownPosition)
            {
                scoreNumber.text = (score + 2 * pickupsCollectedCounter).ToString();
                lastKnownPosition = currentPosition;
            }
        }

        // same but for HalfScore
        else if (hasHalfScorePickupActive)
        {
            isOutlineTransparent = false;
            outlineAnimator.Play("HalfScoreAnim");

            scoreMultiplier.text = "x" + System.Math.Round(0.5f / pickupsCollectedCounter, 2);
            if (currentPosition > lastKnownPosition) {
                // don't divide by 0
                if (pickupsCollectedCounter > 0)
                    scoreNumber.text = (score + (int)(0.5 / pickupsCollectedCounter)).ToString();
                lastKnownPosition = currentPosition;
            }
        }

        // default case, no pickups
        else
        {
            if (currentPosition > lastKnownPosition)
            {
                lastKnownPosition = currentPosition;
                ++score;
                scoreNumber.text = score.ToString();
            }
        }
    }

    // Zakomentirano je ono da nadodajemo na trenutni, logicnije mi je da se reseta skroz

    private IEnumerator DelayRemoval(float time)
    {
        //startingTime = Time.time;

        // "sleep" for some time
        yield return new WaitForSecondsRealtime(time);
        //startingTime = 0f;

        // on awake decrement the pickups
        --pickupsCollectedCounter;

        // if there are any more left, do it again
        if (pickupsCollectedCounter > 0) StartCoroutine(DelayRemoval(SCORE_MULTIPLIER_CHANGE_DURATION));

        // if there are none left, reset the booleans
        if (pickupsCollectedCounter == 0)
        {
            if (hasDoubleScorePickupActive) hasDoubleScorePickupActive = false;

            else if (hasHalfScorePickupActive) hasHalfScorePickupActive = false;
        }
    }

    private void RemoveAPickup()
    {
        //timeElapsed = Time.time - startingTime;

        //if(pickupsCollectedCounter == 1)
          // StartCoroutine(DelayRemoval(SCORE_MULTIPLIER_CHANGE_DURATION));

        //else if (pickupsCollectedCounter > 0) {
            // stop the previous coroutine in order to properly reset the time
            StopAllCoroutines();
            StartCoroutine(DelayRemoval(SCORE_MULTIPLIER_CHANGE_DURATION));
        //}
    }

    private void FadeOutText()
    {
        // if text is visible
        if (pickupNotifier.color.a > 0) {
            // reduce it until close 0
            pickupNotifier.color =
                new Color(pickupNotifier.color.r, pickupNotifier.color.g, pickupNotifier.color.b, pickupNotifier.color.a - 0.0025f);
            // when close enough to 0, set booleans and exit
            if (Math.Abs(pickupNotifier.color.a - 0) < 0.005f)
            {
                isPickupNotifierTextVisible = false;
                fadeOutPickupNotifier = false;
            }
        }
    }


    private void ResetColor()
    {
        pickupNotifier.color =
                new Color(pickupNotifier.color.r, pickupNotifier.color.g, pickupNotifier.color.b, 1f);
    }

    // reset all properties, values, and set the outline to transparent
    // also stop all coroutines used for pickup stacking
    private void ResetScore()
    {
        StopAllCoroutines();
        highScoreNumber.enabled = false;
        highScoreText.enabled = false;
        outlineAnimator.Play("ResetColorAnim");
        hasDoubleScorePickupActive = false;
        hasHalfScorePickupActive = false;
        pickupsCollectedCounter = 0;
        lastKnownPosition = 0;
        scoreNumber.text = "0";
        scoreMultiplier.text = "";
        pickupNotifier.text = "";
        highScoreNumber.text = "";
    }
}