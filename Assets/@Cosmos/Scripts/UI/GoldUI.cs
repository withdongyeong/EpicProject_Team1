using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    TextMeshProUGUI goldText;

    private void Awake()
    {
        EventBus.SubscribeGoldChanged(ChangeGoldUI);
        goldText = GetComponent<TextMeshProUGUI>();
    }
    
    private void ChangeGoldUI(int gold)
    {
        goldText.text = $"Gold : {gold}";
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeGoldChanged(ChangeGoldUI);
    }
}
