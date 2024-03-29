﻿using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public Animator GameStarterAnimator;
    public Rigidbody2D player;
    public Player playerScript;
    public static bool isStarted = false;


    public void Start()
    {
        player.constraints = RigidbodyConstraints2D.FreezePositionY;
        playerScript.enabled = false;
    }

    public void GameStarterFunction()
    {
        if (!isStarted)
        {
            GameStarterAnimator.enabled = true;
            GameStarterAnimator.Play("LevelStartAnim");
            Invoke("SetSpeed", 1f);
        }

    }

    public void SetSpeed()
    {
        playerScript.enabled = true;
        player.constraints = RigidbodyConstraints2D.None;
        player.constraints = RigidbodyConstraints2D.FreezeRotation;
        isStarted = true;
    }
}