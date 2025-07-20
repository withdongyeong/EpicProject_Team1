using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 피격 시 화면 가장자리에 붉은색 그라데이션 효과를 보여주는 컴포넌트
/// </summary>
public class DamageScreenEffect : MonoBehaviour
{
    [Header("화면 효과 설정")]
    [SerializeField] private Image screenEffectImage;
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float holdDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;
    
    private Coroutine effectCoroutine;
    
    private void Awake()
    {
        // 화면 효과 이미지가 설정되지 않았다면 자동으로 찾기
        if (screenEffectImage == null)
        {
            screenEffectImage = GetComponent<Image>();
        }
        
        // 처음에는 보이지 않게 설정
        if (screenEffectImage != null)
        {
            SetEffectAlpha(0f);
        }
    }
    
    /// <summary>
    /// 피격 화면 효과 시작
    /// </summary>
    public void ShowDamageEffect()
    {
        // 이미 실행 중인 코루틴이 있다면 중지
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        
        effectCoroutine = StartCoroutine(DamageScreenEffectCoroutine());
    }
    
    /// <summary>
    /// 화면 효과 코루틴
    /// </summary>
    private IEnumerator DamageScreenEffectCoroutine()
    {
        // 페이드 인 - 빠르게 나타남
        yield return StartCoroutine(FadeEffect(0f, maxAlpha, fadeInDuration));
        
        // 잠시 유지
        yield return new WaitForSeconds(holdDuration);
        
        // 페이드 아웃 - 천천히 사라짐
        yield return StartCoroutine(FadeEffect(maxAlpha, 0f, fadeOutDuration));
        
        effectCoroutine = null;
    }
    
    /// <summary>
    /// 화면 효과 페이드
    /// </summary>
    private IEnumerator FadeEffect(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetEffectAlpha(currentAlpha);
            yield return null;
        }
        
        // 최종값 확실히 설정
        SetEffectAlpha(endAlpha);
    }
    
    /// <summary>
    /// 화면 효과 알파값 설정
    /// </summary>
    private void SetEffectAlpha(float alpha)
    {
        if (screenEffectImage != null)
        {
            Color color = screenEffectImage.color;
            color.a = alpha;
            screenEffectImage.color = color;
        }
    }
    
    /// <summary>
    /// 화면 효과 즉시 중지
    /// </summary>
    public void StopEffect()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
        
        SetEffectAlpha(0f);
    }
}