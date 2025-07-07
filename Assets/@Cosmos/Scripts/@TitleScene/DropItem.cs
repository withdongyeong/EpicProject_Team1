using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 개별 드롭 아이템 컴포넌트
/// </summary>
public class DropItem : MonoBehaviour
{
    private float fallSpeed;
    private float rotationSpeed;
    private float lifeTime;
    
    private float currentLifeTime;
    private SpriteRenderer spriteRenderer;
    private Light2D light2D;
    private float originalLightIntensity;

    public void Initialize(float fall, float rotation, float life, bool enableLight = false, 
                          float lightIntensity = 0.3f, float lightRadius = 2f, Color lightColor = default)
    {
        fallSpeed = fall;
        rotationSpeed = rotation;
        lifeTime = life;
        currentLifeTime = 0f;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Light2D 설정
        if (enableLight)
        {
            light2D = gameObject.AddComponent<Light2D>();
            light2D.lightType = Light2D.LightType.Point;
            light2D.intensity = lightIntensity;
            light2D.pointLightOuterRadius = lightRadius;
            light2D.color = lightColor == default ? Color.white : lightColor;
            light2D.falloffIntensity = 0.5f;
            originalLightIntensity = lightIntensity;
        }
    }

    private void Update()
    {
        // 아래로 떨어지기
        transform.Translate(Vector3.down * fallSpeed * Time.unscaledDeltaTime, Space.World);
        
        // 회전
        transform.Rotate(0, 0, rotationSpeed * Time.unscaledDeltaTime);
        
        // 생존 시간 체크
        currentLifeTime += Time.unscaledDeltaTime;
        
        // 페이드 아웃 효과 (생존 시간의 80% 지나면 서서히 투명해짐)
        if (currentLifeTime > lifeTime * 0.8f)
        {
            float fadeRatio = (currentLifeTime - lifeTime * 0.8f) / (lifeTime * 0.2f);
            
            // 스프라이트 페이드 아웃
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(1f, 0f, fadeRatio);
            spriteRenderer.color = color;
            
            // 라이트 페이드 아웃
            if (light2D != null)
            {
                light2D.intensity = Mathf.Lerp(originalLightIntensity, 0f, fadeRatio);
            }
        }
        
        // 생존 시간이 지나면 삭제
        if (currentLifeTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}