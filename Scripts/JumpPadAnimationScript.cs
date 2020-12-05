using UnityEngine;

public class JumpPadAnimationScript : MonoBehaviour
{

    public Animator jumpPadAnimator;
    private const float OBJECT_DESTRUCTION_DELAY = 0.25f;

    private void Start()
    {
        jumpPadAnimator.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jumpPadAnimator.Play("JumpPadAnimation");
            
            Invoke("DestroyCurrentJumpPad", OBJECT_DESTRUCTION_DELAY);
            
        }
    }

    private void DestroyCurrentJumpPad()
    {
        Destroy(gameObject);
    }
}