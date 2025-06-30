using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 손가락 십자에서 시작하는 구불거리는 선형 확산 패턴
/// </summary>
public class BigHandRadialWavePattern : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;
    private float _growthSpeed = 0.15f; // 성장 속도 (조절 가능)

    public string PatternName => "방사형_확산";
    
    /// <summary>
    /// 성장 속도 프로퍼티 (외부에서 조절 가능)
    /// </summary>
    public float GrowthSpeed { get => _growthSpeed; set => _growthSpeed = value; }
    
    public BigHandRadialWavePattern(GameObject attackEffectPrefab)
    {
        _attackEffectPrefab = attackEffectPrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    /// <summary>
    /// 방사형 확산 패턴 실행
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        if (bigHand == null)
        {
            Debug.LogError("BigHandRadialWavePattern: boss is not BigHand type!");
            yield break;
        }

        Vector3Int startCenter = bigHand.LastFingerTipPosition;
        List<Vector3Int> blockedPositions = bigHand.FingerBlockedPositions;
        
        Debug.Log("구불거리는 선형 확산 패턴 시작");
        
        yield return boss.StartCoroutine(ExecuteWavyLines(boss, startCenter, blockedPositions));
        
        Debug.Log("구불거리는 선형 확산 패턴 완료");
    }

    /// <summary>
    /// 8방향으로 구불거리는 선들이 뻗어나가는 패턴
    /// </summary>
    private IEnumerator ExecuteWavyLines(BaseBoss boss, Vector3Int startCenter, List<Vector3Int> blockedPositions)
    {
        // 8방향 정의
        Vector3Int[] directions = {
            new Vector3Int(0, 1, 0),   // 위
            new Vector3Int(1, 1, 0),   // 우상
            new Vector3Int(1, 0, 0),   // 우
            new Vector3Int(1, -1, 0),  // 우하
            new Vector3Int(0, -1, 0),  // 아래
            new Vector3Int(-1, -1, 0), // 좌하
            new Vector3Int(-1, 0, 0),  // 좌
            new Vector3Int(-1, 1, 0)   // 좌상
        };

        // 각 방향별로 선 생성
        List<WavyLine> wavyLines = new List<WavyLine>();
        for (int i = 0; i < directions.Length; i++)
        {
            WavyLine line = new WavyLine(startCenter, directions[i], blockedPositions);
            wavyLines.Add(line);
        }

        // 모든 선이 동시에 성장
        List<Coroutine> growthCoroutines = new List<Coroutine>();
        foreach (WavyLine line in wavyLines)
        {
            growthCoroutines.Add(boss.StartCoroutine(GrowWavyLine(boss, line)));
        }

        // 모든 성장 완료까지 대기
        foreach (Coroutine coroutine in growthCoroutines)
        {
            yield return coroutine;
        }
    }

    /// <summary>
    /// 개별 구불거리는 선 성장
    /// </summary>
    private IEnumerator GrowWavyLine(BaseBoss boss, WavyLine line)
    {
        while (!line.IsComplete)
        {
            Vector3Int nextPos = line.GetNextPosition();
            if (nextPos != Vector3Int.zero)
            {
                // 블록된 위치가 아닌 경우에만 공격
                if (!line.BlockedPositions.Contains(nextPos))
                {
                    boss.BombHandler.ExecuteFixedBomb(
                        new List<Vector3Int> { new Vector3Int(0, 0, 0) },
                        nextPos,
                        _attackEffectPrefab,
                        warningDuration: 0.3f,
                        explosionDuration: 0.4f,
                        damage: 20
                    );
                }
                line.Advance();
            }
            else
            {
                line.IsComplete = true;
            }
            
            yield return new WaitForSeconds(_growthSpeed);
        }
    }

    public void Cleanup()
    {
        Debug.Log("구불거리는 선형 확산 패턴 정리 완료");
    }
}

/// <summary>
/// 구불거리는 선을 표현하는 클래스
/// </summary>
public class WavyLine
{
    private Vector3Int _startPos;
    private Vector3Int _mainDirection;
    private Vector3Int _currentPos;
    private List<Vector3Int> _path;
    private int _currentStep;
    private System.Random _random;
    
    public List<Vector3Int> BlockedPositions { get; private set; }
    public bool IsComplete { get; set; }

    public WavyLine(Vector3Int startPos, Vector3Int mainDirection, List<Vector3Int> blockedPositions)
    {
        _startPos = startPos;
        _mainDirection = mainDirection;
        _currentPos = startPos;
        _path = new List<Vector3Int>();
        _currentStep = 0;
        _random = new System.Random();
        BlockedPositions = blockedPositions;
        IsComplete = false;
        
        GeneratePath();
    }

    /// <summary>
    /// 구불거리는 경로 생성 - 그리드 끝까지 확실히 도달
    /// </summary>
    private void GeneratePath()
    {
        Vector3Int currentPos = _startPos;
        
        // 그리드 경계에 도달할 때까지 계속 생성
        while (!IsAtGridBoundary(currentPos, _mainDirection))
        {
            Vector3Int nextPos = CalculateNextPosition(currentPos, _path.Count);
            
            // 그리드 밖으로 나가면 경계 내로 조정
            nextPos = ClampToGrid(nextPos);
            
            _path.Add(nextPos);
            currentPos = nextPos;
            
            // 안전장치: 너무 많은 스텝 방지
            if (_path.Count > 50) break;
            
            // 경계에 도달했으면 종료
            if (IsAtGridBoundary(currentPos, _mainDirection)) break;
        }
    }
    
    /// <summary>
    /// 현재 위치가 메인 방향의 그리드 경계에 도달했는지 확인
    /// </summary>
    private bool IsAtGridBoundary(Vector3Int pos, Vector3Int direction)
    {
        // 각 방향별로 경계 확인
        if (direction.x > 0 && pos.x >= 8) return true;  // 오른쪽 끝
        if (direction.x < 0 && pos.x <= 0) return true;  // 왼쪽 끝
        if (direction.y > 0 && pos.y >= 8) return true;  // 위쪽 끝
        if (direction.y < 0 && pos.y <= 0) return true;  // 아래쪽 끝
        
        // 대각선 방향인 경우 둘 중 하나라도 경계에 도달하면 끝
        if (direction.x != 0 && direction.y != 0)
        {
            bool xBoundary = (direction.x > 0 && pos.x >= 8) || (direction.x < 0 && pos.x <= 0);
            bool yBoundary = (direction.y > 0 && pos.y >= 8) || (direction.y < 0 && pos.y <= 0);
            return xBoundary || yBoundary;
        }
        
        return false;
    }

    /// <summary>
    /// 다음 위치 계산 (자연스러운 구불거림)
    /// </summary>
    private Vector3Int CalculateNextPosition(Vector3Int currentPos, int step)
    {
        // 70% 확률로 구불거림
        if (_random.NextDouble() < 0.7f)
        {
            // 메인 방향 + 수직 방향 조합으로 대각선 이동
            Vector3Int[] perpendiculars = GetPerpendicularDirections(_mainDirection);
            if (perpendiculars.Length > 0)
            {
                Vector3Int wobble = perpendiculars[_random.Next(perpendiculars.Length)];
                
                // 메인 방향과 수직 방향을 조합 (대각선 이동)
                Vector3Int combinedDirection = _mainDirection + wobble;
                
                // 결과가 8방향 중 하나가 되도록 정규화
                combinedDirection.x = Mathf.Clamp(combinedDirection.x, -1, 1);
                combinedDirection.y = Mathf.Clamp(combinedDirection.y, -1, 1);
                
                return currentPos + combinedDirection;
            }
        }
        
        // 30% 확률로 직진
        return currentPos + _mainDirection;
    }

    /// <summary>
    /// 메인 방향에 수직인 방향들 구하기 (단일 방향만)
    /// </summary>
    private Vector3Int[] GetPerpendicularDirections(Vector3Int direction)
    {
        List<Vector3Int> perpendiculars = new List<Vector3Int>();
        
        // 메인 방향에 수직인 단일 축 방향만 선택
        if (direction.x != 0) // 좌우 방향이면 상하 수직
        {
            perpendiculars.Add(new Vector3Int(0, 1, 0));  // 위
            perpendiculars.Add(new Vector3Int(0, -1, 0)); // 아래
        }
        
        if (direction.y != 0) // 상하 방향이면 좌우 수직
        {
            perpendiculars.Add(new Vector3Int(1, 0, 0));  // 오른쪽
            perpendiculars.Add(new Vector3Int(-1, 0, 0)); // 왼쪽
        }
        
        // 대각선 방향인 경우 4방향 모두 가능
        if (direction.x != 0 && direction.y != 0)
        {
            perpendiculars.Clear();
            perpendiculars.Add(new Vector3Int(0, 1, 0));  // 위
            perpendiculars.Add(new Vector3Int(0, -1, 0)); // 아래
            perpendiculars.Add(new Vector3Int(1, 0, 0));  // 오른쪽
            perpendiculars.Add(new Vector3Int(-1, 0, 0)); // 왼쪽
        }
        
        return perpendiculars.ToArray();
    }

    /// <summary>
    /// 다음 위치 반환
    /// </summary>
    public Vector3Int GetNextPosition()
    {
        if (_currentStep >= _path.Count)
            return Vector3Int.zero;
            
        return _path[_currentStep];
    }

    /// <summary>
    /// 한 스텝 전진
    /// </summary>
    public void Advance()
    {
        _currentStep++;
        if (_currentStep >= _path.Count)
            IsComplete = true;
    }

    /// <summary>
    /// 그리드 범위 내 확인
    /// </summary>
    private bool IsWithinGrid(Vector3Int position)
    {
        return position.x >= 0 && position.x < 9 && 
               position.y >= 0 && position.y < 9;
    }
    
    /// <summary>
    /// 그리드 범위 내로 위치 조정
    /// </summary>
    private Vector3Int ClampToGrid(Vector3Int pos)
    {
        return new Vector3Int(
            Mathf.Clamp(pos.x, 0, 8),
            Mathf.Clamp(pos.y, 0, 8),
            0
        );
    }
}