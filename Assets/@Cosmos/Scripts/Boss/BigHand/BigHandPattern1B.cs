using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴1B - 손 거두기 및 차단 해제
/// </summary>
public class BigHandPattern1B : IBossAttackPattern
{
    public string PatternName => "패턴1B_손거두기";

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && GridManager.Instance != null && boss is BigHand;
    }

    /// <summary>
    /// 패턴 실행 - 손들을 거두고 차단 해제
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        if (bigHand == null)
        {
            Debug.LogError("BigHandPattern1B: boss is not BigHand type!");
            yield break;
        }

        Debug.Log("패턴1B 시작 - 손 거두기 및 차단 해제");
        
        // 손들을 화면 밖으로 돌려보내고 차단 해제
        yield return ReturnHandsAndUnblock(bigHand);
        
        Debug.Log("패턴1B 완료 - 손 거두기 및 차단 해제 완료");
    }

    private IEnumerator ReturnHandsAndUnblock(BigHand bigHand)
    {
        Vector3 leftHandExitPos = new Vector3(-20, -20, 0);
        Vector3 rightHandExitPos = new Vector3(15, 15, 0);
        
        Vector3 leftHandCurrentPos = bigHand.LeftHandObject != null ? bigHand.LeftHandObject.transform.position : leftHandExitPos;
        Vector3 rightHandCurrentPos = bigHand.RightHandObject != null ? bigHand.RightHandObject.transform.position : rightHandExitPos;
        
        float duration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (bigHand.LeftHandObject != null)
                bigHand.LeftHandObject.transform.position = Vector3.Lerp(leftHandCurrentPos, leftHandExitPos, progress);
            
            if (bigHand.RightHandObject != null)
                bigHand.RightHandObject.transform.position = Vector3.Lerp(rightHandCurrentPos, rightHandExitPos, progress);
            
            yield return null;
        }
        
        // 손 오브젝트들 제거
        if (bigHand.LeftHandObject != null)
        {
            Object.Destroy(bigHand.LeftHandObject);
            bigHand.LeftHandObject = null;
        }
        if (bigHand.RightHandObject != null)
        {
            Object.Destroy(bigHand.RightHandObject);
            bigHand.RightHandObject = null;
        }
        
        // 차단된 위치들 해제
        foreach (Vector3Int gridPos in bigHand.BlockedPositions)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(gridPos);
        }
        bigHand.BlockedPositions.Clear();
        
        Debug.Log("테두리 차단 해제 완료! 손들이 화면 밖으로 나간 후 삭제됨");
    }

    public void Cleanup()
    {
        Debug.Log("패턴1B 정리 - BigHand에서 직접 관리");
    }
}