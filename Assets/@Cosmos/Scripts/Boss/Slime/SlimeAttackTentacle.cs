using UnityEngine;

public class SlimeAttackTentacle : MonoBehaviour
{
    private int damage = 20;

    void OnTriggerEnter2D(Collider2D other)
    {
        damage = GlobalSetting.Instance.GetBossBalance(1).strongDamage;
        PlayerHp player = other.GetComponent<PlayerHp>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
