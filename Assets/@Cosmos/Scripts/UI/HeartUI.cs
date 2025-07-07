using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HeartUIManager : MonoBehaviour
{
    [Header("하트 설정")]
    [SerializeField] private Sprite heartSprite; // 할당할 하트 스프라이트
    [SerializeField] private Transform heartParent; // 하트들이 생성될 부모 오브젝트
    [SerializeField] private float heartSpacing = 60f; // 하트 간격
    [SerializeField] private Vector2 heartSize = new Vector2(50f, 50f); // 하트 크기
    
    [Header("연출 설정")]
    [SerializeField] private float fadeInDuration = 0.5f; // 페이드인 시간
    [SerializeField] private float scaleUpDuration = 0.3f; // 스케일업 시간
    [SerializeField] private float maxScale = 1.3f; // 최대 스케일
    [SerializeField] private float delayBeforeAnimation = 0.2f; // 애니메이션 시작 전 지연시간
    
    private List<GameObject> heartObjects = new List<GameObject>();
    private int currentLifeCount = 0;
    
    void Start()
    {
        // 시작할 때 현재 라이프에 맞게 하트 생성
        UpdateHeartUI();
    }
    
    void Update()
    {
        // 라이프가 변경되었는지 확인
        if (LifeManager.Instance.Life != currentLifeCount)
        {
            UpdateHeartUI();
        }
    }
    
    void UpdateHeartUI()
    {
        int targetLifeCount = LifeManager.Instance.Life + 1;
        
        // 현재 하트 개수보다 목표 개수가 많으면 하트 추가
        if (targetLifeCount > currentLifeCount)
        {
            for (int i = currentLifeCount; i < targetLifeCount; i++)
            {
                CreateHeart(i, i == targetLifeCount - 1); // 마지막 하트만 연출 적용
            }
        }
        // 현재 하트 개수보다 목표 개수가 적으면 하트 제거
        else if (targetLifeCount < currentLifeCount)
        {
            for (int i = currentLifeCount - 1; i >= targetLifeCount; i--)
            {
                if (i < heartObjects.Count)
                {
                    Destroy(heartObjects[i]);
                    heartObjects.RemoveAt(i);
                }
            }
        }
        
        currentLifeCount = targetLifeCount;
    }
    
    void CreateHeart(int index, bool playAnimation)
    {
        // 하트 오브젝트 생성
        GameObject heartObj = new GameObject($"Heart_{index}");
        heartObj.transform.SetParent(heartParent, false); // worldPositionStays를 false로 설정
        
        // Image 컴포넌트 추가 및 설정
        Image heartImage = heartObj.AddComponent<Image>();
        heartImage.sprite = heartSprite;
        heartImage.preserveAspect = true;
        
        // RectTransform 설정
        RectTransform rectTransform = heartObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = heartSize;
        rectTransform.anchoredPosition = new Vector2(index * heartSpacing, 0);
        rectTransform.localScale = Vector3.one;
        
        // 리스트에 추가
        heartObjects.Add(heartObj);
        
        // 연출이 필요한 경우 (새로 생성된 하트)
        if (playAnimation)
        {
            StartCoroutine(PlayHeartAnimation(heartImage, rectTransform));
        }
        else
        {
            // 애니메이션이 없는 하트는 바로 불투명하게 설정
            Color color = heartImage.color;
            color.a = 1f;
            heartImage.color = color;
        }
    }
    
    IEnumerator PlayHeartAnimation(Image heartImage, RectTransform rectTransform)
    {
        // 시작할 때 투명하게 설정
        Color startColor = heartImage.color;
        startColor.a = 0f;
        heartImage.color = startColor;
        
        // 지연 시간 대기 (TimeScale 영향 받지 않음)
        yield return new WaitForSecondsRealtime(delayBeforeAnimation);
        
        // 페이드인 애니메이션 (TimeScale 영향 받지 않음)
        heartImage.DOFade(1f, fadeInDuration).SetUpdate(true);
        
        // 페이드인 완료 후 스케일 애니메이션
        yield return new WaitForSecondsRealtime(fadeInDuration);
        
        // 스케일업 후 원래 크기로 돌아가는 애니메이션 (TimeScale 영향 받지 않음)
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(rectTransform.DOScale(maxScale, scaleUpDuration * 0.6f))
                    .Append(rectTransform.DOScale(1f, scaleUpDuration * 0.4f))
                    .SetUpdate(true); // TimeScale 영향 받지 않도록 설정
    }
    
    // 수동으로 하트 UI 업데이트를 호출하고 싶을 때 사용
    public void ForceUpdateHeartUI()
    {
        UpdateHeartUI();
    }
    
    // 특정 인덱스의 하트에 연출 적용 (예: 하트 획득 시)
    public void PlayHeartGainAnimation(int heartIndex)
    {
        if (heartIndex >= 0 && heartIndex < heartObjects.Count)
        {
            Image heartImage = heartObjects[heartIndex].GetComponent<Image>();
            RectTransform rectTransform = heartObjects[heartIndex].GetComponent<RectTransform>();
            StartCoroutine(PlayHeartAnimation(heartImage, rectTransform));
        }
    }
}