using UnityEngine;

public class LevelClear : MonoBehaviour
{
    public Transform player;
    private const float CLEARER_OFFSET_Y = -10.45f;

    public void Start()
    {
        transform.position = new Vector3(0f, CLEARER_OFFSET_Y, 0f);
    }
    void FixedUpdate()
    {
        transform.position = new Vector3(0f, player.position.y + CLEARER_OFFSET_Y, 0f);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LevelPrefab"))
        {
            Destroy(collision.transform.root.gameObject);
        }
    }
}
