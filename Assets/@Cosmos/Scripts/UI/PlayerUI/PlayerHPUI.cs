using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    //hp와 protect의 잔량을 표시하는 이미지 UI입니다
    private Image protectUI;
    private Image hPUI;

    //hp와 보호막 수치가 들어가있는 스크립트입니다
    private PlayerHp _playerHp;
    private PlayerProtection playerProtect;

    //현재 hp, 최대 hp, 현재 보호막 수치입니다
    private int currenthP;
    private int maxHP;
    private int currentProtect;


    private void Awake()
    {
        EventBus.SubscribeGameStart(Init);
    }
    
    private void Update()
    {
        if(_playerHp == null) return;
        ChangeBar();
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(Init);
    }
    
    
    private void Init()
    {
        //초기화하는 과정입니다. UI들과 _playerHp,PlayerProtection을 씬에서 찾아 가져옵니다
        protectUI = transform.GetChild(0).GetComponent<Image>();
        hPUI = transform.GetChild(1).GetComponent<Image>();

        _playerHp = FindAnyObjectByType<PlayerHp>();
        maxHP = _playerHp.MaxHealth;
        playerProtect = FindAnyObjectByType<PlayerProtection>();
    }

    /// <summary>
    /// 바를 현재 HP에 맞춰서 변경하는 메서드입니다
    /// </summary>
    private void ChangeBar()
    {
        //현재 HP와 현재 보호막을 가져옵니다
        currenthP = _playerHp.CurrentHealth;
        currentProtect = playerProtect.ProtectionAmount;

        //최대 HP + 현재 보호막에서 최대 HP가 차지하는 분량을 계산합니다
        float maxHPFillAmount = (float)maxHP / (float)(maxHP + currentProtect);
        //최대 HP가 차지하는 분량에서 현재 HP가 차지하는 분량만큼 hpUI를 채웁니다
        hPUI.fillAmount = maxHPFillAmount * currenthP / maxHP;
        //현재 HP가 차지하는 분량 + 현재 보호막이 차지하는 분량만큼 보호막 UI를 채웁니다
        protectUI.fillAmount = (1 - maxHPFillAmount) + hPUI.fillAmount;
    }
}
