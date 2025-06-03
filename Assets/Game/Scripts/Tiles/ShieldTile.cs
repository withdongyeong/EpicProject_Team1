using UnityEngine;

public class ShieldTile : BaseTile
{
    [SerializeField] private int _shieldAmount = 5;

    private PlayerShield _playerShield;

    public int ShieldDuration { get => _shieldAmount; set => _shieldAmount = value; }

    private void Start()
    {
        _playerShield = FindAnyObjectByType<PlayerShield>();
    }

    /// <summary>
    /// 타일 발동 - 플레이어 방어 효과 부여
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && _playerShield != null)
        {
            GrantShield();
        }
    }

    /// <summary>
    /// 플레이어에게 방어 상태 부여
    /// </summary>
    private void GrantShield()
    {
        _playerShield.SetShield(true, _shieldAmount);
    }

    

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _shieldAmount = itemData.ShieldAmount;
    }
}