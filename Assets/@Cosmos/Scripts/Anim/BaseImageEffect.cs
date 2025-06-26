using UnityEngine;
using UnityEngine.UI;

public class FadeTwinkle : MonoBehaviour
{
    [Header("페이드 반짝임 설정")]
    [Range(0.3f, 0.9f)]
    public float minAlpha = 0.6f;         // 최소 투명도 (페이드될 때)
    
    [Range(0.9f, 1f)]
    public float maxAlpha = 1f;           // 최대 투명도 (원래 상태)
    
    [Range(0.5f, 3f)]
    public float fadeSpeed = 1.2f;        // 페이드 속도
    
    [Range(0f, 2f)]
    public float randomOffset = 0.5f;     // 랜덤 오프셋
    
    [Header("색상 페이드")]
    public bool enableColorFade = true;
    public Color baseColor = Color.white;
    public Color fadeColor = Color.gray;  // 페이드될 때 색상 (어두운 색)
    
    [Header("글로우 효과 (UI만)")]
    public bool enableGlow = false;
    public float glowIntensity = 0.5f;
    
    private Image imageComponent;
    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup;
    
    private float timeOffset;
    private Color originalColor;
    private bool isInitialized = false;

    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        // 컴포넌트들 찾기
        imageComponent = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // 원본 색상 저장
        if (imageComponent != null)
        {
            originalColor = imageComponent.color;
            if (baseColor == Color.white) baseColor = originalColor;
        }
        else if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            if (baseColor == Color.white) baseColor = originalColor;
        }
        
        // 랜덤 시작 오프셋
        timeOffset = Random.Range(0f, randomOffset);
        
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        
        // 사인파를 이용한 부드러운 페이드 효과 (엇박 패턴)
        float fadeValue = (Mathf.Sin((Time.time + timeOffset) * fadeSpeed + Mathf.PI) + 1f) / 2f;
        
        // 투명도 변화 (페이드 -> 원래)
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, fadeValue);
        
        // 색상 페이드 (어두운 색상 -> 원래 색상)
        Color currentColor = enableColorFade ? 
            Color.Lerp(fadeColor, baseColor, fadeValue) : 
            originalColor;
        currentColor.a = currentAlpha;
        
        // 컴포넌트별 적용
        if (canvasGroup != null)
        {
            canvasGroup.alpha = currentAlpha;
        }
        
        if (imageComponent != null)
        {
            imageComponent.color = currentColor;
            
            // UI 글로우 효과 (페이드 시에만)
            if (enableGlow)
            {
                // Outline 컴포넌트가 있다면 색상 적용
                var outline = GetComponent<Outline>();
                if (outline != null)
                {
                    Color glowColor = fadeColor;
                    glowColor.a = fadeValue * glowIntensity;
                    outline.effectColor = glowColor;
                }
                
                // Shadow 컴포넌트가 있다면 색상 적용
                var shadow = GetComponent<Shadow>();
                if (shadow != null)
                {
                    Color shadowColor = fadeColor;
                    shadowColor.a = fadeValue * 0.3f;
                    shadow.effectColor = shadowColor;
                }
            }
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = currentColor;
        }
    }
    
    // 페이드 효과 활성화/비활성화
    public void SetFadeEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled)
        {
            // 원래 상태로 복원
            if (canvasGroup != null)
                canvasGroup.alpha = maxAlpha;
                
            if (imageComponent != null)
                imageComponent.color = originalColor;
            else if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
    
    // 페이드 속도 변경
    public void SetFadeSpeed(float speed)
    {
        fadeSpeed = speed;
    }
    
    // 페이드 강도 설정
    public void SetFadeIntensity(float colorFade)
    {
        // 색상 강도는 fadeColor의 명도로 조절
        if (enableColorFade)
        {
            Color.RGBToHSV(fadeColor, out float h, out float s, out float v);
            fadeColor = Color.HSVToRGB(h, s, v * colorFade);
        }
    }
    
    // 즉시 페이드 상태로
    public void ForceFade()
    {
        if (!isInitialized) return;
        
        Color fadedColor = enableColorFade ? fadeColor : originalColor;
        fadedColor.a = minAlpha;
        
        if (canvasGroup != null)
            canvasGroup.alpha = minAlpha;
            
        if (imageComponent != null)
            imageComponent.color = fadedColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = fadedColor;
    }
    
    // 즉시 원래 상태로
    public void ForceNormal()
    {
        if (!isInitialized) return;
        
        if (canvasGroup != null)
            canvasGroup.alpha = maxAlpha;
            
        if (imageComponent != null)
            imageComponent.color = baseColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }
}