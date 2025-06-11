using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    private Image protectUI;
    private Image hpUI;

    private PlayerHealth playerHealth;
    private PlayerProtection playerProtect;

    private int currentHP;
    private int maxHP;
    private int currentProtect;
    private bool isInitialized = false;

    /// <summary>
    /// 초기 UI 컴포넌트 설정
    /// </summary>
    private void Start()
    {
        Debug.Log("PlayerHPUI Start() 호출됨");
        
        protectUI = transform.GetChild(0).GetComponent<Image>();
        hpUI = transform.GetChild(1).GetComponent<Image>();
        
        // 플레이어가 생성될 때까지 기다림
        TryInitializePlayerComponents();
    }

    /// <summary>
    /// 매 프레임 초기화 시도 및 UI 업데이트
    /// </summary>
    private void Update()
    {
        if (!isInitialized)
        {
            TryInitializePlayerComponents();
        }
        
        if (isInitialized)
        {
            ChangeBar();
        }
    }

    /// <summary>
    /// 플레이어 컴포넌트들을 찾아서 초기화 시도
    /// </summary>
    private void TryInitializePlayerComponents()
    {
        if (playerHealth == null)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }
        
        if (playerProtect == null)
        {
            playerProtect = FindAnyObjectByType<PlayerProtection>();
        }
        
        // 둘 다 찾았으면 초기화 완료
        if (playerHealth != null && playerProtect != null)
        {
            maxHP = playerHealth.MaxHealth;
            isInitialized = true;
            Debug.Log("PlayerHPUI 초기화 완료!");
        }
    }

    /// <summary>
    /// 바를 현재 HP에 맞춰서 변경하는 메서드
    /// </summary>
    private void ChangeBar()
    {
        if (!isInitialized) return;

        currentHP = playerHealth.CurrentHealth;
        currentProtect = playerProtect.ProtectionAmount;

        float maxHPFillAmount = (float)maxHP / (float)(maxHP + currentProtect);
        hpUI.fillAmount = maxHPFillAmount * currentHP / maxHP;
        protectUI.fillAmount = (1 - maxHPFillAmount) + hpUI.fillAmount;
    }
}