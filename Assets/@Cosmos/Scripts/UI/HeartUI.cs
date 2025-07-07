using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUIManager : MonoBehaviour
{
    [Header("하트 설정")]
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Transform heartParent;
    [SerializeField] private float heartSpacing = 60f;
    [SerializeField] private Vector2 heartSize = new Vector2(50f, 50f);

    [Header("연출 설정")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float maxScale = 1.3f;
    [SerializeField] private float delayBeforeAnimation = 0.2f;

    private List<GameObject> heartObjects = new();
    private int currentLifeCount = 0;

    private void Awake()
    {
        EventBus.SubscribeBossDeath(UpdateHeartUI);
    }

    private void UpdateHeartUI()
    {
        int targetLifeCount = LifeManager.Instance.Life + 1;

        // 다 지우고 다시 생성
        foreach (var heart in heartObjects)
        {
            Destroy(heart);
        }
        heartObjects.Clear();

        for (int i = 0; i < targetLifeCount; i++)
        {
            CreateHeart(i);
        }


        currentLifeCount = targetLifeCount;
    }

    private void CreateHeart(int index)
    {
        GameObject heartObj = new GameObject($"Heart_{index}");
        heartObj.transform.SetParent(heartParent, false);

        Image heartImage = heartObj.AddComponent<Image>();
        heartImage.sprite = heartSprite;
        heartImage.preserveAspect = true;

        RectTransform rectTransform = heartObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = heartSize;
        rectTransform.anchoredPosition = new Vector2(index * heartSpacing, 0);
        rectTransform.localScale = Vector3.one;

        heartObjects.Add(heartObj);
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeBossDeath(UpdateHeartUI);
    }
}
