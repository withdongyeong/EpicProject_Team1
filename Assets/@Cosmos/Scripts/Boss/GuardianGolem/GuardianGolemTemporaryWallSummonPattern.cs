using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemTemporaryWallSummonPattern : IBossAttackPattern
{
    private GameObject _temporaryWall;
    private int _temporaryWallCount;
    private List<Vector3Int> _singlePointShape;
    private List<Vector3Int> _wallPositions;

    public string PatternName => "GuardianGolemTemporaryWallSummonPattern";

    /// <summary>
    /// 임시벽 생성자
    /// </summary>
    public GuardianGolemTemporaryWallSummonPattern(GameObject TemporaryWall, int TemporaryWallCount)
    {
        _temporaryWall = TemporaryWall;
        _temporaryWallCount = TemporaryWallCount;

        // 단일 점 모양 (거미줄은 한 칸만 차지)
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };

        _wallPositions = new List<Vector3Int>();
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteSpiderWebAttack(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary> 
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _temporaryWall != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 임시벽 설치 공격
    /// </summary>
    private IEnumerator ExecuteSpiderWebAttack(BaseBoss boss)
    {
        _wallPositions.Clear();
        int deleteCount = boss.gameObject.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;

        // 방법 1: 순차적으로 임시벽 설치 (기존 방식과 유사)
        for (int i = 0; i < _temporaryWallCount; i++)
        {
            // 랜덤 위치 생성 (플레이어 위치 제외)
            int NotEditX = Random.Range(deleteCount - 4 , 4 - deleteCount); 
            int Y = Random.Range(0, 9);

            Vector3Int Position = new Vector3Int(NotEditX + 4, Y , 0);

            _wallPositions.Add(Position);
        }

        foreach (var randomPosition in _wallPositions)
        {
            if (randomPosition != Vector3Int.zero) // 유효한 위치가 생성되었을 때만
            {
                boss.BombHandler.ExecuteFixedBomb(_singlePointShape, randomPosition, _temporaryWall,
                                              warningDuration: 1f, explosionDuration: 3.0f, damage: 0, warningType: WarningType.Type3);

            }
        }

        yield return 0;
    }

    /// <summary>
    /// 플레이어 위치를 제외한 랜덤 위치 생성
    /// </summary>
    /// <returns>유효한 랜덤 위치</returns>
    private Vector3Int GenerateRandomPositionExcludingPlayer(BaseBoss boss)
    {
        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(boss.BombHandler.PlayerController.transform.position);

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
