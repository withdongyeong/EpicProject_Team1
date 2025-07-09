using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class TileChanceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI chanceText;

    public LocalizedString NomallocalizedString;
    public LocalizedString RarelocalizedString;
    public LocalizedString EpiclocalizedString;
    public LocalizedString LegendarylocalizedString;
    public LocalizedString MythiclocalizedString;

    private void Awake()
    {
        chanceText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        chanceText.gameObject.SetActive(true);
        ShopChanceClass chanceList = GlobalSetting.Shop_ChanceList[StageSelectManager.Instance.StageNum];

        string Probabailitytext = "";

        NomallocalizedString.StringChanged += (text) => {
            Probabailitytext += text.Replace("{0}", (chanceList.shop_NormalChance.ToString())) + "\n";
        };
        RarelocalizedString.StringChanged += (text) => {
            Probabailitytext += text.Replace("{0}", (chanceList.shop_RareChance.ToString())) + "\n";
        };
        EpiclocalizedString.StringChanged += (text) => {
            Probabailitytext += text.Replace("{0}", (chanceList.shop_EpicChance.ToString())) + "\n";
        };
        LegendarylocalizedString.StringChanged += (text) => {
            Probabailitytext += text.Replace("{0}", (chanceList.shop_LegendaryChance.ToString())) + "\n";
        };
        MythiclocalizedString.StringChanged += (text) => {
            Probabailitytext += text.Replace("{0}", (chanceList.Shop_MythicChance().ToString()));
        };

        //string text = $"일반 확률: {chanceList.shop_NormalChance}" +
        //    $"\n<color=#B6E6FE>희귀 확률</color>: {chanceList.shop_RareChance}" +
        //    $"\n<color=#E2ACFE>영웅 확률</color>: {chanceList.shop_EpicChance}" +
        //    $"\n<color=#FEB996>전설 확률</color>: {chanceList.shop_LegendaryChance}" +
        //    $"\n<color=#A6E6D9>신화 확률</color>: {chanceList.Shop_MythicChance()}";

        chanceText.text = Probabailitytext;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        chanceText.gameObject.SetActive(false);
    }


}
