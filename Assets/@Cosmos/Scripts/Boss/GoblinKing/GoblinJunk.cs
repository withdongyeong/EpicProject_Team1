using UnityEngine;

public class GoblinJunk : MonoBehaviour
{
    private int damage = 15;

    private void Start()
    {
        Destroy(this.gameObject, 6f);
    }

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
