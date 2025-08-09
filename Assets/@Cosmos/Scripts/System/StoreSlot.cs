using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreSlot : MonoBehaviour
{
    private int objectCost;
    private GameObject objectPrefab;
    private bool isPurchased = false;
    public bool IsPurchased => isPurchased;
    private Image image;
    private HoverTileInfo hoverTileInfo;
    private int slotNum;
    public int SlotNum => slotNum;
    [SerializeField]
    private Image backgroundImage;

    private TextMeshProUGUI priceText;
    public Action _onPurchase;


    private void Awake()
    {
        image = GetComponent<Image>();
        backgroundImage = transform.parent.GetComponent<Image>();
        hoverTileInfo = GetComponent<HoverTileInfo>();
        slotNum = transform.parent.parent.GetSiblingIndex();
        priceText = transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>();
        EventBus.SubscribeGoldChanged(SetPriceTextColor);
    }

    public GameObject GetObject()
    {
        if (isPurchased)
        {
            Debug.Log("이미 구매한 오브젝트입니다.");
            return null;
        }
        return objectPrefab; // 오브젝트 반환
    }
    
    
    public bool CanPurchase()
    {
        // 이미 구매한 오브젝트라면
        if (isPurchased)
        {
            Debug.Log("이미 구매한 오브젝트입니다.");
            return false;
        }

        // 돈이 충분하다면
        if (GoldManager.Instance.CurrentGold >= objectCost)
        {
            return true;
        }
        else // 돈이 부족하다면
        {
            Debug.Log("돈이 부족합니다.");
            return false;
        }
    }
    /// <summary>
    /// 물건을 사는 메서드입니다
    /// </summary>
    /// <returns></returns>
    public bool BuyObject()
    {
        //만약 이미 산거라면
        if (isPurchased)
        {
            Debug.Log("이미 구매한 오브젝트입니다.");
            return false;
        }

        //돈이 지불 된다면
        if(GoldManager.Instance.UseCurrentGold(objectCost))
        {
            isPurchased = true; // 구매 상태로 변경
            image.color = Color.gray; // 색상 변경
            priceText.color = Color.gray; //가격 텍스트 색상 변경
            _onPurchase?.Invoke();
            if ((StoreLockManager.Instance.GetStoreLocks(SlotNum) != null))
            {
                StoreLockManager.Instance.RemoveStoreLock(SlotNum);
            }
            //Debug.Log($"오브젝트 구매 완료: {objectPrefab.name} (가격: {objectCost})");
            string tileName = objectPrefab.GetComponent<TileObject>().GetTileData().TileName;
            GameManager.Instance.LogHandler.AddPurchasedTile(tileName);
            PurchasedTileManager.Instance.AddPurchasedTiles(tileName);
            return true;
        }
        else // 돈이 없어요
        {
            return false;

        }

        
    }
    public void SetSlot(int cost, GameObject prefab)
    {
        this.objectCost = cost;
        this.objectPrefab = prefab;
        isPurchased = false;
        image.color = Color.white;
        image.sprite = prefab.GetComponent<TileObject>().GetTileSprite();
        hoverTileInfo.SetTileObject(prefab.GetComponent<TileObject>());
        image.SetNativeSize();
        backgroundImage.GetComponent<RectTransform>().sizeDelta = image.rectTransform.sizeDelta;
   
        // 태그 아이콘과 함께 가격 표시
        string tagIcon = GetCategoryIcon(prefab.GetComponent<TileObject>().data.tileCategory);
        if (!string.IsNullOrEmpty(tagIcon))
        {
            priceText.text = $"{cost}G <sprite name=\"{tagIcon}\">";
        }
        else
        {
            priceText.text = $"{cost}G";
        }
   
        SetPriceTextColor(GoldManager.Instance.CurrentGold);
    }

    /// <summary>
    /// Description에서 첫 번째 스프라이트 태그를 추출합니다
    /// </summary>
    private string GetFirstTagFromDescription(string description)
    {
        var regex = new System.Text.RegularExpressions.Regex(@"<sprite name=""(.*?)"">");
        var match = regex.Match(description);
        return match.Success ? match.Groups[1].Value : "";
    }
    
    /// <summary>
    /// TileCategory에 따른 스프라이트 이름을 반환합니다
    /// </summary>
    private string GetCategoryIcon(TileCategory category)
    {
        switch (category)
        {
            case TileCategory.Weapon: return "Weapon";
            case TileCategory.MagicCircle: return "MagicCircle";
            case TileCategory.Armor: return "Armor";
            case TileCategory.Consumable: return "Consumable";
            case TileCategory.Trinket: return "Trinket";
            case TileCategory.Summon: return "Summon";
            case TileCategory.Planet: return "Planet";
            default: return "";
        }
    }


    private void SetPriceTextColor(int gold)
    {
        if(objectCost > gold)
        {
            priceText.color = Color.red;
        }
        else
        {
            priceText.color = Color.white;
        }
        if(isPurchased)
        {
            priceText.color = Color.gray;
        }
    }

    public string GetObjectName()
    {
        return objectPrefab.name;
    }

    public void SetSlotPurchased()
    {
        isPurchased = true; // 구매 상태로 변경
        image.color = Color.gray; // 색상 변경
        priceText.color = Color.gray; //가격 텍스트 색상 변경
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeGoldChanged(SetPriceTextColor);
    }
}
