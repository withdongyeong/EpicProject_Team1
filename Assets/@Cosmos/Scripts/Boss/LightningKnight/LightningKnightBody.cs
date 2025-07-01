using UnityEngine;

public class LightningKnightBody : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHp playerHp = collision.GetComponent<PlayerHp>();

        if (playerHp != null)
        {
            playerHp.TakeDamage(20);
        }
    }
}
