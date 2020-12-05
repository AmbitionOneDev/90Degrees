using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public Transform player;
    private const float CAMERA_PLAYER_OFFSET_Z = -7f;

    // For some reason there is a .45 padding of some sort
    private const float CAMERA_PLAYER_OFFSET_Y = 4.45f;

    public void Start()
    {
        transform.position = new Vector3(0f, CAMERA_PLAYER_OFFSET_Y, CAMERA_PLAYER_OFFSET_Z);
    }
    void Update()
    {
        // Offset the Camera
        transform.position = new Vector3(0f, player.position.y + CAMERA_PLAYER_OFFSET_Y, CAMERA_PLAYER_OFFSET_Z);
    }
}
