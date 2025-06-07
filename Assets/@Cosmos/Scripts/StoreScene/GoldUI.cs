using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    TextMeshProUGUI goldText;

    private void Awake()
    {
        goldText = GetComponent<TextMeshProUGUI>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoldManager.Instance.OnCurrentGoldChange += ChangeGoldUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeGoldUI(int gold)
    {
        goldText.text = $"Gold : {gold}";
    }
}
