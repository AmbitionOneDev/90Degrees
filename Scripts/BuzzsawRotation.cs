using UnityEngine;

public class BuzzsawRotation : MonoBehaviour
{
    public bool shouldMove = false;
    public Animator movingBuzzsawAnimator;

    private readonly float rotationSpeed = 300f;
    public void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, rotationSpeed) * Time.fixedDeltaTime);
        if (shouldMove)
        {
            movingBuzzsawAnimator.Play("MovingBuzzsaw");
        }
    }
}
