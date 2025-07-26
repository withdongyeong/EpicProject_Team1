using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : Singleton<FadeManager>
{
    [SerializeField]
    private Image fadeImage; // 전체화면 검정 이미지

    protected override void Awake()
    {
        base.Awake();
        fadeImage = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void LoadSceneWithFade(string sceneName, float fadeDuration = 0.2f)
    {
        StartCoroutine(FadeAndLoad(sceneName, fadeDuration));
    }

    private IEnumerator FadeAndLoad(string sceneName, float duration)
    {
        // 1. Fade Out
        fadeImage.raycastTarget = true;
        yield return fadeImage.DOFade(1f, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();

        // 2. 씬 비동기 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // 3. 로딩 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // 4. Fade In 전에 활성화
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        // 5. 새 씬 로드 완료 후 한 프레임 대기
        yield return null;

        // 6. Fade In
        yield return fadeImage.DOFade(0f, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .WaitForCompletion();
        fadeImage.raycastTarget = false;
        SceneLoader.SetSettingSceneLoadedFalse();
    }
}
