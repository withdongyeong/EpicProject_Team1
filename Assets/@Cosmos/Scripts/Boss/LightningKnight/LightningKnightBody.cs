using UnityEngine;

public class LightningKnightBody : MonoBehaviour
{
    /// <summary>
    /// 현재 안씀
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // PlayerHp playerHp = collision.GetComponent<PlayerHp>();
        //
        // if (playerHp != null)
        // {
        //     playerHp.TakeDamage(20);
        // }
    }
}
