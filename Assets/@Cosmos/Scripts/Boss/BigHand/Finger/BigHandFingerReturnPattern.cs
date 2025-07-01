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
        Vector3 exitPos = GetExitPositionByPrefab(currentPos, bigHand.FingerObject);
        
        // 차단 해제
        if (bigHand.FingerBlockedPositions != null)
        {
            foreach (Vector3Int pos in bigHand.FingerBlockedPositions)
            {
                GridManager.Instance.RemoveUnmovableGridPosition(pos);
            }
            bigHand.FingerBlockedPositions.Clear();
        }
        
        Debug.Log($"손가락 복귀: {currentPos} → {exitPos}");
        
        // 복귀 애니메이션
        float duration = 0.8f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 0.8f, elapsedTime / duration);
            
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
        
        Debug.Log("손가락 복귀 완료");
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