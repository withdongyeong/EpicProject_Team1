using UnityEngine;

public class Warning : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public float totalDuration = 0.8f; // 전체 애니메이션 시간
    public float image1CompleteTime = 0.4f; // 이미지1 완료 시간
    public float image2StartTime = 0.4f; // 이미지2 시작 시간
    public float baseShowTime = 0.1f; // 타격 전 base 보이는 시간
    public float startScale = 1.2f; // 시작 크기
    public float image2StartScale = 0.5f; // 이미지2 시작 크기
    public float finalScale = 1f; // 최종 크기
    public float image1StartAlpha = 1f; // 이미지1 시작 알파값
    public float image1EndAlpha = 0.6f; // 이미지1 끝 알파값
    public float image2StartAlpha = 0f; // 이미지2 시작 알파값
    public float image2EndAlpha = 0.6f; // 이미지2 끝 알파값
    [SerializeField] private AnimationCurve scaleCurve; // 크기 변화 곡선
    [SerializeField] private AnimationCurve alphaCurve; // 알파 변화 곡선
    
    [Header("스프라이트 설정")]
    public Sprite image1Sprite; // 첫 번째 이미지
    public Sprite image2Sprite; // 두 번째 이미지
    
    private SpriteRenderer image1Renderer; // 첫 번째 이미지 렌더러
    private SpriteRenderer image2Renderer; // 두 번째 이미지 렌더러
    private SpriteRenderer baseRenderer; // 베이스 이미지 렌더러
    
    private Vector3 originalScale;
    private Color originalColor;
    private float animationTimer = 0f;
    private bool isAnimating = false;

    private void Awake()
    {
        // 자식 오브젝트에서 SpriteRenderer들 가져오기
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        
        if (childRenderers.Length < 3)
        {
            Debug.LogError("Warning: 자식 오브젝트에 SpriteRenderer가 3개 필요합니다!");
            return;
        }
        
        // 첫 번째, 두 번째, 세 번째 자식의 SpriteRenderer
        image1Renderer = childRenderers[0];
        image2Renderer = childRenderers[1];
        baseRenderer = childRenderers[2];
        
        // 원래 값들 저장
        originalScale = transform.localScale;
        originalColor = image1Renderer.color;
        
        // AnimationCurve가 설정되지 않았다면 기본값 설정
        if (scaleCurve == null || scaleCurve.keys.Length == 0)
        {
            scaleCurve = new AnimationCurve();
            scaleCurve.AddKey(0f, 0f);
            scaleCurve.AddKey(1f, 1f);
        }
        
        if (alphaCurve == null || alphaCurve.keys.Length == 0)
        {
            alphaCurve = new AnimationCurve();
            alphaCurve.AddKey(0f, 0f);
            alphaCurve.AddKey(1f, 1f);
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
        if (image1Renderer == null || image2Renderer == null || baseRenderer == null) return;
        
        // 이미지1 초기 설정
        image1Renderer.transform.localScale = Vector3.one * startScale;
        image1Renderer.sprite = image1Sprite;
        Color startColor1 = originalColor;
        startColor1.a = image1StartAlpha;
        image1Renderer.color = startColor1;
        
        // 이미지2 초기 설정 (작은 크기로)
        image2Renderer.transform.localScale = Vector3.one * image2StartScale;
        image2Renderer.sprite = image2Sprite;
        Color startColor2 = originalColor;
        startColor2.a = image2StartAlpha;
        image2Renderer.color = startColor2;
        
        // 베이스 초기 설정 (안보이게)
        baseRenderer.gameObject.SetActive(false);
        
        animationTimer = 0f;
        isAnimating = true;
    }

    /// <summary>
    /// 애니메이션 업데이트
    /// </summary>
    private void UpdateAnimation()
    {
        animationTimer += Time.deltaTime;
        
        // 애니메이션 완료 체크
        if (animationTimer >= totalDuration)
        {
            animationTimer = totalDuration;
            isAnimating = false;
        }
        
        // 이미지1 애니메이션 (0~image1CompleteTime)
        if (animationTimer <= image1CompleteTime)
        {
            UpdateImage1Animation();
        }
        
        // 이미지2 애니메이션 (image2StartTime~totalDuration)
        if (animationTimer >= image2StartTime)
        {
            UpdateImage2Animation();
        }
        
        // 베이스 표시 (타격 직전 0.1초)
        float baseShowStartTime = totalDuration - baseShowTime;
        if (animationTimer >= baseShowStartTime && !baseRenderer.gameObject.activeInHierarchy)
        {
            baseRenderer.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 이미지1 애니메이션
    /// </summary>
    private void UpdateImage1Animation()
    {
        float progress = animationTimer / image1CompleteTime;
        progress = Mathf.Clamp01(progress);
        
        // 스케일 애니메이션
        float scaleProgress = scaleCurve.Evaluate(progress);
        float currentScaleMultiplier = Mathf.Lerp(startScale, finalScale, scaleProgress);
        image1Renderer.transform.localScale = Vector3.one * currentScaleMultiplier;
        
        // 알파 애니메이션
        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(image1StartAlpha, image1EndAlpha, alphaProgress);
        
        Color currentColor = originalColor;
        currentColor.a = currentAlpha;
        image1Renderer.color = currentColor;
    }
    
    /// <summary>
    /// 이미지2 애니메이션
    /// </summary>
    private void UpdateImage2Animation()
    {
        float image2Timer = animationTimer - image2StartTime;
        float image2Duration = totalDuration - image2StartTime - baseShowTime; // base 보이기 전까지만
        float progress = image2Timer / image2Duration;
        progress = Mathf.Clamp01(progress);
        
        // 스케일 애니메이션 (base 나타나기 전에 최대크기 완료)
        float scaleProgress = scaleCurve.Evaluate(progress);
        float currentScaleMultiplier = Mathf.Lerp(image2StartScale, finalScale, scaleProgress);
        image2Renderer.transform.localScale = Vector3.one * currentScaleMultiplier;
        
        // 알파 애니메이션
        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(image2StartAlpha, image2EndAlpha, alphaProgress);
        
        Color currentColor = originalColor;
        currentColor.a = currentAlpha;
        image2Renderer.color = currentColor;
    }

    /// <summary>
    /// 애니메이션을 다시 시작하는 메서드
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
        
        if (image1Renderer != null)
        {
            image1Renderer.color = originalColor;
            image1Renderer.sprite = image1Sprite;
            image1Renderer.transform.localScale = Vector3.one;
        }
        
        if (image2Renderer != null)
        {
            Color resetColor = originalColor;
            resetColor.a = image2StartAlpha;
            image2Renderer.color = resetColor;
            image2Renderer.transform.localScale = Vector3.one * image2StartScale;
        }
        
        if (baseRenderer != null)
        {
            baseRenderer.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        ResetToOriginal();
    }
}