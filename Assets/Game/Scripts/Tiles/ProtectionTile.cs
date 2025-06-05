using UnityEngine;

public class ProtectionTile : BaseTile
{
    [SerializeField] private int _protectionAmount = 5;
   
    private PlayerProtection _playerProtection;
    
    public int ProtectionAmount { get => _protectionAmount; set => _protectionAmount = value; }
    
    private void Start()
    {
        _playerProtection = FindAnyObjectByType<PlayerProtection>();
    }
    
    /// <summary>
    /// 타일 발동 - 플레이어 보호 효과 부여
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && _playerProtection != null)
        {
            GrantProtection();
        }
    }
    
    /// <summary>
    /// 플레이어에게 보호 상태 부여
    /// </summary>
    private void GrantProtection()
    {
        _playerProtection.SetProtection(true, _protectionAmount);
    }

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _protectionAmount = itemData.ProtectionAmount;
    }
}