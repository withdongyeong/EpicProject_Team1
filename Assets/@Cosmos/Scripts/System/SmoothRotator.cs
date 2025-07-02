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

            StartCoroutine(RotateZCoroutine(target,action, angle, duration));
        }
    }

    private IEnumerator RotateZCoroutine(Transform target,Action action, float angle, float duration)
    {
        isRotating = true;
        Quaternion startRot = target.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, angle);
        float elapsed = 0f;
        Vector3 startLocalPos = DragManager.Instance.LocalPos;

        while (elapsed < duration)
        {
            target.rotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            float currentAngle = Mathf.Lerp(0f, angle, elapsed/duration); 
            Quaternion localRot = Quaternion.Euler(0, 0, currentAngle);
            DragManager.Instance.LocalPos = localRot * startLocalPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        DragManager.Instance.LocalPos = Quaternion.Euler(0, 0, angle) * startLocalPos;
        target.rotation = endRot;
        isRotating = false;
        action?.Invoke(); // 회전 완료 후 액션 실행
        
    }
}
