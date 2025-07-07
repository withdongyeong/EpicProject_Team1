using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour
{
    private Light2D starLight;

    [SerializeField] private float maxIntensity = 10f;
    [SerializeField] private float minIntensity = 5f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float innerRadius = 0.3f;
    [SerializeField] private float outerRadius = 0.5f;

    private float velocity = 0f;
    private float randomOffset; // 시작 시 랜덤 오프셋
    private float timer;

    private void Awake()
    {
        starLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        if (starLight == null)
        {
            Debug.LogError("Light component not found on this GameObject.");
            enabled = false;
            return;
        }

        starLight.intensity = minIntensity;
        starLight.pointLightInnerRadius = innerRadius;
        starLight.pointLightOuterRadius = outerRadius;

        randomOffset = UnityEngine.Random.Range(0f, duration);
        timer = Time.time + randomOffset;
    }

    private void Update()
    {
        float t = (Time.time - timer) % (2 * duration);

        float targetIntensity;
        if (t < duration)
        {
            // 커지는 구간
            targetIntensity = maxIntensity;
        }
        else
        {
            // 작아지는 구간
            targetIntensity = minIntensity;
        }

        starLight.pointLightInnerRadius = innerRadius;
        starLight.pointLightOuterRadius = outerRadius;

        // 부드럽게 전환
        starLight.intensity = Mathf.SmoothDamp(starLight.intensity, targetIntensity, ref velocity, duration * 0.5f);
    }
}