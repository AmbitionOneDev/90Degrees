using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    // dodano zbog removedoublescore odnosno removehalfscore
    private bool hasDoubleScorePickupActive = false;
    private bool hasHalfScorePickupActive = false;
    private int score = 0;

    // Score-related pickups duration in seconds
    private const float SCORE_MULTIPLIER_CHANGE_DURATION = 5f;


    private const int DEFAULT_SCORE = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        /********************************************************
         * ako je lik pokupio neki od pickupa
         * pa nakon toga pokupio suprotni pickup
         * makni efekte tog pickupa
         * i dodaj efekte tog novopokupljenog 
         * TO MORAS DODAT, JOS NIJE DODANO
         ********************************************************/
        switch (collision.tag)
        {
            // If the player picks up the 2x pickup
            case "DoubleScore":

                // Destroy the picked up object
                Destroy(collision.gameObject);

                hasDoubleScorePickupActive = true;

                // FIX THIS, ADD PROPER IMPLEMENTATION
                // USIING A METHOD
                Debug.Log("Poduplavam score");

                // Reset the score multiplier
                Invoke("RemoveDoubleScorePickup", SCORE_MULTIPLIER_CHANGE_DURATION);
                break;

            // If the player picks up the 0.5x pickup
            case "HalfScore":

                // Destroy the picked up object
                Destroy(collision.gameObject);

                hasHalfScorePickupActive = true;

                // FIX THIS, ADD PROPER IMPLEMENTATION
                // USIING A METHOD
                Debug.Log("Rezem score na pola");

                // Reset the score multiplier
                Invoke("RemoveHalfScorePickup", SCORE_MULTIPLIER_CHANGE_DURATION);
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*********************************************DODAJ DA SE SVAKIH 10M DODA 10 BODOVA (NEKI COUNTER, % OD POSITION.Y I TAKO TO, YOULL FIGURE IT OUT MORON)************************/
    //private void FixedUpdate()
    //{
    //    if (hasDoubleScorePickupActive)
    //    {
    //        score += 20;
    //    }
    //    else if (hasHalfScorePickupActive)
    //    {
    //        score += 5;
    //    }
    //    else
    //    {
    //        score += DEFAULT_SCORE;
    //    }
    //}

    

    /// <summary>
    /// Napravit ces int counter preko kojeg ces onda pratit koliko puta je pokupljen double score
    /// i ovisno o counteru ces povećat odnosno smanjit multiplier, kao i vrijeme trajanja
    /// NAZOVI JE REMOVEDOUBLESCORE JER U INVOKE NE MOZES PREDAT METODE KOJE PRIMAJU PARAMETAR!!
    /// </summary>
    void RemoveDoubleScorePickup()
    {
        Debug.Log("Score multiplier resetan");
        hasDoubleScorePickupActive = false;
    }

    void RemoveHalfScorePickup()
    {
        Debug.Log("Score multiplier resetan");
        hasHalfScorePickupActive = false;
    }
}
