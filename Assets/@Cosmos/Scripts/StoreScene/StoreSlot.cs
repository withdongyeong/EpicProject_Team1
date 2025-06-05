using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreSlot : MonoBehaviour
{
    private int objectCost;
    private GameObject objectPrefab;
    private bool isPurchased = false;
    private Image image;
    private StoreDragSystem _storeDragSystem;


    private void Awake()
    {
        image = GetComponent<Image>();
        _storeDragSystem = GetComponent<StoreDragSystem>();
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
    
    public void BuyObject()
    {
        if (isPurchased)
        {
            Debug.Log("이미 구매한 오브젝트입니다.");
            return;
        }

        isPurchased = true; // 구매 상태로 변경
        image.color = Color.gray; // 색상 변경
        Debug.Log($"오브젝트 구매 완료: {objectPrefab.name} (가격: {objectCost})");
    }
    public void SetSlot(int cost, GameObject prefab)
    {
        Debug.Log(cost);
        this.objectCost = cost;
        this.objectPrefab = prefab;
        isPurchased = false; // 초기화
        image.color = Color.white; // 초기 색상 설정
        image.sprite = prefab.GetComponent<ItemObject>().GetSprite(); // 아이템 오브젝트의 스프라이트 설정
    }
}
