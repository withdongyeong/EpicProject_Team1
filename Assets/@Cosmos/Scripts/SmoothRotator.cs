using UnityEngine;
using System.Collections;
using System;

public class SmoothRotator : MonoBehaviour
{
    private bool isRotating = false;

    public void RotateZ(Transform target,Action action, float angle = 90f, float duration = 0.1f)
    {
        if(!isRotating && target != null)
        {
            StartCoroutine(RotateZCoroutine(target,action, angle, duration));
        }
    }

    private IEnumerator RotateZCoroutine(Transform target,Action action, float angle, float duration)
    {
        isRotating = true;
        Quaternion startRot = target.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, angle);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.rotation = endRot;
        isRotating = false;
        action?.Invoke(); // 회전 완료 후 액션 실행
        
    }
}
