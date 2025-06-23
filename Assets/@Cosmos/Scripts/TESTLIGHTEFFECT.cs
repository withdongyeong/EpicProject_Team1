using UnityEngine;

using UnityEngine.Rendering.Universal;

public class TESTLIGHTEFFECT : MonoBehaviour
{
    public Light2D light2D; // 인스펙터에서 할당
    public float minIntensity = 1f;
    public float maxIntensity = 2f;
    public float outer;
    public float duration = 2f; // 왕복 시간

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        // 주기적으로 왕복 (PingPong)
        float t = Mathf.PingPong(timer, duration) / duration;
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        light2D.pointLightOuterRadius = Mathf.Lerp(2, 6, t);
    }
}
