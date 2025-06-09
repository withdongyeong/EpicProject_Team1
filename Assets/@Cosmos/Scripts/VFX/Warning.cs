using UnityEngine;

public class Warning : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [SerializeField] private float animationDuration = 0.1f; // 총 애니메이션 시간
    [SerializeField] private float startScale = 1.1f; // 시작 크기 (원래 크기의 1.5배)
    [SerializeField] private AnimationCurve scaleCurve; // 크기 변화 곡선 (1.5배→1배)
    [SerializeField] private AnimationCurve colorCurve; // 색상 변화 곡선 (흰색→원래색)
    
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
            scaleCurve.AddKey(0f, 0f); // 시작에서 0 리턴 = Lerp(1.5, 1, 0) = 1.5배
            scaleCurve.AddKey(1f, 1f); // 끝에서 1 리턴 = Lerp(1.5, 1, 1) = 1배
        }
        
        if (colorCurve == null || colorCurve.keys.Length == 0)
        {
            colorCurve = new AnimationCurve();
            colorCurve.AddKey(0f, 0f); // 시작에서 0 리턴 = Lerp(흰색, 원래색, 0) = 흰색
            colorCurve.AddKey(1f, 1f); // 끝에서 1 리턴 = Lerp(흰색, 원래색, 1) = 원래색
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
        spriteRenderer.color = Color.white; // 흰색으로 시작
        
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
        float currentScaleMultiplier = Mathf.Lerp(startScale, 1f, scaleProgress); // 1.5f → 1f
        transform.localScale = originalScale * currentScaleMultiplier;
        
        // 색상 애니메이션 (흰색에서 원래 색으로)
        float colorProgress = colorCurve.Evaluate(progress);
        Color currentColor = Color.Lerp(Color.white, originalColor, colorProgress);
        spriteRenderer.color = currentColor;
        
        // 디버그용 (필요시 주석 해제)
        // Debug.Log($"Progress: {progress:F2}, Scale: {currentScaleMultiplier:F2}, Color: {currentColor}");
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