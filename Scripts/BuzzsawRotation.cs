using UnityEngine;

public class BuzzsawRotation : MonoBehaviour
{
    private float rotationSpeed = 300f;
    public void FixedUpdate()
    {
        transform.Rotate(new Vector3(0f, 0f, rotationSpeed) * Time.fixedDeltaTime);    
    }
}
