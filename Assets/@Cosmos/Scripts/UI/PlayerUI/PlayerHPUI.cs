using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    //hp와 protect의 잔량을 표시하는 이미지 UI입니다
    private Image protectUI;
    private Image hPUI;
    
    // 데미지 미리보기용 추가 UI
    private Image damagePreviewUI; // 받을 데미지를 미리 표시하는 UI

    //hp와 보호막 수치가 들어가있는 스크립트입니다
    private PlayerHp _playerHp;
    private PlayerProtection playerProtect;

    //현재 hp, 최대 hp, 현재 보호막 수치입니다
    private int currenthP;
    private int maxHP;
    private int currentProtect;
    
    // 이전 체력 값 (데미지 감지용)
    private int _previousHP;
    
    // 데미지 애니메이션 상태
    private bool _isDamageAnimating = false;
    private float _currentDisplayHP;
    private int _targetHP;

    [Header("UI 설정")]
    public bool isMinimal = false; // true면 플레이어를 따라다님
    public Vector3 offsetPosition = new Vector3(0, 2, 0); // 플레이어 위 오프셋
    public float uiScale = 0.005f; // UI 크기 조절

    [Header("데미지 미리보기 설정")]
    private Color damagePreviewColor = new Color(0.7f, 0.4f, 0.5f, 1f);
    private float shakeIntensity = 10f;
    private float shakeDuration = 0.2f;
    private float damageAnimationDuration = 0.4f;

    [Header("위험 상태 설정")]
    private float dangerHealthThreshold = 0.3f;
    private float blinkInterval = 0.3f;
    private Color dangerColor = new Color(1f, 0.5f, 0f, 1f);

    private Vector3 _originalPosition;
    private Color _originalHPColor;
    private bool _isDangerState = false;
    private Coroutine _dangerBlinkCoroutine;

    private void Awake()
    {
        EventBus.SubscribeGameStart(Init);
    }

    private void Start()
    {
        _originalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (_playerHp == null) return;
        CheckForDamage();
        UpdateBar();
        UpdateDangerState();
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(Init);
        if (_dangerBlinkCoroutine != null) StopCoroutine(_dangerBlinkCoroutine);
    }

    private void Init()
    {
        protectUI = transform.GetChild(0).GetComponent<Image>();
        hPUI = transform.GetChild(1).GetComponent<Image>();

        GameObject damagePreviewObj = new GameObject("DamagePreview");
        damagePreviewObj.transform.SetParent(transform);
        damagePreviewObj.transform.SetSiblingIndex(1);

        damagePreviewUI = damagePreviewObj.AddComponent<Image>();
        damagePreviewUI.sprite = hPUI.sprite;
        damagePreviewUI.type = Image.Type.Filled;
        damagePreviewUI.fillMethod = hPUI.fillMethod;
        damagePreviewUI.color = damagePreviewColor;

        RectTransform damageRect = damagePreviewUI.GetComponent<RectTransform>();
        RectTransform hpRect = hPUI.GetComponent<RectTransform>();
        damageRect.anchorMin = hpRect.anchorMin;
        damageRect.anchorMax = hpRect.anchorMax;
        damageRect.offsetMin = hpRect.offsetMin;
        damageRect.offsetMax = hpRect.offsetMax;
        damageRect.localScale = hpRect.localScale;

        damagePreviewUI.fillAmount = 0;
        _originalHPColor = hPUI.color;

        _playerHp = FindAnyObjectByType<PlayerHp>();
        if (_playerHp != null)
        {
            maxHP = _playerHp.MaxHealth;
            _previousHP = _playerHp.CurrentHealth;
            _currentDisplayHP = _previousHP;
            playerProtect = FindAnyObjectByType<PlayerProtection>();

            if (isMinimal)
            {
                SetupFollowPlayer();
            }
        }
    }

    /// <summary>
    /// 데미지 감지 및 미리보기 시작
    /// </summary>
    private void CheckForDamage()
    {
        if (_playerHp == null) return;

        int currentHP = _playerHp.CurrentHealth;

        // 체력이 감소한 경우에만 미리보기 시퀀스 작동
        if (!_isDamageAnimating && currentHP < _previousHP)
        {
            StartDamageSequence(_previousHP, currentHP);
        }

        // 단순히 감소했을 때만 이전값을 갱신
        if (currentHP != _previousHP)
        {
            _previousHP = currentHP;
        }
    }


    /// <summary>
    /// 데미지 시퀀스 시작
    /// </summary>
    private void StartDamageSequence(int fromHP, int toHP)
    {
        _isDamageAnimating = true;
        _targetHP = toHP;

        // 1. 즉시 HP UI 줄이기
        currenthP = toHP;
        hPUI.fillAmount = CalculateHPFillAmount(toHP);

        // 2. 데미지 미리보기 UI 설정
        ShowDamagePreview(fromHP);

        // 3. 흔들기 효과
        StartCoroutine(ShakeUI());

        // 4. 미리보기 → 애니메이션
        StartCoroutine(DamagePreviewAnimate(fromHP, toHP));
    }

    /// <summary>
    /// 데미지 미리보기 설정
    /// </summary>
    private void ShowDamagePreview(int fromHP)
    {
        damagePreviewUI.fillAmount = CalculateHPFillAmount(fromHP);
        _currentDisplayHP = fromHP;
    }

    /// <summary>
    /// 미리보기 UI 애니메이션
    /// </summary>
    private IEnumerator DamagePreviewAnimate(int fromHP, int toHP)
    {
        float fromFill = CalculateHPFillAmount(fromHP);
        float toFill = CalculateHPFillAmount(toHP);
        float elapsed = 0f;

        while (elapsed < damageAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / damageAnimationDuration;
            damagePreviewUI.fillAmount = Mathf.Lerp(fromFill, toFill, t);
            yield return null;
        }

        damagePreviewUI.fillAmount = toFill;
        _isDamageAnimating = false;
    }

    /// <summary>
    /// UI 흔들기 효과
    /// </summary>
    private IEnumerator ShakeUI()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                UnityEngine.Random.Range(-shakeIntensity, shakeIntensity),
                0);
            transform.localPosition = _originalPosition + offset;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localPosition = _originalPosition;
    }

    /// <summary>
    /// 위험 상태 깜박임 확인
    /// </summary>
    private void UpdateDangerState()
    {
        if (_playerHp == null) return;

        float ratio = (float)_playerHp.CurrentHealth / maxHP;
        bool isDanger = ratio < dangerHealthThreshold;

        if (isDanger && !_isDangerState)
        {
            _isDangerState = true;
            StartDangerBlink();
        }
        else if (!isDanger && _isDangerState)
        {
            _isDangerState = false;
            StopDangerBlink();
        }
    }

    private void StartDangerBlink()
    {
        if (_dangerBlinkCoroutine != null) StopCoroutine(_dangerBlinkCoroutine);
        _dangerBlinkCoroutine = StartCoroutine(DangerBlinkCoroutine());
    }

    private void StopDangerBlink()
    {
        if (_dangerBlinkCoroutine != null) StopCoroutine(_dangerBlinkCoroutine);
        _dangerBlinkCoroutine = null;
        hPUI.color = _originalHPColor;
    }

    private IEnumerator DangerBlinkCoroutine()
    {
        bool on = false;
        while (_isDangerState)
        {
            hPUI.color = on ? dangerColor : _originalHPColor;
            on = !on;
            yield return new WaitForSecondsRealtime(blinkInterval);
        }
    }

    /// <summary>
    /// HP 수치를 기반으로 fillAmount 계산
    /// </summary>
    private float CalculateHPFillAmount(int hp)
    {
        if (playerProtect == null) return (float)hp / maxHP;

        int protect = playerProtect.ProtectionAmount;
        float hpRatio = (float)maxHP / (maxHP + protect);
        return hpRatio * hp / maxHP;
    }

    /// <summary>
    /// Bar UI 업데이트
    /// </summary>
    private void UpdateBar()
    {
        if (_playerHp == null || playerProtect == null) return;

        currentProtect = playerProtect.ProtectionAmount;
        float hpRatio = (float)maxHP / (maxHP + currentProtect);

        if (!_isDamageAnimating)
        {
            currenthP = _playerHp.CurrentHealth;
            hPUI.fillAmount = hpRatio * currenthP / maxHP;
        }

        protectUI.fillAmount = (1 - hpRatio) + hPUI.fillAmount;
    }

    /// <summary>
    /// 미니멀 모드일 경우 따라다니도록 설정
    /// </summary>
    private void SetupFollowPlayer()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
        {
            SetupWorldSpaceCanvas();
        }
        else
        {
            transform.SetParent(_playerHp.transform);
            transform.localPosition = offsetPosition;
        }
    }

    private void SetupWorldSpaceCanvas()
    {
        GameObject worldCanvas = new GameObject("PlayerUI_WorldCanvas");
        worldCanvas.transform.SetParent(_playerHp.transform);
        worldCanvas.transform.localPosition = offsetPosition;

        Canvas newCanvas = worldCanvas.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.WorldSpace;
        newCanvas.worldCamera = Camera.main;

        CanvasScaler scaler = worldCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.dynamicPixelsPerUnit = 64;

        RectTransform canvasRect = worldCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(108, 21);

        worldCanvas.transform.localScale = new Vector3(uiScale, uiScale, uiScale);

        transform.SetParent(worldCanvas.transform);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
