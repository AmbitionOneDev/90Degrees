using UnityEngine;

public class TriggerSpike : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<Animator>().Play("LepezaRotation");
        }
    }
}
