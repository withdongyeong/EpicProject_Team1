using UnityEngine;
using UnityEngine.UI;

public class HighlightTwinkle : MonoBehaviour
{
    [Header("하이라이트 강조 설정")]
    
    [Range(0.3f, 1f)]
    public float minAlpha = 0.7f;         // 최소 투명도
    
    [Range(1f, 1f)]
    public float maxAlpha = 1f;           // 최대 투명도
    
    [Range(0.5f, 3f)]
    public float highlightSpeed = 1.2f;   // 하이라이트 속도
    
    [Range(0f, 2f)]
    public float randomOffset = 0.5f;     // 랜덤 오프셋
    
    [Header("색상 하이라이트")]
    public bool enableColorHighlight = true;
    public Color baseColor = Color.white;
    public Color highlightColor = Color.yellow;
    
    [Header("글로우 효과 (UI만)")]
    public bool enableGlow = false;
    public float glowIntensity = 1.5f;
    
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
        
        // 사인파를 이용한 부드러운 하이라이트 효과
        float highlightValue = (Mathf.Sin((Time.time + timeOffset) * highlightSpeed) + 1f) / 2f;
        
        // 투명도 변화
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, highlightValue);
        
        // 색상 하이라이트
        Color currentColor = enableColorHighlight ? 
            Color.Lerp(baseColor, highlightColor, highlightValue) : 
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
            
            // UI 글로우 효과
            if (enableGlow)
            {
                // Outline 컴포넌트가 있다면 색상 적용
                var outline = GetComponent<Outline>();
                if (outline != null)
                {
                    Color glowColor = highlightColor;
                    glowColor.a = highlightValue * glowIntensity;
                    outline.effectColor = glowColor;
                }
                
                // Shadow 컴포넌트가 있다면 색상 적용
                var shadow = GetComponent<Shadow>();
                if (shadow != null)
                {
                    Color shadowColor = highlightColor;
                    shadowColor.a = highlightValue * 0.5f;
                    shadow.effectColor = shadowColor;
                }
            }
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.color = currentColor;
        }
    }
    
    // 하이라이트 효과 활성화/비활성화
    public void SetHighlightEnabled(bool enabled)
    {
        this.enabled = enabled;
        
        if (!enabled)
        {
            // 원래 상태로 복원
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
                
            if (imageComponent != null)
                imageComponent.color = originalColor;
            else if (spriteRenderer != null)
                spriteRenderer.color = originalColor;
        }
    }
    
    // 하이라이트 속도 변경
    public void SetHighlightSpeed(float speed)
    {
        highlightSpeed = speed;
    }
    
    // 강조 강도 설정
    public void SetHighlightIntensity(float colorBoost)
    {
        // 색상 강도는 highlightColor의 채도로 조절
        if (enableColorHighlight)
        {
            Color.RGBToHSV(highlightColor, out float h, out float s, out float v);
            highlightColor = Color.HSVToRGB(h, s * colorBoost, v);
        }
    }
    
    // 즉시 하이라이트 상태로
    public void ForceHighlight()
    {
        if (!isInitialized) return;
        
        Color highlightedColor = enableColorHighlight ? highlightColor : originalColor;
        
        if (imageComponent != null)
            imageComponent.color = highlightedColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = highlightedColor;
    }
    
    // 즉시 기본 상태로
    public void ForceNormal()
    {
        if (!isInitialized) return;
        
        if (imageComponent != null)
            imageComponent.color = baseColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = baseColor;
    }
}