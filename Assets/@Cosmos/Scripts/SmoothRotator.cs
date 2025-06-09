using UnityEngine;
using System.Collections;

public class SmoothRotator : MonoBehaviour
{
    private bool isRotating = false;

    public void RotateZ(Transform target, float angle = 90f, float duration = 0.1f)
    {
        if(!isRotating && target != null)
        {
            StartCoroutine(RotateZCoroutine(target, angle, duration));
        }
    }

    private IEnumerator RotateZCoroutine(Transform target, float angle, float duration)
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
    }
}
