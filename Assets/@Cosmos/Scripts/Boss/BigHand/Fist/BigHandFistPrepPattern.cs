using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 주먹 꽂히기 전 플레이어 유도 패턴
/// </summary>
public class BigHandFistPrepPattern : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;
    private int _damage;

    public string PatternName => "주먹_유도";
    
    public BigHandFistPrepPattern(GameObject attackEffectPrefab, int damage)
    {
        _attackEffectPrefab = attackEffectPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null;
    }

    /// <summary>
    /// 주먹 영역에 폭탄 패턴으로 플레이어 밀어내기
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        Debug.Log("주먹 유도 패턴 시작");
        
        Vector3Int playerPos = new Vector3Int(boss.BombHandler.PlayerController.CurrentX, boss.BombHandler.PlayerController.CurrentY, 0);
        Vector3Int fistCenterColumn = CalculateFistCenterColumn(playerPos);
        List<Vector3Int> fistArea = GetFistArea(fistCenterColumn);
        
        // 보스에 주먹 영역 정보 저장
        var bigHand = boss as BigHand;
        if (bigHand != null)
        {
            bigHand.PlannedFistCenterColumn = fistCenterColumn;
            bigHand.PlannedFistArea = new List<Vector3Int>(fistArea);
        }
        
        yield return boss.StartCoroutine(ExecuteBombPattern(boss, fistArea));
        
        Debug.Log("주먹 유도 패턴 완료");
    }

    /// <summary>
    /// 주먹이 꽂힐 중심 열 계산 - 플레이어 반대편 선택
    /// </summary>
    private Vector3Int CalculateFistCenterColumn(Vector3Int playerPos)
    {
        int centerX;
        
        // 플레이어 위치에 따라 반대편 선택
        if (playerPos.x <= 4) // 플레이어가 왼쪽에 가까우면
        {
            centerX = 5; // 오른쪽 7칸 (X: 2-8) 중심
            Debug.Log($"플레이어 위치 {playerPos.x} - 오른쪽 7칸 선택 (중심: {centerX})");
        }
        else // 플레이어가 오른쪽에 가까우면
        {
            centerX = 3; // 왼쪽 7칸 (X: 0-6) 중심
            Debug.Log($"플레이어 위치 {playerPos.x} - 왼쪽 7칸 선택 (중심: {centerX})");
        }
        
        return new Vector3Int(centerX, 4, 0); // Y는 중앙값으로 설정
    }

    /// <summary>
    /// 주먹이 차지할 전체 영역 계산 (가로7칸 x 세로9칸)
    /// </summary>
    private List<Vector3Int> GetFistArea(Vector3Int centerColumn)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        int centerX = centerColumn.x;
        
        // 가로 7칸 (중심 ±3)
        for (int x = centerX - 3; x <= centerX + 3; x++)
        {
            // 세로 9칸 (전체)
            for (int y = 0; y < 9; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (IsWithinGrid(pos))
                {
                    area.Add(pos);
                }
            }
        }
        
        Debug.Log($"주먹 영역 계산 완료: 중심X={centerX}, 총 {area.Count}칸");
        return area;
    }

    /// <summary>
    /// 주먹 영역에 중심에서 확장되는 세로선 폭발 패턴
    /// </summary>
    private IEnumerator ExecuteBombPattern(BaseBoss boss, List<Vector3Int> fistArea)
    {
        // 유도 패턴을 반복
        for (int repeat = 0; repeat < 2; repeat++)
        {
            Debug.Log($"주먹 유도 {repeat + 1}차 시작");
            
            if (repeat % 2 == 0) // 짝수 차수는 세로선 확장
            {
                yield return boss.StartCoroutine(ExecuteSingleWavePattern(boss));
            }
            else // 홀수 차수는 가로선 밀어내기
            {
                yield return boss.StartCoroutine(ExecuteHorizontalPushPattern(boss));
            }
            
            if (repeat < 2) // 마지막이 아니면 간격 추가
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    /// <summary>
    /// 단일 확장 웨이브 패턴
    /// </summary>
    private IEnumerator ExecuteSingleWavePattern(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        Vector3Int centerColumn = bigHand.PlannedFistCenterColumn;
        int centerX = centerColumn.x;
        
        // 중심에서 바깥으로 확장하는 세로선 폭발
        for (int distance = 0; distance <= 3; distance++)
        {
            List<Vector3Int> currentWaveColumns = new List<Vector3Int>();
            
            if (distance == 0)
            {
                // 중심선
                currentWaveColumns.Add(new Vector3Int(centerX, 0, 0));
            }
            else
            {
                // 좌우 대칭으로 추가
                int leftX = centerX - distance;
                int rightX = centerX + distance;
                
                if (leftX >= 0)
                    currentWaveColumns.Add(new Vector3Int(leftX, 0, 0));
                if (rightX < 9)
                    currentWaveColumns.Add(new Vector3Int(rightX, 0, 0));
            }
            
            // 각 열에 대해 세로 전체 폭발
            foreach (Vector3Int columnStart in currentWaveColumns)
            {
                boss.StartCoroutine(ExecuteVerticalLineExplosion(boss, columnStart.x));
            }
            
            yield return new WaitForSeconds(0.15f); // 확장 간격
        }
    }
    
    /// <summary>
    /// 특정 X좌표 열에 세로 전체 폭발
    /// </summary>
    private IEnumerator ExecuteVerticalLineExplosion(BaseBoss boss, int columnX)
    {
        // 세로 9칸 전체에 동시 폭발
        List<Vector3Int> verticalLine = new List<Vector3Int>();
        for (int y = 0; y < 9; y++)
        {
            verticalLine.Add(new Vector3Int(0, y - 4, 0)); // 상대좌표로 변환 (중심을 기준으로)
        }
        
        // 해당 열의 중심점 (Y=4)을 기준으로 세로선 폭발
        Vector3Int centerOfColumn = new Vector3Int(columnX, 4, 0);

        boss.StartCoroutine(PlayAttackSound());

        boss.BombHandler.ExecuteFixedBomb(
            verticalLine,
            centerOfColumn,
            _attackEffectPrefab,
            warningDuration: 0.8f,
            explosionDuration: 0.8f,
            damage: _damage
        );
        
        yield break;
    }

    /// <summary>
    /// 가로선으로 플레이어를 밖으로 밀어내는 패턴
    /// </summary>
    private IEnumerator ExecuteHorizontalPushPattern(BaseBoss boss)
    {
        var bigHand = boss as BigHand;
        Vector3Int centerColumn = bigHand.PlannedFistCenterColumn;
        int centerX = centerColumn.x;
        
        // 중심에서 위아래로 확장하는 가로선 폭발
        for (int distance = 0; distance <= 4; distance++)
        {
            List<int> currentWaveRows = new List<int>();
            
            if (distance == 0)
            {
                // 중심선 (Y=4)
                currentWaveRows.Add(4);
            }
            else
            {
                // 위아래 대칭으로 추가
                int upperY = 4 + distance;
                int lowerY = 4 - distance;
                
                if (upperY < 9)
                    currentWaveRows.Add(upperY);
                if (lowerY >= 0)
                    currentWaveRows.Add(lowerY);
            }
            
            // 각 행에 대해 주먹 영역 가로 전체 폭발
            foreach (int rowY in currentWaveRows)
            {
                boss.StartCoroutine(ExecuteHorizontalLineExplosion(boss, centerX, rowY));
            }
            
            yield return new WaitForSeconds(0.15f); // 확장 간격
        }
    }
    
    /// <summary>
    /// 특정 Y좌표 행에 주먹 영역 가로 전체 폭발
    /// </summary>
    private IEnumerator ExecuteHorizontalLineExplosion(BaseBoss boss, int centerX, int rowY)
    {
        // 주먹 영역 가로 7칸에 동시 폭발
        List<Vector3Int> horizontalLine = new List<Vector3Int>();
        for (int x = centerX - 3; x <= centerX + 3; x++)
        {
            if (x >= 0 && x < 9) // 그리드 범위 내에서만
            {
                horizontalLine.Add(new Vector3Int(x - centerX, 0, 0)); // 상대좌표로 변환
            }
        }
        
        // 해당 행의 중심점을 기준으로 가로선 폭발
        Vector3Int centerOfRow = new Vector3Int(centerX, rowY, 0);

        boss.StartCoroutine(PlayAttackSound());

        boss.BombHandler.ExecuteFixedBomb(
            horizontalLine,
            centerOfRow,
            _attackEffectPrefab,
            warningDuration: 0.8f,
            explosionDuration: 0.7f,
            damage: _damage
        );
        
        yield break;
    }

    /// <summary>
    /// 그리드 범위 내 확인
    /// </summary>
    private bool IsWithinGrid(Vector3Int position)
    {
        return position.x >= 0 && position.x < 9 && 
               position.y >= 0 && position.y < 9;
    }

    public void Cleanup()
    {
        Debug.Log("주먹 유도 패턴 정리 완료");
    }

    public IEnumerator PlayAttackSound(string SoundName, float BombTime)
    {
        yield return new WaitForSeconds(BombTime); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip(SoundName);
    }

    public IEnumerator PlayAttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip("BigHandAttackActivate");
    }

}