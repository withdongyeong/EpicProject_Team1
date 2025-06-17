using UnityEngine;

public class SlimeActtckTentacle : MonoBehaviour
{
    private int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHp player = other.GetComponent<PlayerHp>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
