using System;
using TMPro;
using UnityEngine;

public class QAtest : MonoBehaviour
{

    public TextMeshProUGUI hptext;
    public TextMeshProUGUI bosstext;
    private PlayerHp playerHp;
    private BaseBoss boss;
    // Update is called once per frame

    private void Start()
    {
        playerHp = FindAnyObjectByType<PlayerHp>();
        boss = FindAnyObjectByType<BaseBoss>();
    }

    void Update()
    {
        if (playerHp == null || boss == null)
        {
            playerHp = FindAnyObjectByType<PlayerHp>();
            boss = FindAnyObjectByType<BaseBoss>();
        }
        
        if (playerHp == null || boss == null) return;
        if (hptext != null && bosstext != null)
        {
            hptext.text = "HP: " + playerHp.CurrentHealth + " / " + playerHp.MaxHealth;
            bosstext.text = "HP: " + boss.CurrentHealth + " / " + boss.MaxHealth;
        }
        else
        {
            Debug.LogWarning("hptext is not assigned in the inspector.");
        }
    }
    
    
}
