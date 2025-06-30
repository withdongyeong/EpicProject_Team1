using System.Collections;
using UnityEngine;

/// <summary>
/// 손가락 복귀 전용 패턴
/// </summary>
public class BigHandFingerReturnPattern : IBossAttackPattern
{
    public string PatternName => "손가락_복귀";

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null;
    }

    /// <summary>
    /// 손가락 복귀 실행
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        if (bigHand?.FingerObject != null)
        {
            yield return ReturnFingerObject(bigHand);
        }
    }

    /// <summary>
    /// 손가락 오브젝트 복귀
    /// </summary>
    private IEnumerator ReturnFingerObject(BigHand bigHand)
    {
        Vector3 currentPos = bigHand.FingerObject.transform.position;
        Vector3 exitPos = CalculateExitPosition(currentPos, bigHand);
        
        // 차단 해제
        foreach (Vector3Int pos in bigHand.FingerBlockedPositions)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(pos);
        }
        bigHand.FingerBlockedPositions.Clear();
        
        // 복귀 애니메이션
        float duration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (bigHand.FingerObject != null)
                bigHand.FingerObject.transform.position = Vector3.Lerp(currentPos, exitPos, progress);
            
            yield return null;
        }
        
        // 손가락 오브젝트 제거
        if (bigHand.FingerObject != null)
        {
            Object.Destroy(bigHand.FingerObject);
            bigHand.FingerObject = null;
        }
    }

    /// <summary>
    /// 손가락이 진입한 방향의 반대로 복귀
    /// </summary>
    private Vector3 CalculateExitPosition(Vector3 currentPos, BigHand bigHand)
    {
        Vector3Int fingerTipPos = bigHand.LastFingerTipPosition;
        Vector3Int fingerStartPos = FindFingerStartPosition(bigHand.FingerBlockedPositions, fingerTipPos);
        Vector3Int entryDirection = fingerTipPos - fingerStartPos;
        
        Vector3 exitDirection = Vector3.zero;
        
        if (Mathf.Abs(entryDirection.x) > Mathf.Abs(entryDirection.y))
        {
            // X축이 주 방향
            if (entryDirection.x > 0)
            {
                exitDirection = Vector3.left;  // 왼쪽에서 온 경우 왼쪽으로
            }
            else
            {
                exitDirection = Vector3.right; // 오른쪽에서 온 경우 오른쪽으로
            }
        }
        else
        {
            // Y축이 주 방향
            if (entryDirection.y > 0)
            {
                exitDirection = Vector3.down;  // 아래에서 온 경우 아래로
            }
            else
            {
                exitDirection = Vector3.up;    // 위에서 온 경우 위로
            }
        }
        
        return currentPos + exitDirection * 15f;
    }

    /// <summary>
    /// 손가락의 시작점 찾기
    /// </summary>
    private Vector3Int FindFingerStartPosition(System.Collections.Generic.List<Vector3Int> blockedPositions, Vector3Int tipPos)
    {
        if (blockedPositions == null || blockedPositions.Count == 0)
            return tipPos;
        
        Vector3Int startPos = blockedPositions[0];
        float maxDistance = 0f;
        
        foreach (Vector3Int pos in blockedPositions)
        {
            float distance = Mathf.Sqrt((pos.x - tipPos.x) * (pos.x - tipPos.x) + (pos.y - tipPos.y) * (pos.y - tipPos.y));
            if (distance > maxDistance)
            {
                maxDistance = distance;
                startPos = pos;
            }
        }
        
        return startPos;
    }

    public void Cleanup()
    {
        // 복귀 패턴은 정리할 것 없음
    }
}