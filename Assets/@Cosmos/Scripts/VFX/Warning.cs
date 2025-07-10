using UnityEngine;

public class Warning : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public float totalDuration = 0.8f;
    public float image1CompleteTime = 0.4f;
    public float image2StartTime = 0.4f;
    public float baseShowTime = 0.1f;
    public float startScale = 1.2f;
    public float image2StartScale = 0.5f;
    public float finalScale = 1f;
    public float image1StartAlpha = 1f;
    public float image1EndAlpha = 0.6f;
    public float image2StartAlpha = 0f;
    public float image2EndAlpha = 0.6f;
    
    [Header("색상 변화 (image1)")]
    public Color startColor = new Color(0.23f, 0.23f, 0.23f); // #3a3a3a
    public Color endColor = new Color(0.82f, 0.2f, 0.2f);  
    
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve alphaCurve;

    [Header("스프라이트 설정")]
    public Sprite image1Sprite;
    public Sprite image2Sprite;

    private SpriteRenderer image1Renderer;
    private SpriteRenderer image2Renderer;
    private SpriteRenderer baseRenderer;

    private Vector3 originalScale;
    private float animationTimer = 0f;
    private bool isAnimating = false;

    private void Awake()
    {
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (childRenderers.Length < 3)
        {
            Debug.LogError("Warning: 자식 오브젝트에 SpriteRenderer가 3개 필요합니다!");
            return;
        }

        image1Renderer = childRenderers[0];
        image2Renderer = childRenderers[1];
        baseRenderer = childRenderers[2];

        originalScale = transform.localScale;

        if (scaleCurve == null || scaleCurve.keys.Length == 0)
        {
            scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }

        if (alphaCurve == null || alphaCurve.keys.Length == 0)
        {
            alphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
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

    public void StartWarningAnimation()
    {
        if (image1Renderer == null || image2Renderer == null || baseRenderer == null) return;

        // 이미지1 초기 설정
        image1Renderer.transform.localScale = Vector3.one * startScale;
        image1Renderer.sprite = image1Sprite;
        image1Renderer.color = startColor.WithAlpha(image1StartAlpha);

        // 이미지2 초기 설정
        image2Renderer.transform.localScale = Vector3.one * image2StartScale;
        image2Renderer.sprite = image2Sprite;
        image2Renderer.color = new Color(1f, 1f, 1f, image2StartAlpha);

        // 베이스 초기화
        baseRenderer.gameObject.SetActive(false);

        animationTimer = 0f;
        isAnimating = true;
    }

    private void UpdateAnimation()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= totalDuration)
        {
            animationTimer = totalDuration;
            isAnimating = false;
        }

        if (animationTimer <= image1CompleteTime)
        {
            UpdateImage1Animation();
        }

        if (animationTimer >= image2StartTime)
        {
            UpdateImage2Animation();
        }

        // base 표시
        float baseShowStartTime = totalDuration - baseShowTime;
        if (animationTimer >= baseShowStartTime && !baseRenderer.gameObject.activeInHierarchy)
        {
            baseRenderer.gameObject.SetActive(true);
            baseRenderer.transform.localScale = Vector3.one * 2f;
        }

        // base 수축
        if (baseRenderer.gameObject.activeInHierarchy)
        {
            baseRenderer.transform.localScale = Vector3.Lerp(baseRenderer.transform.localScale, Vector3.one, Time.unscaledDeltaTime * 20f);
        }

    }

    private void UpdateImage1Animation()
    {
        float progress = Mathf.Clamp01(animationTimer / image1CompleteTime);

        float scaleProgress = scaleCurve.Evaluate(progress);
        float currentScale = Mathf.Lerp(startScale, finalScale, scaleProgress);
        image1Renderer.transform.localScale = Vector3.one * currentScale;

        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(image1StartAlpha, image1EndAlpha, alphaProgress);

        Color lerpedColor = Color.Lerp(startColor, endColor, progress);
        lerpedColor.a = currentAlpha;
        image1Renderer.color = lerpedColor;
    }

    private void UpdateImage2Animation()
    {
        float image2Timer = animationTimer - image2StartTime;
        float image2Duration = totalDuration - image2StartTime - baseShowTime;
        float progress = Mathf.Clamp01(image2Timer / image2Duration);

        float scaleProgress = scaleCurve.Evaluate(progress);
        float currentScale = Mathf.Lerp(image2StartScale, finalScale, scaleProgress);
        image2Renderer.transform.localScale = Vector3.one * currentScale;

        float alphaProgress = alphaCurve.Evaluate(progress);
        float currentAlpha = Mathf.Lerp(image2StartAlpha, image2EndAlpha, alphaProgress);
        image2Renderer.color = new Color(1f, 1f, 1f, currentAlpha);
    }

    public void ReplayAnimation()
    {
        StartWarningAnimation();
    }

    public void ResetToOriginal()
    {
        isAnimating = false;
        transform.localScale = originalScale;

        if (image1Renderer != null)
        {
            image1Renderer.color = startColor.WithAlpha(image1StartAlpha);
            image1Renderer.sprite = image1Sprite;
            image1Renderer.transform.localScale = Vector3.one;
        }

        if (image2Renderer != null)
        {
            image2Renderer.color = new Color(1f, 1f, 1f, image2StartAlpha);
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

// 헬퍼 확장 메서드
public static class ColorExtensions
{
    public static Color WithAlpha(this Color c, float alpha)
    {
        c.a = alpha;
        return c;
    }
}
