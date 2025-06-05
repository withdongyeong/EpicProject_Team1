using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    private Image protectUI;
    private Image hPUI;

    private PlayerHealth playerHealth;
    private PlayerProtection playerProtect;

    private int currenthP;
    private int maxHP;
    private int currentProtect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        protectUI = transform.GetChild(0).GetComponent<Image>();
        hPUI = transform.GetChild(1).GetComponent<Image>();

        playerHealth = FindAnyObjectByType<PlayerHealth>();
        maxHP = playerHealth.MaxHealth;
        playerProtect = FindAnyObjectByType<PlayerProtection>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeBar();
    }

    private void ChangeBar()
    {
        currenthP = playerHealth.CurrentHealth;
        currentProtect = playerProtect.ProtectionAmount;
        float maxHPFillAmount = (float)maxHP / (float)(maxHP + currentProtect);
        hPUI.fillAmount = maxHPFillAmount * currenthP / maxHP;
        protectUI.fillAmount = (1 - maxHPFillAmount) + hPUI.fillAmount;
    }
}
