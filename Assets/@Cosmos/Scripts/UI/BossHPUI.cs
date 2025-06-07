using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHPUI : MonoBehaviour
{
    private BaseBoss _enemy; // 연결된 적 참조
    private Image hpUI; // 체력바 이미지
    private GameObject debuffPanel; // 디버프 UI 패널
    private GameObject[] debuffUI; // 디버프 UI 프리팹 배열
    private Dictionary<BossDebuff, (GameObject icon, GameObject text)> activeDebuffUIs; // 활성 디버프 UI 추적

    // 체력 관련 변수
    private int currentHP;
    private int maxHP;

    private void Awake()
    {
        hpUI = transform.GetChild(1).GetComponent<Image>();
        debuffPanel = transform.GetChild(2).gameObject;
        debuffUI = Resources.LoadAll<GameObject>("Prefabs/UI");
        activeDebuffUIs = new Dictionary<BossDebuff, (GameObject icon, GameObject text)>();
    }

    private void Start()
    {
        // 최대 체력 설정
        _enemy = FindAnyObjectByType<BaseBoss>();
        if (_enemy != null)
        {
            maxHP = _enemy.MaxHealth;
        }
    }

    /// <summary>
    /// 매 프레임 체력 업데이트
    /// </summary>
    private void Update()
    {
        if (_enemy == null)
            return;

        ChangeBar();
    }

    /// <summary>
    /// 체력바를 현재 체력에 맞게 업데이트합니다.
    /// </summary>
    private void ChangeBar()
    {
        // 현재 체력 업데이트
        currentHP = _enemy.CurrentHealth;
        hpUI.fillAmount = (float)currentHP / (float)(maxHP);
    }

    /// <summary>
    /// 디버프 UI를 업데이트합니다.
    /// </summary>
    /// <param name="debuff">업데이트할 디버프</param>
    /// <param name="count">디버프 개수</param>
    public void UpdateDebuffUI(BossDebuff debuff, int count)
    {
        if (count <= 0)
        {
            RemoveDebuffUI(debuff);
            return;
        }

        if (activeDebuffUIs.ContainsKey(debuff))
        {
            UpdateDebuffText(debuff, count);
        }
        else
        {
            CreateDebuffUI(debuff, count);
        }
    }

    /// <summary>
    /// 지정된 디버프의 UI를 생성합니다.
    /// </summary>
    private void CreateDebuffUI(BossDebuff debuff, int count)
    {
        GameObject iconInstance = null;
        GameObject textInstance = null;
        int iconIndex = -1;
        int textIndex = -1;

        switch (debuff)
        {
            case BossDebuff.Burning:
                iconIndex = 0;
                textIndex = 1;
                break;
            case BossDebuff.Frostbite:
                iconIndex = 2;
                textIndex = 3;
                break;
            default:
                return; // 지원하지 않는 디버프는 무시
        }

        if (IsValidPrefab(iconIndex))
        {
            iconInstance = Instantiate(debuffUI[iconIndex], debuffPanel.transform);
            iconInstance.name = $"{debuff}Icon";
            iconInstance.SetActive(true);
        }

        if (IsValidPrefab(textIndex))
        {
            textInstance = Instantiate(debuffUI[textIndex], debuffPanel.transform);
            textInstance.name = $"{debuff}Text";
            TextMeshProUGUI tmpText = textInstance.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = count.ToString();
            }
            textInstance.SetActive(true);
        }

        activeDebuffUIs[debuff] = (iconInstance, textInstance);
    }

    /// <summary>
    /// 디버프 텍스트를 업데이트합니다.
    /// </summary>
    private void UpdateDebuffText(BossDebuff debuff, int count)
    {
        if (activeDebuffUIs.TryGetValue(debuff, out var ui))
        {
            if (ui.text != null)
            {
                TextMeshProUGUI tmpText = ui.text.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    tmpText.text = count.ToString();
                }
            }
        }
    }

    /// <summary>
    /// 프리팹이 유효한지 확인합니다.
    /// </summary>
    private bool IsValidPrefab(int index)
    {
        return index >= 0 && index < debuffUI.Length && debuffUI[index] != null;
    }

    /// <summary>
    /// 지정된 디버프의 UI를 제거합니다.
    /// </summary>
    private void RemoveDebuffUI(BossDebuff debuff)
    {
        if (activeDebuffUIs.ContainsKey(debuff))
        {
            var (icon, text) = activeDebuffUIs[debuff];
            if (icon != null)
            {
                Destroy(icon);
            }
            if (text != null)
            {
                Destroy(text);
            }
            activeDebuffUIs.Remove(debuff);
        }
    }
}