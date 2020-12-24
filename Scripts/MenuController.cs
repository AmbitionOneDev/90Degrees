using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject returnButton;
    public Animator animator;
    public Camera cam;
    public GameObject modalAboutUs;
    public Animator modalAnimator;


    // Animation names
    private string statsAnim;
    private string shopAnim;
    private string settingsAnim;

    // Auxiliaries
    private string lastPlayedAnimation;
    private string boolName;

    // Settings variables
    private bool returnIsEnabled;
    private bool isMusicEnabled = true;
    private bool isSoundEnabled = true;
    private bool isTutorialEnabled = true;
    private bool isOn = false;

    void Start()
    {
        // use the 21:9 animation for taller aspect ratios, else use 16:9
        // add these lines and anims for the other 2 as well
        statsAnim = (cam.aspect < (9f / 16.1f) && cam.aspect > (9f / 21.1f)) ? "StatsWindowAnimationTall" : "StatsWindowAnimation";
        settingsAnim = (cam.aspect < (9f / 16.1f) && cam.aspect > (9f / 21.1f)) ? "SettingsWindowAnimationTall" : "SettingsWindowAnimation";

        returnIsEnabled = false;
        returnButton.SetActive(false);
        modalAboutUs.SetActive(false);
    }

    public void SetStatsBool()
    {
        boolName = "goToStats";
        animator.SetBool(boolName, true);
    }
    
    public void SetSettingsBool()
    {
        boolName = "goToSettings";
        animator.SetBool(boolName, true);
    }
    
    public void SetShopBool()
    {
        boolName = "goToShop";
        animator.SetBool(boolName, true);
    }

    public void SwitchToStats()
    {
        SetStatsBool();
        if (animator.GetBool("goToStats"))
        {
            animator.Play(statsAnim);
            ToggleReturn();
            lastPlayedAnimation = statsAnim;
        }
    }

    // dodaj kasnije
    public void SwitchToSettings() {
        SetSettingsBool();
        if (animator.GetBool("goToSettings"))
        {
            animator.Play(settingsAnim);
            ToggleReturn();
            lastPlayedAnimation = settingsAnim;
        }
    }
    public void SwitchToShop() { }
    

    // bring back the starting buttons
    public void ResetView()
    {
        // reset the bool
        animator.SetBool(boolName, false);

        // play the reversed version of the last played animation
        animator.Play(lastPlayedAnimation + "Reverse");

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
        Debug.Log("Toggled music.");
        isMusicEnabled = !isMusicEnabled;
    }

    public void ToggleSound()
    {
        Debug.Log("Toggled sound.");
        isSoundEnabled = !isSoundEnabled;
    }

    public void ToggleTutorial()
    {
        Debug.Log("Toggled tutorial.");
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
}
