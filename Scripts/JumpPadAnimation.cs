using UnityEngine;

public class JumpPadAnimation : MonoBehaviour
{

    public Animator jumpPadAnimator;
    private const string jumpAnimation = "JumpPadAnimationNew";
    private const float OBJECT_DESTRUCTION_DELAY = 0.25f;

    private void Start()
    {
        jumpPadAnimator.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jumpPadAnimator.Play(jumpAnimation);
            
            Invoke("DestroyCurrentJumpPad", OBJECT_DESTRUCTION_DELAY);
        }
    }

    private void DestroyCurrentJumpPad()
    {
        Destroy(gameObject);
    }
}