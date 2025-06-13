using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 수정된 아라크네 거미줄 패턴 - BombAvoidanceManager 사용
/// </summary>
public class ArachneSpiderWebPattern : IBossAttackPattern
{
    private GameObject _spiderWebPrefab;
    private int _spiderWebCount;
    private BombAvoidanceManager _bombManager;
    private List<Vector3Int> _singlePointShape;
    
    public string PatternName => "ArachneSpiderWeb";

    /// <summary>
    /// 아라크네 거미줄 패턴 생성자
    /// </summary>
    /// <param name="spiderWebPrefab">거미줄 프리팹</param>
    /// <param name="spiderWebCount">거미줄 개수</param>
    /// <param name="bombManager">폭탄 피하기 매니저</param>
    public ArachneSpiderWebPattern(GameObject spiderWebPrefab, int spiderWebCount, BombAvoidanceManager bombManager)
    {
        _spiderWebPrefab = spiderWebPrefab;
        _spiderWebCount = spiderWebCount;
        _bombManager = bombManager;
        
        // 단일 점 모양 (거미줄은 한 칸만 차지)
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteSpiderWebAttack(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _spiderWebPrefab != null && _bombManager != null;
    }

    /// <summary>
    /// 거미줄 설치 공격
    /// </summary>
    private IEnumerator ExecuteSpiderWebAttack(BaseBoss boss)
    {
        // 방법 1: 순차적으로 거미줄 설치 (기존 방식과 유사)
        for (int i = 0; i < _spiderWebCount; i++)
        {
            // 랜덤 위치 생성 (플레이어 위치 제외)
            Vector3Int randomPosition = GenerateRandomPositionExcludingPlayer();
            
            if (randomPosition != Vector3Int.zero) // 유효한 위치가 생성되었을 때만
            {
                _bombManager.ExecuteFixedBomb(_singlePointShape, randomPosition, _spiderWebPrefab, 
                                              warningDuration: 0.1f, explosionDuration: 5.0f, damage: 0);
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 플레이어 위치를 제외한 랜덤 위치 생성
    /// </summary>
    /// <returns>유효한 랜덤 위치</returns>
    private Vector3Int GenerateRandomPositionExcludingPlayer()
    {
        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(_bombManager.PlayerController.transform.position);
        
        for (int attempts = 0; attempts < 20; attempts++) // 최대 20번 시도
        {
            int x = Random.Range(0, 9); // 0~8 (9x9 그리드)
            int y = Random.Range(0, 9);
            Vector3Int randomPos = new Vector3Int(x, y, 0);
            
            // 그리드 내부이고 플레이어 위치가 아닌 경우
            if (GridManager.Instance.IsWithinGrid(randomPos) && randomPos != playerGridPos)
            {
                return randomPos;
            }
        }
        
        return Vector3Int.zero; // 유효한 위치를 찾지 못한 경우
    }
}