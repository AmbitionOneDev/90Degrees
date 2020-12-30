using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject returnButton;
    public Animator animator;
    public GameObject modalAboutUs;
    public Animator modalAnimator;
    public Player playerScript;

    // Animation names
    private readonly string shopAnim = "ShopWindowAnimation";
    private readonly string statsAnim = "StatsWindowAnimation";
    private readonly string settingsAnim = "SettingsWindowAnimation";

    // Auxiliaries
    private string lastPlayedAnimation;
    private string boolName;

    // Settings variables
    private bool returnIsEnabled;
    private bool isMusicEnabled = true;
    private bool isSoundEnabled = true;
    private bool isTutorialEnabled = true;
    private bool isOn = false;

    public void Start()
    {
        returnIsEnabled = false;
        returnButton.SetActive(false);
        modalAboutUs.SetActive(false);
    }

    public void SetAnimBool(string boolName)
    {
        this.boolName = boolName;
        animator.SetBool(boolName, true);
    }

    public void SwitchToStats()
    {
        SetAnimBool("goToStats");
        animator.Play(statsAnim);
        ToggleReturn();
        lastPlayedAnimation = statsAnim;
    }

    public void SwitchToSettings()
    {
        SetAnimBool("goToSettings");
        animator.Play(settingsAnim);
        ToggleReturn();
        lastPlayedAnimation = settingsAnim;
    }
    public void SwitchToShop() {
        SetAnimBool("goToShop");
        animator.Play(shopAnim);
        ToggleReturn();
        lastPlayedAnimation = shopAnim;
    }


    // bring back the starting buttons
    public void ResetView()
    {
        // reset the bool
        animator.SetBool(boolName, false);

        // play the reversed version of the last played animation
        animator.Play(lastPlayedAnimation + "Reversed");

        ToggleReturn();
    }

    private void ToggleReturn()
    {
        returnIsEnabled = !returnIsEnabled;
        returnButton.SetActive(returnIsEnabled);
    }

    // za ove dvije ces morat koristit jos neki audio manager, pogledaj unity tutoriale za glazbu i zvukove
    public void ToggleMusic()
    {
        isMusicEnabled = !isMusicEnabled;
    }

    public void ToggleSound()
    {
        isSoundEnabled = !isSoundEnabled;
    }

    public void ToggleTutorial()
    {
        isTutorialEnabled = !isTutorialEnabled;
    }

    public void ToggleCredits()
    {
        // if modal window is disabled at the start, enable it
        if (!modalAboutUs.activeSelf) modalAboutUs.SetActive(true);

        if (!isOn)
            modalAnimator.Play("Fade-in");
        else
            modalAnimator.Play("Fade-out");
        isOn = !isOn;
    }

    public void PauseGame()
    {
        animator.Play("PauseMenuAnim");
        playerScript.isPaused = true;
        playerScript.StopMovement();
        playerScript.enabled = false;
    }

    public void ContinueGame()
    {
        animator.Play("PauseMenuAnimReversed");
        playerScript.enabled = true;
        playerScript.isPaused = false;
        playerScript.ResetMovement();
    }
    

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameStarter.isStarted = false;
    }
}
