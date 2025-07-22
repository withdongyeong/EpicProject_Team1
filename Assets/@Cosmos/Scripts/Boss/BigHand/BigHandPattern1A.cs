using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴1A - 손으로 테두리 가두기
/// 기존 이동불가 칸을 고려하여 해제하지 않도록 수정
/// </summary>
public class BigHandPattern1A : IBossAttackPattern
{
    private GameObject _leftHandPrefab;
    private GameObject _rightHandPrefab;
    private GameObject _wallPrefab;
    private Vector3 _leftHandOffset = new Vector3(-4, -5, 0);
    private Vector3 _rightHandOffset = new Vector3(3, -2, 0);

    public string PatternName => "9_1";
    
    public BigHandPattern1A(GameObject leftHandPrefab, GameObject rightHandPrefab, GameObject wallPrefab)
    {
        _leftHandPrefab = leftHandPrefab;
        _rightHandPrefab = rightHandPrefab;
        _wallPrefab = wallPrefab;
    }

    /// <summary>
    /// 손 오프셋 설정
    /// </summary>
    /// <param name="leftHandOffset">왼손 오프셋</param>
    /// <param name="rightHandOffset">오른손 오프셋</param>
    public void SetHandOffsets(Vector3 leftHandOffset, Vector3 rightHandOffset)
    {
        _leftHandOffset = leftHandOffset;
        _rightHandOffset = rightHandOffset;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _leftHandPrefab != null && 
               _rightHandPrefab != null && 
               GridManager.Instance != null;
    }

    /// <summary>
    /// 패턴 실행 - 테두리를 차단하고 손을 배치
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        if (bigHand == null)
        {
            Debug.LogError("BigHandPattern1A: boss is not BigHand type!");
            yield break;
        }

        boss.SetAnimationTrigger("Attack");
        
        // 바깥쪽 테두리 위치들 가져오기
        List<Vector3Int> outerBorderPositions = GetOuterBorderPositions();
        
        // 그리드 차단 (기존 이동불가 상태 고려)
        BlockGridPositionsSelectively(outerBorderPositions, boss, bigHand);
        
        // 손 오브젝트들 생성 및 이동
        yield return CreateAndMoveHands(bigHand);
        
        Debug.Log("패턴1A 완료 - 테두리 차단 및 손 배치 완료");
    }

    /// <summary>
    /// 바깥쪽 테두리 위치들 반환
    /// </summary>
    private List<Vector3Int> GetOuterBorderPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (x == 0 || x == 8 || y == 0 || y == 8)
                {
                    positions.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        
        return positions;
    }

    /// <summary>
    /// 그리드 위치를 선택적으로 차단 (기존 이동불가 상태 고려)
    /// </summary>
    /// <param name="positions">차단할 위치들</param>
    /// <param name="boss">보스 인스턴스</param>
    /// <param name="bigHand">BigHand 보스 인스턴스</param>
    private void BlockGridPositionsSelectively(List<Vector3Int> positions, BaseBoss boss, BigHand bigHand)
    {
        List<Vector3Int> singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
        
        // 기존 차단 위치 초기화
        bigHand.BlockedPositions.Clear();
        bigHand.OriginallyUnmovablePositions.Clear();
        
        int newlyBlockedCount = 0;
        int alreadyBlockedCount = 0;
        
        foreach (Vector3Int gridPos in positions)
        {
            // 기존에 이동불가였는지 확인
            bool wasAlreadyUnmovable = GridManager.Instance.UnmovableGridPositions.Contains(gridPos);
            
            // 벽 이펙트는 모든 위치에 생성
            boss.BombHandler.ExecuteFixedBomb(singlePointShape, gridPos, _wallPrefab,
                warningDuration: 1f, explosionDuration: boss.Beat * 40, damage: 0, 
                PatternName, WarningType.Type3);
            
            if (wasAlreadyUnmovable)
            {
                // 기존에 이동불가였던 위치 기록
                bigHand.OriginallyUnmovablePositions.Add(gridPos);
                alreadyBlockedCount++;
            }
            else
            {
                // 새로 차단할 위치만 추가
                GridManager.Instance.AddUnmovableGridPosition(gridPos);
                bigHand.BlockedPositions.Add(gridPos);
                newlyBlockedCount++;
            }
        }
        
    }

    /// <summary>
    /// 손 오브젝트들 생성 및 이동
    /// </summary>
    private IEnumerator CreateAndMoveHands(BigHand bigHand)
    {
        Vector3 leftHandStartPos = new Vector3(-20, -20, 0);
        Vector3 leftHandTargetPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(2, 2, 0)) + _leftHandOffset;
        
        Vector3 rightHandStartPos = new Vector3(15, 15, 0);
        Vector3 rightHandTargetPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(6, 6, 0)) + _rightHandOffset;
        
        bigHand.LeftHandObject = Object.Instantiate(_leftHandPrefab, leftHandStartPos, Quaternion.identity);
        bigHand.RightHandObject = Object.Instantiate(_rightHandPrefab, rightHandStartPos, Quaternion.identity);
        
        float duration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (bigHand.LeftHandObject != null)
                bigHand.LeftHandObject.transform.position = Vector3.Lerp(leftHandStartPos, leftHandTargetPos, progress);
            
            if (bigHand.RightHandObject != null)
                bigHand.RightHandObject.transform.position = Vector3.Lerp(rightHandStartPos, rightHandTargetPos, progress);
            
            yield return null;
        }
        
        if (bigHand.LeftHandObject != null)
            bigHand.LeftHandObject.transform.position = leftHandTargetPos;
        if (bigHand.RightHandObject != null)
            bigHand.RightHandObject.transform.position = rightHandTargetPos;
        
        Debug.Log("손 오브젝트들이 목표 위치에 도착했습니다");
    }

    public void Cleanup()
    {
        Debug.Log("패턴1A 정리 - BigHand에서 직접 관리");
    }
}