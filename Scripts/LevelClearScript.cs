using UnityEngine;

public class LevelClearScript : MonoBehaviour
{
    public Transform player;
    private const float CLEARER_OFFSET_Y = -10.45f;

    public void Start()
    {
        transform.position = new Vector3(0f, CLEARER_OFFSET_Y, 0f);
    }
    void Update()
    {
        // Offset the Clearer
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
