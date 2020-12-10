using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject returnButton;
    public Animator animator;
    public Camera cam;
    
    private string boolName;
    private bool returnIsEnabled;

    // Animation names
    private string statsAnim;
    private string shopAnim;
    private string settingsAnim;

    private string lastPlayedAnimation;

    

    void Start()
    {
        // use the 21:9 animation for taller aspect ratios, else use 16:9
        // add these lines and anims for the other 2 as well
        statsAnim = (cam.aspect < (9f / 16.1f) && cam.aspect > (9f / 21.1f)) ? "StatsWindowAnimationTall" : "StatsWindowAnimation";
        
        returnIsEnabled = false;
        returnButton.SetActive(false);    
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
    public void SwitchToSettings() { }
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
}
