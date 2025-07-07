using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DefeatHeartUIManager : MonoBehaviour
{
    [Header("하트 설정")]
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Transform heartParent;
    [SerializeField] private float heartSpacing = 60f;
    [SerializeField] private Vector2 heartSize = new Vector2(50f, 50f);

    [Header("연출 설정")]
    [SerializeField] private float fadeOutDelay = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    private List<GameObject> heartObjects = new List<GameObject>();

    void Start()
    {
        InitHearts();
    }

    void InitHearts()
    {
        int heartCount = LifeManager.Instance.Life + 1;

        for (int i = 0; i < heartCount; i++)
        {
            GameObject heartObj = new GameObject($"Heart_{i}");
            heartObj.transform.SetParent(heartParent, false);

            Image heartImage = heartObj.AddComponent<Image>();
            heartImage.sprite = heartSprite;
            heartImage.preserveAspect = true;

            RectTransform rect = heartObj.GetComponent<RectTransform>();
            rect.sizeDelta = heartSize;
            rect.anchoredPosition = new Vector2(i * heartSpacing, 0);
            rect.localScale = Vector3.one;

            heartObjects.Add(heartObj);
        }

        // 가장 마지막 하트 페이드아웃
        if (heartObjects.Count > 0)
        {
            StartCoroutine(FadeLastHeart());
        }
    }

    IEnumerator FadeLastHeart()
    {
        yield return new WaitForSecondsRealtime(fadeOutDelay);

        GameObject last = heartObjects[heartObjects.Count - 1];
        Image img = last.GetComponent<Image>();
        if (img != null)
        {
            img.DOFade(0f, fadeOutDuration).SetUpdate(true);
        }
    }
}