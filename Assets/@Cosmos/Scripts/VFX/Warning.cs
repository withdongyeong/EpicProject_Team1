using UnityEngine;

public class Warning : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public float animationDuration = 0.8f; // 총 애니메이션 시간
    public float startScale = 1.4f; // 시작 크기
    public float startAlpha = 0.5f; // 시작 알파값
    [SerializeField] private AnimationCurve scaleCurve; // 크기 변화 곡선
    [SerializeField] private AnimationCurve alphaCurve; // 알파 변화 곡선
    
    private SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    private Color originalColor;
    private float animationTimer = 0f;
    private bool isAnimating = false;

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
        
        // AnimationCurve가 설정되지 않았다면 기본값 설정
        if (scaleCurve == null || scaleCurve.keys.Length == 0)
        {
            scaleCurve = new AnimationCurve();
            scaleCurve.AddKey(0f, 0f); // 시작에서 0 리턴 = Lerp(1.4, 1, 0) = 1.4배
            scaleCurve.AddKey(1f, 1f); // 끝에서 1 리턴 = Lerp(1.4, 1, 1) = 1배
        }
        
        if (alphaCurve == null || alphaCurve.keys.Length == 0)
        {
            alphaCurve = new AnimationCurve();
            alphaCurve.AddKey(0f, 0f); // 시작에서 0 리턴 = Lerp(0.5, 1, 0) = 0.5 알파
            alphaCurve.AddKey(1f, 1f); // 끝에서 1 리턴 = Lerp(0.5, 1, 1) = 1 알파
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
    /// </summary>
    public void StartWarningAnimation()
    {
        if (spriteRenderer == null) return;
        
        // 초기 설정
        transform.localScale = originalScale * startScale; // 크게 시작
        
        // 알파값만 변경하고 색상은 원래 색 유지
        Color startColor = originalColor;
        startColor.a = startAlpha;
        spriteRenderer.color = startColor;
        
        animationTimer = 0f;
        isAnimating = true;
    }

    private void UpdateAnimation()
    {
        animationTimer += Time.deltaTime;
        float progress = animationTimer / animationDuration;
        
        if (progress >= 1f)
        {
            // 애니메이션 완료
            progress = 1f;
            isAnimating = false;
        }
        
        // 크기 애니메이션 (크게 시작해서 원래 크기로)
        float scaleProgress = scaleCurve.Evaluate(progress);
        float currentScaleMultiplier = Mathf.Lerp(startScale, 1f, scaleProgress); // 1.4f → 1f
        transform.localScale = originalScale * currentScaleMultiplier;
        
        // 알파 애니메이션 (0.5에서 1로)
        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(startAlpha, 0.6f, alphaProgress); // 0.5f → 1f
        
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
        transform.localScale = originalScale;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴 시 원래 상태로 복원
        ResetToOriginal();
    }
}