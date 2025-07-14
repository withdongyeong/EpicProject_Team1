using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 보스 HP UI - 체력 감소시 데미지 미리보기, 애니메이션, 흔들림 적용
/// </summary>
public class BossHPUI : MonoBehaviour
{
    private BaseBoss _enemy;
    private Image hpUI;
    private GameObject debuffPanel;
    private GameObject[] debuffUI;
    private Dictionary<BossDebuff, (GameObject icon, GameObject text)> activeDebuffUIs;

    // 체력 관련 변수
    private int currentHP;
    private int maxHP;

    // 데미지 미리보기 UI
    private Image damagePreviewUI;

    private int _previousHP;
    private float _currentPreviewHP; // 프리뷰 UI의 현재 표시 체력
    private float _targetPreviewHP;  // 프리뷰 UI가 도달해야 할 목표 체력

    private Coroutine _currentShakeCoroutine;
    private Coroutine _previewUpdateCoroutine;

    [Header("데미지 미리보기 설정")]
    private Color damagePreviewColor = new Color(0.7f, 0.4f, 0.5f, 1f);
    private float previewDuration = 1f;

    [Header("흔들림 설정 - 3단계")]
    private float smallShakeThreshold = 30f;
    private float mediumShakeThreshold = 100f;
    private float smallShakeIntensity = 5f;
    private float mediumShakeIntensity = 10f;
    private float bigShakeIntensity = 20f;
    private float smallShakeDuration = 0.1f;
    private float mediumShakeDuration = 0.2f;
    private float bigShakeDuration = 0.3f;

    private Vector3 _originalPosition;

    private void Awake()
    {
        EventBus.SubscribeGameStart(Init);
        hpUI = transform.GetChild(1).GetComponent<Image>();
        debuffPanel = transform.GetChild(2).gameObject;
        debuffUI = Resources.LoadAll<GameObject>("Prefabs/UI/BossDebuffUI");
        activeDebuffUIs = new Dictionary<BossDebuff, (GameObject icon, GameObject text)>();
    }

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_enemy == null) return;
        CheckForDamage();
        UpdateBar();
    }

    /// <summary>
    /// 보스 HP 정보 초기화
    /// </summary>
    private void Init()
    {
        _enemy = FindAnyObjectByType<BaseBoss>();
        if (_enemy != null)
        {
            maxHP = _enemy.MaxHealth;
            _previousHP = _enemy.CurrentHealth;
            _currentPreviewHP = _previousHP;
            _targetPreviewHP = _previousHP;
            CreateDamagePreviewUI();
        }
    }

    /// <summary>
    /// 데미지 미리보기 UI 생성
    /// </summary>
    private void CreateDamagePreviewUI()
    {
        GameObject obj = new GameObject("DamagePreview");
        obj.transform.SetParent(hpUI.transform.parent);
        obj.transform.SetSiblingIndex(hpUI.transform.GetSiblingIndex());

        damagePreviewUI = obj.AddComponent<Image>();
        damagePreviewUI.sprite = hpUI.sprite;
        damagePreviewUI.type = Image.Type.Filled;
        damagePreviewUI.fillMethod = hpUI.fillMethod;
        damagePreviewUI.fillOrigin = hpUI.fillOrigin;
        damagePreviewUI.fillClockwise = hpUI.fillClockwise;
        damagePreviewUI.color = damagePreviewColor;

        RectTransform r = damagePreviewUI.GetComponent<RectTransform>();
        RectTransform baseRect = hpUI.GetComponent<RectTransform>();
        r.anchorMin = baseRect.anchorMin;
        r.anchorMax = baseRect.anchorMax;
        r.offsetMin = baseRect.offsetMin;
        r.offsetMax = baseRect.offsetMax;
        r.localScale = baseRect.localScale;
        r.pivot = baseRect.pivot;
        r.sizeDelta = baseRect.sizeDelta;
        r.anchoredPosition = baseRect.anchoredPosition;

        damagePreviewUI.raycastTarget = false;
        damagePreviewUI.enabled = true;
        damagePreviewUI.fillAmount = CalculateHPFillAmount(_previousHP);
    }

    /// <summary>
    /// 체력 감소 감지
    /// </summary>
    private void CheckForDamage()
    {
        int newHP = _enemy.CurrentHealth;
        if (newHP < _previousHP)
        {
            // 데미지를 받았을 때
            float damage = _previousHP - newHP;
            StartShakeEffect(damage);

            // 목표 체력 업데이트 (프리뷰가 향해야 할 체력)
            _targetPreviewHP = newHP;

            // 코루틴 중지 및 재시작 (새 속도 기준으로)
            if (_previewUpdateCoroutine != null)
                StopCoroutine(_previewUpdateCoroutine);

            _previewUpdateCoroutine = StartCoroutine(AnimatePreviewHP(_currentPreviewHP, _targetPreviewHP, previewDuration));
        }

        _previousHP = newHP;
    }

    /// <summary>
    /// 프리뷰 UI 애니메이션 코루틴
    /// </summary>
    private IEnumerator AnimatePreviewHP(float from, float to, float duration)
    {
        float elapsed = 0f;
        float diff = to - from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _currentPreviewHP = from + diff * t;

            damagePreviewUI.fillAmount = CalculateHPFillAmount(Mathf.RoundToInt(_currentPreviewHP));
            yield return null;
        }

        _currentPreviewHP = to;
        damagePreviewUI.fillAmount = CalculateHPFillAmount(Mathf.RoundToInt(_currentPreviewHP));
    }

    /// <summary>
    /// Bar UI 업데이트
    /// </summary>
    private void UpdateBar()
    {
        currentHP = _enemy.CurrentHealth;
        // 실제 HP바는 즉시 업데이트
        hpUI.fillAmount = CalculateHPFillAmount(currentHP);
    }

    /// <summary>
    /// 현재 체력 기반으로 fillAmount 비율 계산
    /// </summary>
    private float CalculateHPFillAmount(int hp)
    {
        return (float)hp / maxHP;
    }

    /// <summary>
    /// 첫 데미지 진입 시 흔들림
    /// </summary>
    private void StartShakeEffect(float damage)
    {
        if (_currentShakeCoroutine != null)
            StopCoroutine(_currentShakeCoroutine);

        (float intensity, float duration) = GetShake(damage);
        _currentShakeCoroutine = StartCoroutine(ShakeUI(intensity, duration));
    }

    /// <summary>
    /// 데미지 크기에 따른 흔들림 강도 반환
    /// </summary>
    private (float, float) GetShake(float damage)
    {
        if (damage < smallShakeThreshold) return (smallShakeIntensity, smallShakeDuration);
        if (damage < mediumShakeThreshold) return (mediumShakeIntensity, mediumShakeDuration);
        return (bigShakeIntensity, bigShakeDuration);
    }

    /// <summary>
    /// UI 흔들기 코루틴
    /// </summary>
    private IEnumerator ShakeUI(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = _originalPosition + new Vector3(
                UnityEngine.Random.Range(-intensity, intensity),
                UnityEngine.Random.Range(-intensity, intensity),
                0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
        _currentShakeCoroutine = null;
    }

    /// <summary>
    /// 디버프 UI 업데이트
    /// </summary>
    public void UpdateDebuffUI(BossDebuff debuff, int count)
    {
        if(activeDebuffUIs.ContainsKey(debuff) || debuff == BossDebuff.TemporaryCurse)
        {
            if(count<=0)
            {
                RemoveDebuffUI(debuff);
            }
            else
            {
                UpdateDebuffText(debuff, count);
            }
        }
        else
        {
            CreateDebuffUI(debuff, count);
        }
        
    }

    /// <summary>
    /// 디버프 UI 생성
    /// </summary>
    private void CreateDebuffUI(BossDebuff debuff, int count)
    {
        GameObject iconInstance = null;
        GameObject textInstance = null;
        int iconIndex = -1;
        int textIndex = -1;

        switch (debuff)
        {
            case BossDebuff.Burning: iconIndex = 0; textIndex = 1; break;
            case BossDebuff.Frostbite: iconIndex = 2; textIndex = 3; break;
            case BossDebuff.Mark: iconIndex = 4; textIndex = 5; break;
            case BossDebuff.Curse: iconIndex = 6; textIndex = 7; break;
            case BossDebuff.Pain: iconIndex = 8; textIndex = 9; break;
            default: return;
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
            if (tmpText != null) tmpText.text = count.ToString();
            textInstance.SetActive(true);
        }

        activeDebuffUIs[debuff] = (iconInstance, textInstance);
    }

    /// <summary>
    /// 디버프 수치 텍스트 갱신
    /// </summary>
    private void UpdateDebuffText(BossDebuff debuff, int count)
    {
        //만약 들어온 디버프가 동상 화염 저주 낙인 고통 이런거면
        if (activeDebuffUIs.TryGetValue(debuff, out var ui) && ui.text != null)
        {
            TextMeshProUGUI tmpText = ui.text.GetComponent<TextMeshProUGUI>();
            if (tmpText != null)
            {
                if(debuff == BossDebuff.Curse)
                {
                    //임시 저주가 0이면 뒤에 (+0)이게 안생기게 합니다.
                    int tempCurse = _enemy.GetDebuffCount(BossDebuff.TemporaryCurse);
                    if(tempCurse != 0)
                    {
                        tmpText.text = count.ToString() + " (+" + _enemy.GetDebuffCount(BossDebuff.TemporaryCurse) + ")";
                    }
                    else
                    {
                        tmpText.text = count.ToString();
                    }
                    
                }
                else
                {
                    tmpText.text = count.ToString();
                }
            }
        }
        else if(debuff == BossDebuff.TemporaryCurse) //얘는 별개 아이콘이 없어용
        {
            if(!activeDebuffUIs.TryGetValue(BossDebuff.Curse, out var result))
            {
                UpdateDebuffUI(BossDebuff.Curse, _enemy.GetDebuffCount(BossDebuff.Curse));
            }
            UpdateDebuffText(BossDebuff.Curse, _enemy.GetDebuffCount(BossDebuff.Curse)); //저주 아이콘을 반영하게 하면 자동으로 임시저주도 적용
        }
    }

    /// <summary>
    /// 디버프 UI 제거
    /// </summary>
    private void RemoveDebuffUI(BossDebuff debuff)
    {
        if (activeDebuffUIs.TryGetValue(debuff, out var ui))
        {
            if (ui.icon != null) Destroy(ui.icon);
            if (ui.text != null) Destroy(ui.text);
            activeDebuffUIs.Remove(debuff);
        }
        else if(debuff == BossDebuff.TemporaryCurse)
        {
            UpdateDebuffUI(BossDebuff.Curse, _enemy.GetDebuffCount(BossDebuff.Curse));
        }
    }

    /// <summary>
    /// 프리팹 유효성 검사
    /// </summary>
    private bool IsValidPrefab(int index)
    {
        return index >= 0 && index < debuffUI.Length && debuffUI[index] != null;
    }

    private void OnDestroy()
    {
        if (_currentShakeCoroutine != null) StopCoroutine(_currentShakeCoroutine);
        if (_previewUpdateCoroutine != null) StopCoroutine(_previewUpdateCoroutine);
        EventBus.UnsubscribeGameStart(Init);
    }
}
