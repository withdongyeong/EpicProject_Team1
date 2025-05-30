using UnityEngine;

/// <summary>
/// 인벤토리 아이템 데이터 클래스
/// </summary>
///
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItemData : ScriptableObject
{
    public string itemName; // 아이템 이름
}