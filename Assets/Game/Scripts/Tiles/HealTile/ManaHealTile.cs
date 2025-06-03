using UnityEngine;

public class ManaHealTile : HealTile
{

    private PlayerMana _playerMana;

    private void Start()
    {
        _playerMana = FindAnyObjectByType<PlayerMana>();
    }

    /// <summary>
    /// 타일 발동 - 플레이어 체력 회복
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && _playerMana != null)
        {
            HealPlayer();
        }
    }

    /// <summary>
    /// 플레이어 마나 회복 처리
    /// </summary>
    private void HealPlayer()
    {
        // 플레이어 체력 회복
        _playerMana.ModifyCurrentMana(HealAmount);

        // 회복 이펙트 생성
        CreateHealEffect();
    }

    /// <summary>
    /// 회복 이펙트 생성
    /// </summary>
    private void CreateHealEffect()
    {
        if (_healEffectPrefab != null && _playerMana != null)
        {
            // 플레이어 위치에 회복 이펙트 생성
            GameObject effectObj = Instantiate(
                _healEffectPrefab,
                _playerMana.transform.position,
                Quaternion.identity
            );

            // 일정 시간 후 이펙트 제거
            Destroy(effectObj, 0.1f);
        }
    }

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _healAmount = itemData.HealAmount;
    }
}
