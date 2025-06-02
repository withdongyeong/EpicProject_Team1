using UnityEngine;

public class HealTotem : BaseTotem
{
    private PlayerHealth _playerHealth;

    private int _healAmount;

    [SerializeField] protected GameObject _healEffectPrefab;


    public override void InitializeTotem(InventoryItemData itemData)
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
        _healAmount = itemData.HealAmount;
        base.InitializeTotem(itemData);
    }

    public override void ActivateTotem()
    {
        HealPlayer();
    }

    public override void ActivateTotemBetter()
    {
        base.ActivateTotemBetter();
    }

    /// <summary>
    /// 플레이어 체력 회복 처리
    /// </summary>
    private void HealPlayer()
    {
        // 플레이어 체력 회복
        _playerHealth.Heal(_healAmount);

        // 회복 이펙트 생성
        CreateHealEffect();
    }

    /// <summary>
    /// 회복 이펙트 생성
    /// </summary>
    private void CreateHealEffect()
    {
        if (_healEffectPrefab != null && _playerHealth != null)
        {
            // 플레이어 위치에 회복 이펙트 생성
            GameObject effectObj = Instantiate(
                _healEffectPrefab,
                _playerHealth.transform.position,
                Quaternion.identity
            );

            // 일정 시간 후 이펙트 제거
            Destroy(effectObj, 0.1f);
        }
    }
}
