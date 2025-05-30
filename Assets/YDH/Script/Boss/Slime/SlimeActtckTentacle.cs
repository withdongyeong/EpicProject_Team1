using UnityEngine;

public class SlimeActtckTentacle : MonoBehaviour
{
    private int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
