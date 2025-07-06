using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileChanceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI chanceText;

    private void Awake()
    {
        chanceText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        chanceText.gameObject.SetActive(true);
        ShopChanceClass chanceList = GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum];
        string text = $"일반 확률: {chanceList.shop_NormalChance}\n<color=blue>희귀 확률</color>: {chanceList.shop_RareChance}\n<color=#8000FF>영웅 확률</color>: {chanceList.shop_EpicChance}" +
            $"\n<color=#FFEB04>전설 확률</color>: {chanceList.shop_LegendaryChance}\n<color=#A6E6D9>신화 확률</color>: {chanceList.Shop_MythicChance()}";
        chanceText.text = text;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chanceText.gameObject.SetActive(false);
    }


}
