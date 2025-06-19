using UnityEngine;

public class Warning : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public float totalAnimationDuration = 0.8f; // 총 애니메이션 시간
    public float firstPhaseDuration = 0.7f; // 첫 번째 단계 시간 (1.2 -> 0.8)
    public float startScale = 1.2f; // 시작 크기
    public float middleScale = 0.8f; // 중간 크기 (0.7초 시점)
    public float finalScale = 1f; // 최종 크기 (0.8초 시점)
    public float startAlpha = 0.1f; // 시작 알파값
    [SerializeField] private AnimationCurve scaleCurve; // 크기 변화 곡선 (첫 번째 단계용)
    [SerializeField] private AnimationCurve alphaCurve; // 알파 변화 곡선
    
    [Header("스프라이트 설정")]
    public Sprite finalSprite; // 마지막 단계에서 변경될 스프라이트
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Color originalColor;
    private Sprite originalSprite; // 원래 스프라이트 저장
    private float animationTimer = 0f;
    private bool isAnimating = false;
    private bool spriteChanged = false; // 스프라이트 변경 여부 추적

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Warning: SpriteRenderer가 없습니다!");
            return;
        }
        
        // 원래 값들 저장
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;
        originalSprite = spriteRenderer.sprite; // 원래 스프라이트 저장
        
        // AnimationCurve가 설정되지 않았다면 기본값 설정
        if (scaleCurve == null || scaleCurve.keys.Length == 0)
        {
            scaleCurve = new AnimationCurve();
            scaleCurve.AddKey(0f, 0f); // 시작에서 0 리턴
            scaleCurve.AddKey(1f, 1f); // 끝에서 1 리턴
        }
        
        if (alphaCurve == null || alphaCurve.keys.Length == 0)
        {
            alphaCurve = new AnimationCurve();
            alphaCurve.AddKey(0f, 0f); // 시작에서 0 리턴
            alphaCurve.AddKey(1f, 1f); // 끝에서 1 리턴
        }
    }

    private void Start()
    {
        StartWarningAnimation();
    }

    private void Update()
    {
        if (isAnimating)
        {
            UpdateAnimation();
        }
    }

    /// <summary>
    /// 경고 애니메이션 시작
    /// 1.2 -> 0.7초 동안 0.8로 축소 -> 0.8초에 1.0으로 팝업 + 스프라이트 변경
    /// </summary>
    public void StartWarningAnimation()
    {
        if (spriteRenderer == null) return;
        
        // 초기 설정
        transform.localScale = originalScale * startScale; // 1.2배로 크게 시작
        spriteRenderer.sprite = originalSprite; // 원래 스프라이트로 초기화
        spriteChanged = false; // 스프라이트 변경 플래그 초기화
        
        // 알파값만 변경하고 색상은 원래 색 유지
        Color startColor = originalColor;
        startColor.a = startAlpha;
        spriteRenderer.color = startColor;
        
        animationTimer = 0f;
        isAnimating = true;
    }

    /// <summary>
    /// 2단계 스케일 애니메이션 + 스프라이트 변경 업데이트
    /// </summary>
    private void UpdateAnimation()
    {
        animationTimer += Time.deltaTime;
        float progress = animationTimer / totalAnimationDuration;
        
        if (progress >= 1f)
        {
            // 애니메이션 완료
            progress = 1f;
            isAnimating = false;
        }
        
        // 스케일 애니메이션 - 2단계로 나눔
        float currentScaleMultiplier;
        
        if (animationTimer <= firstPhaseDuration)
        {
            // 첫 번째 단계: 1.2 -> 0.8 (0~0.7초)
            float firstPhaseProgress = animationTimer / firstPhaseDuration;
            float scaleProgress = scaleCurve.Evaluate(firstPhaseProgress);
            currentScaleMultiplier = Mathf.Lerp(startScale, middleScale, scaleProgress);
        }
        else
        {
            // 두 번째 단계: 0.8 -> 1.0 (0.7~0.8초, 즉시 팝업)
            currentScaleMultiplier = finalScale;
            
            // 스프라이트 변경 (한 번만 실행)
            if (!spriteChanged && finalSprite != null)
            {
                spriteRenderer.sprite = finalSprite;
                spriteChanged = true;
            }
        }
        
        transform.localScale = originalScale * currentScaleMultiplier;
        
        // 알파 애니메이션 (전체 시간에 걸쳐 진행)
        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(startAlpha, 0.6f, alphaProgress);
        
        Color currentColor = originalColor;
        currentColor.a = currentAlpha;
        spriteRenderer.color = currentColor;
    }

    /// <summary>
    /// 애니메이션을 다시 시작하는 메서드 (외부에서 호출 가능)
    /// </summary>
    public void ReplayAnimation()
    {
        StartWarningAnimation();
    }

    /// <summary>
    /// 즉시 원래 상태로 복원
    /// </summary>
    public void ResetToOriginal()
    {
        isAnimating = false;
        spriteChanged = false;
        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.sprite = originalSprite; // 원래 스프라이트로 복원
        }
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 원래 상태로 복원
        ResetToOriginal();
    }
}