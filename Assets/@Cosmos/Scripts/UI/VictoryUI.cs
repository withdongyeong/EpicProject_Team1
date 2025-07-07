using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VictoryUI : MonoBehaviour
{
    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private Light2D _light;

    private void Start()
    {
        _originalScale = transform.localScale;
        _originalRotation = transform.rotation;
        StartCoroutine(LoopVictoryEffect());
    }

    /// <summary>
    /// 반복 실행 코루틴
    /// </summary>
    private IEnumerator LoopVictoryEffect()
    {
        while (true)
        {
            yield return PlayVictoryEffect();
            yield return new WaitForSecondsRealtime(0.5f); // 반복 간 간격
        }
    }

    /// <summary>
    /// 1회 애니메이션 효과
    /// </summary>
    private IEnumerator PlayVictoryEffect()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // Light2D 추가
        _light = gameObject.AddComponent<Light2D>();
        _light.intensity = 1.3f;
        _light.pointLightOuterRadius = 4f;
        _light.pointLightInnerRadius = 2f;
        _light.lightType = Light2D.LightType.Sprite;
        _light.color = Color.white;

        // 확대 & 좌우 회전 (좌로 → 우로)
        float growDuration = 0.4f;
        float elapsed = 0f;

        Vector3 targetScale = _originalScale * 2f;
        Quaternion leftRotation = Quaternion.Euler(0f, 0f, -15f);
        Quaternion rightRotation = Quaternion.Euler(0f, 0f, 15f);

        // 좌로 기울이며 커짐 (절반)
        while (elapsed < growDuration / 2f)
        {
            float t = elapsed / (growDuration / 2f);
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, t);
            transform.rotation = Quaternion.Lerp(_originalRotation, leftRotation, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // 우로 기울이며 유지 (나머지 절반)
        elapsed = 0f;
        while (elapsed < growDuration / 2f)
        {
            float t = elapsed / (growDuration / 2f);
            transform.localScale = targetScale;
            transform.rotation = Quaternion.Lerp(leftRotation, rightRotation, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // 0.2초 유지
        yield return new WaitForSecondsRealtime(0.2f);

        // 스케일 & 회전 복귀
        float shrinkDuration = 0.3f;
        elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            float t = elapsed / shrinkDuration;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, t);
            transform.rotation = Quaternion.Lerp(rightRotation, _originalRotation, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = _originalScale;
        transform.rotation = _originalRotation;

        Destroy(_light);
    }
}
