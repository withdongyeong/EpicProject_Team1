using System.Collections;
using UnityEngine;

/// <summary>
/// 손가락 복귀 전용 패턴
/// 기존 이동불가 칸을 고려하여 해제하지 않도록 수정
/// </summary>
public class BigHandFingerReturnPattern : IBossAttackPattern
{
    public string PatternName => "9_11";

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
    /// 손가락 오브젝트 복귀 (기존 이동불가 위치는 유지)
    /// </summary>
    private IEnumerator ReturnFingerObject(BigHand bigHand)
    {
        Vector3 currentPos = bigHand.FingerObject.transform.position;
        Vector3 exitPos = GetExitPositionByPrefab(currentPos, bigHand.FingerObject);
        
        // 차단 해제 (기존에 이동불가였던 위치는 해제하지 않음)
        RemoveFingerBlockingSelectively(bigHand);
        
        Debug.Log($"손가락 복귀: {currentPos} → {exitPos}");
        
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
        
        // 관련 정보 정리
        bigHand.LastFingerTipPosition = Vector3Int.zero;
        if (bigHand.FingerBlockedPositions != null)
        {
            bigHand.FingerBlockedPositions.Clear();
        }
        if (bigHand.FingerOriginallyUnmovablePositions != null)
        {
            bigHand.FingerOriginallyUnmovablePositions.Clear();
        }
        
        Debug.Log("손가락 복귀 완료");
    }

    /// <summary>
    /// 손가락 차단을 선택적으로 해제 (기존 이동불가 위치는 유지)
    /// </summary>
    /// <param name="bigHand">BigHand 보스 인스턴스</param>
    private void RemoveFingerBlockingSelectively(BigHand bigHand)
    {
        int removedCount = 0;
        int maintainedCount = 0;
        
        // 패턴으로 새로 차단한 위치만 해제
        if (bigHand.FingerBlockedPositions != null)
        {
            foreach (Vector3Int pos in bigHand.FingerBlockedPositions)
            {
                GridManager.Instance.RemoveUnmovableGridPosition(pos);
                removedCount++;
            }
        }
        
        // 기존에 이동불가였던 위치 개수 세기 (실제로는 해제하지 않음)
        if (bigHand.FingerOriginallyUnmovablePositions != null)
        {
            maintainedCount = bigHand.FingerOriginallyUnmovablePositions.Count;
        }
        
        Debug.Log($"손가락 차단 해제: 해제 {removedCount}개, 유지 {maintainedCount}개 (아이템 효과 보존)");
    }

    /// <summary>
    /// 프리팹 이름에 따라 고정된 복귀 방향 결정
    /// </summary>
    private Vector3 GetExitPositionByPrefab(Vector3 currentPos, GameObject fingerObject)
    {
        string prefabName = fingerObject.name.Replace("(Clone)", "").ToLower();
        
        if (prefabName.Contains("bottom"))
        {
            // Bottom 프리팹: 아래쪽으로 복귀
            return currentPos + Vector3.down * 15f;
        }
        else if (prefabName.Contains("top"))
        {
            // Top 프리팹: 위쪽으로 복귀
            return currentPos + Vector3.up * 15f;
        }
        else if (prefabName.Contains("left"))
        {
            // Left 프리팹: 왼쪽으로 복귀
            return currentPos + Vector3.left * 15f;
        }
        else if (prefabName.Contains("right"))
        {
            // Right 프리팹: 오른쪽으로 복귀
            return currentPos + Vector3.right * 15f;
        }
        else
        {
            // 기본값: 위쪽으로
            Debug.LogWarning($"알 수 없는 손가락 프리팹: {prefabName}, 기본 방향(위쪽) 사용");
            return currentPos + Vector3.up * 15f;
        }
    }

    public void Cleanup()
    {
        Debug.Log("손가락 복귀 패턴 정리 완료");
    }
}