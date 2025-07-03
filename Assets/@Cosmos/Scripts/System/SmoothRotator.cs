using UnityEngine;
using System.Collections;
using System;

public class SmoothRotator : MonoBehaviour
{
    private bool isRotating = false;
    private Quaternion endRot;
    private Vector3 startLocalPos;
    private Coroutine coroutine;

    private Transform _target;

    public void RotateZ(Transform target,Action action, float angle = 90f, float duration = 0.1f)
    {
        if(!isRotating && target != null)
        {
            
            //튜토리얼 중이고 , 회전 퀘스트 중이면.. 
            if (GameManager.Instance.IsInTutorial)
            {
                RotateQuest quest = GuideHandler.instance.CurrentQuest as RotateQuest;
                if (quest != null)
                {
                    quest.tilesRotated++;
                }
            }
            //까지입니다 ..

            coroutine = StartCoroutine(RotateZCoroutine(target,action, angle, duration));
        }
    }

    private IEnumerator RotateZCoroutine(Transform target,Action action, float angle, float duration)
    {
        isRotating = true;
        _target = target;
        Quaternion startRot = _target.rotation;
        endRot = startRot * Quaternion.Euler(0, 0, angle);
        startLocalPos = DragManager.Instance.LocalPos;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            _target.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            float currentAngle = Mathf.Lerp(0f, angle, elapsed/duration); 
            Quaternion localRot = Quaternion.Euler(0, 0, currentAngle);
            DragManager.Instance.LocalPos = localRot * startLocalPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        DragManager.Instance.LocalPos = Quaternion.Euler(0, 0, angle) * startLocalPos;
        _target.rotation = endRot;
        isRotating = false;
        coroutine = null;
        action?.Invoke(); // 회전 완료 후 액션 실행
        
    }

    public void TryStopRotate()
    {
        if(isRotating)
        {
            StopCoroutine(coroutine);
            DragManager.Instance.LocalPos = Quaternion.Euler(0, 0, 90) * startLocalPos;
            _target.rotation = endRot;
            isRotating = false;
            coroutine = null;
        }
    }
}
