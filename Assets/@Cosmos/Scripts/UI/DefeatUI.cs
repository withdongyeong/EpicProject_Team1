using System.Collections;
using UnityEngine;

/// <summary>
/// 패배 UI - 들어올렸다가 떨어지면서 기울어지고, 톡톡 튀고, 기울어진 채 멈춤
/// 반복 가능함
/// </summary>
public class DefeatUI : MonoBehaviour
{
    private RectTransform _rect;
    private Vector3 _originalPos;
    private Quaternion _originalRot;

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
        _originalPos = _rect.localPosition;
        _originalRot = _rect.localRotation;

        StartCoroutine(LoopDefeatEffect());
    }

    /// <summary>
    /// 반복 코루틴
    /// </summary>
    private IEnumerator LoopDefeatEffect()
    {
        while (true)
        {
            yield return PlayDefeatEffect();
            yield return new WaitForSecondsRealtime(1f); // 반복 간 대기 시간
        }
    }

    /// <summary>
    /// 단일 애니메이션 연출
    /// </summary>
    private IEnumerator PlayDefeatEffect()
    {
        // 1. 위로 들어올림
        float liftDuration = 0.3f;
        float elapsed = 0f;
        Vector3 liftedPos = _originalPos + new Vector3(0, 40f, 0);

        while (elapsed < liftDuration)
        {
            float t = elapsed / liftDuration;
            _rect.localPosition = Vector3.Lerp(_originalPos, liftedPos, t);
            _rect.localRotation = Quaternion.Lerp(_originalRot, Quaternion.identity, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _rect.localPosition = liftedPos;

        // 2. 잠시 정지
        yield return new WaitForSecondsRealtime(0.2f);

        // 3. 툭 떨어짐
        float dropDuration = 0.25f;
        elapsed = 0f;
        Vector3 dropPos = _originalPos + new Vector3(0, -20f, 0);

        while (elapsed < dropDuration)
        {
            float t = elapsed / dropDuration;
            _rect.localPosition = Vector3.Lerp(liftedPos, dropPos, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        _rect.localPosition = dropPos;

        // 4. 톡톡 튀는 회전
        float bounceDuration = 0.5f;
        elapsed = 0f;
        float maxAngle = 15f;

        while (elapsed < bounceDuration)
        {
            float t = elapsed / bounceDuration;
            float angle = Mathf.Sin(t * Mathf.PI * 4f) * maxAngle * (1f - t); // 점점 줄어듦
            _rect.localRotation = Quaternion.Euler(0f, 0f, angle);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
