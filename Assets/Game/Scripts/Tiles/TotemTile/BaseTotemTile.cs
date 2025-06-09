//using UnityEngine;

//public class BaseTotemTile : BaseTile
//{
//    private GameObject _totem;

//    private InventoryItemData _itemData;

//    private TotemManager _totemManager;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        //나중에 이걸 기믹 매니저에 요청하는 식으로 바꿀수도 있습니다. 토템 매니저가 항상 존재하는게 아니라면.
//        //_totemManager = FindAnyObjectByType<TotemManager>();
//    }

//    public override void Activate()
//    {
//        base.Activate();
//        if (GetState() == TileState.Activated && _totem != null)
//        {
//            SummonTotem();
//        }
//    }

//    protected void SummonTotem()
//    {
//        _totemManager = FindAnyObjectByType<TotemManager>();
//        if(_totemManager != null)
//        {
//            var summonedTotem = Instantiate(_totem, _totemManager.transform);
//            summonedTotem.transform.localPosition = Vector3.zero;
//            summonedTotem.GetComponent<BaseTotem>().InitializeTotem(_itemData);
//        }

//    }

//    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
//    {
//        base.ModifyTilePropertiesByItemData(itemData);
//        if (itemData.Summon == null)
//        {
//            Debug.LogWarning("itemData.Summon is null!");
//        }
//        else
//        {
//            Debug.Log("Summon prefab: " + itemData.Summon.name);
//        }
        
//        _totem = itemData.Summon;
//        _itemData = itemData;
//        //_itemData = ScriptableObject.Instantiate(itemData);
//    }
//}
