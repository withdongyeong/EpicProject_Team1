using UnityEngine;

public class DeathArea : MonoBehaviour
{
    bool IsStayPlayer;
    PlayerHp playerHp;
    private int _damage;

    private string PatternName = "7_1";
    private void Start()
    {
        playerHp = FindAnyObjectByType<PlayerHp>();
        _damage = GlobalSetting.Instance.GetBossBalance(7).weakDamage;
    }

    private void Update()
    {
        if(IsStayPlayer && playerHp.CurrentHealth > 0) playerHp.TakeDamage(_damage, patternName:PatternName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        PlayerController playerController = collision.GetComponent<PlayerController>();

        Vector3 PlayerPosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerController.CurrentX, playerController.CurrentY, 0));

        if (playerController != null && PlayerPosition == this.transform.position)
        {
            IsStayPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        PlayerController playerController = collision.GetComponent<PlayerController>();

        Vector3 PlayerPosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerController.CurrentX, playerController.CurrentY, 0));

        if (playerController != null && PlayerPosition != this.transform.position)
        {
            IsStayPlayer = false;
        }
    }
}
