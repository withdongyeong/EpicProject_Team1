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
        //Debug.Log(cost);
        this.objectCost = cost;
        this.objectPrefab = prefab;
        isPurchased = false; // 초기화
        image.color = Color.white; // 초기 색상 설정
        image.sprite = prefab.GetComponent<TileObject>().GetTileSprite(); // 아이템 오브젝트의 스프라이트 설정
        //infoUI.SetTileObject(prefab.GetComponent<TileObject>()); // InfoUI에 TileObject 설정
        hoverTileInfo.SetTileObject(prefab.GetComponent<TileObject>());
        image.SetNativeSize();
        backgroundImage.GetComponent<RectTransform>().sizeDelta = image.rectTransform.sizeDelta; // 배경 이미지 크기 조정
        priceText.text = $"{cost}G";
        SetPriceTextColor(GoldManager.Instance.CurrentGold);
        
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

    private void OnDestroy()
    {
        EventBus.UnsubscribeGoldChanged(SetPriceTextColor);
    }
}
