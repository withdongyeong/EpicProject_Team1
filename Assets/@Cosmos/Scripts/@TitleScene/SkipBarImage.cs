using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkipBarImage : MonoBehaviour
{
    private Image skipBarImage;
    [SerializeField]
    private bool isTweening = false;

    private void Awake()
    {
        skipBarImage = GetComponent<Image>();
    }

    private void Start()
    {
        SetFillAmount(0f);
    }

    private void Update()
    {
        if (skipBarImage.fillAmount == 1f && !isTweening)
        {
            
            ActivateSkipBarImage();
        }
    }

    public void SetFillAmount(float fillAmount)
    {
        if (skipBarImage == null)
        {
            skipBarImage = GetComponent<Image>();
        }
        skipBarImage.fillAmount = fillAmount;
    }
    
    
    public void ActivateSkipBarImage()
    {
        if (isTweening)
        {
            return; // 이미 애니메이션이 진행 중이면 아무 작업도 하지 않음
        }
        isTweening = true;
        transform.DOScale(1.5f, 0.1f)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo)
            .SetAutoKill(true)
            .SetUpdate(UpdateType.Normal, false)
            .onComplete = () =>
        {
            GameManager.Instance.LoadTitle();
        };
    }
}
