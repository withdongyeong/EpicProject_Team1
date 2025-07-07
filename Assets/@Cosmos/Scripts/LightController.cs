using System;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private Light2D starLight;
    [SerializeField] private float maxIntensity = 10f;
    [SerializeField] private float minIntensity = 5f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float innerRadius = 0.3f;
    [SerializeField] private float outerRadius = 0.5f; // 빛의 반경

    [SerializeField] private float randomTimer;

    private void Awake()
    {
        starLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        if (starLight == null)
        {
            Debug.LogError("Light component not found on this GameObject.");
            return;
        }

        starLight.intensity = minIntensity;
        starLight.pointLightInnerRadius = innerRadius;
        starLight.pointLightOuterRadius = outerRadius;
        randomTimer = (int)Random.Range(0f, 4f);
        StartCoroutine(FadeLight());
    }

    private IEnumerator FadeLight()
    {

        //랜덤 타임 오프셋
        yield return new WaitForSeconds(randomTimer);
        while (true)
        {

            starLight.pointLightInnerRadius = innerRadius;
            starLight.pointLightOuterRadius = outerRadius;
            //1. 커집니다
            starLight.intensity = minIntensity;
            float elapsedTime = 0f;
            float targetIntensity = maxIntensity;
            float startIntensity = starLight.intensity;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                starLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
                yield return null;
            }


            //2. 작아집니다
            starLight.intensity = targetIntensity;
            elapsedTime = 0f;
            targetIntensity = minIntensity;
            startIntensity = starLight.intensity;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                starLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
                yield return null;
            }

        }
    }

}
