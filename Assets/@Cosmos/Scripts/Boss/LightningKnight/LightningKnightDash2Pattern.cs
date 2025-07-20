using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightDash2Pattern : IBossAttackPattern
{
    private Vector3 _startTransform;
    private List<Vector3Int> _gridDashPoint;
    private GameObject _AttackEffect;
    private int _weakDamage;
    private int _strongDamage;

    public string PatternName => "LightningKnightDashPattern";

    public LightningKnightDash2Pattern(Vector3 startTransform, List<Vector3Int> GridDashPoint, GameObject AttackEffect, int weakDamage, int strongDamage)
    {
        _startTransform = startTransform;
        _gridDashPoint = GridDashPoint;
        _AttackEffect = AttackEffect;
        _weakDamage = weakDamage;
        _strongDamage = strongDamage;
    }
    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 웨이브 패턴 실행 - 매 웨이브마다 다른 방향에서 시작
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.Unstoppable = true;
        Vector3 originalPosition = _startTransform;

        List<Vector3> dashPoints = DashPointCreate(_gridDashPoint);
        dashPoints.Add(originalPosition);

        boss.boolAnimation("IsDash",true);

        foreach (var target in dashPoints)
        {
            // 방향 설정 (flipX)
            Vector3 direction = (target - boss.transform.position).normalized;

            // 공격 경고 생성
            boss.StartCoroutine(DamageAreaCreate(boss, boss.transform.position, target));

            boss.StartCoroutine(WaveAreaCreate(boss, boss.transform.position, target));
            yield return new WaitForSeconds(boss.Beat);

            // 이동
            yield return MoveOverTime(boss.transform, target, boss.Beat / 8);

            // 도착 후 약간 대기
            yield return new WaitForSeconds(boss.Beat / 4);
        }

        boss.Unstoppable = false;
        // 대시 종료
        boss.boolAnimation("IsDash", false);
        
        // 후폭풍 종료 대기
        yield return new WaitForSeconds(boss.Beat * 4);
    }

    private List<Vector3> DashPointCreate(List<Vector3Int> GridDashPoint)
    {
        List<Vector3> DashPoint = new List<Vector3>();

        foreach (var point in GridDashPoint)
        {
            DashPoint.Add(GridManager.Instance.GridToWorldPosition(point));
        }

        return DashPoint;
    }


    private IEnumerator MoveOverTime(Transform target, Vector3 destination, float duration)
    {
        Vector3 start = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.position = Vector3.Lerp(start, destination, t);
            yield return null;
        }

        target.position = destination;
    }

    IEnumerator DamageAreaCreate(BaseBoss boss, Vector3 StartPoint, Vector3 EndPoint)
    {
        Vector3Int GridStartPoint = GridManager.Instance.WorldToGridPosition(StartPoint);
        Vector3Int GridEndPoint = GridManager.Instance.WorldToGridPosition(EndPoint);

        // 수직 이동 (x가 같고, 아래에서 위로 - x=1)
        if (GridStartPoint.x == GridEndPoint.x && GridStartPoint.x == 1)
        {
            for (int vertical = 7; vertical >= 0; vertical--)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                if (vertical == 7)
                {
                    // 첫 번째만: 시작점 중심 3x3 전체 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical + y, 0));
                        }
                    }
                }
                else
                {
                    // 나머지 모든 이동: 새로 추가되는 아래쪽 3칸만 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical - 1, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));
                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 1f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        // 수직 이동 (x가 같고, 위에서 아래로 - x=7)
        if (GridStartPoint.x == GridEndPoint.x && GridStartPoint.x == 7)
        {
            for (int vertical = 1; vertical <= 8; vertical++)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                if (vertical == 1)
                {
                    // 첫 번째만: 시작점 중심 3x3 전체 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical + y, 0));
                        }
                    }
                }
                else
                {
                    // 나머지 모든 이동: 새로 추가되는 위쪽 3칸만 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical + 1, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));
                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 1f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        // 수평 이동 (y가 같고, 왼쪽에서 오른쪽으로 - x=1)
        if (GridStartPoint.y == GridEndPoint.y && GridStartPoint.x == 1)
        {
            for (int horizontal = 1; horizontal <= 8; horizontal++)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                if (horizontal == 1)
                {
                    // 첫 번째만: 시작점 중심 3x3 전체 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            DangerAreas.Add(new Vector3Int(horizontal + x, GridStartPoint.y + y, 0));
                        }
                    }
                }
                else
                {
                    // 나머지 모든 이동: 새로 추가되는 오른쪽 3칸만 생성
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(horizontal + 1, GridStartPoint.y + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));
                boss.BombHandler.ShowWarningOnly(DangerAreas, 1f, WarningType.Type2); 
                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 0.8f, 25, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        // 수평 이동 (y가 같고, 오른쪽에서 왼쪽으로 - x=7)
        if (GridStartPoint.y == GridEndPoint.y && GridStartPoint.x == 7)
        {
            for (int horizontal = 7; horizontal >= 0; horizontal--)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                if (horizontal == 7)
                {
                    // 첫 번째만: 시작점 중심 3x3 전체 생성
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            DangerAreas.Add(new Vector3Int(horizontal + x, GridStartPoint.y + y, 0));
                        }
                    }
                }
                else
                {
                    // 나머지 모든 이동: 새로 추가되는 왼쪽 3칸만 생성
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(horizontal - 1, GridStartPoint.y + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));
                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 1f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        yield return 0;
}

    IEnumerator WaveAreaCreate(BaseBoss boss, Vector3 StartPoint, Vector3 EndPoint)
    {
        yield return new WaitForSeconds(boss.Beat / 2);
        Vector3Int gridStart = GridManager.Instance.WorldToGridPosition(StartPoint);
        Vector3Int gridEnd = GridManager.Instance.WorldToGridPosition(EndPoint);

        Vector3Int dir = gridEnd - gridStart;

        // 수평 이동 (왼쪽 또는 오른쪽)
        if (dir.x != 0 && dir.y == 0)
        {
            // 보스가 지나간 경로를 따라 양쪽으로 번개 웨이브 뻗어나가기
            yield return CreateHorizontalLightningWave(boss, gridStart, gridEnd);
        }
        // 수직 이동 (위 또는 아래)
        else if (dir.y != 0 && dir.x == 0)
        {
            // 보스가 지나간 경로를 따라 양쪽으로 번개 웨이브 뻗어나가기
            yield return CreateVerticalLightningWave(boss, gridStart, gridEnd);
        }

        yield return null;
    }

    private IEnumerator CreateHorizontalLightningWave(BaseBoss boss, Vector3Int gridStart, Vector3Int gridEnd)
    {
        // 보스가 지나간 경로의 Y좌표
        int pathY = gridStart.y;
        if (pathY <= 3)
        {
            pathY = 3;
        }
        else
        {
            pathY = 6;
        }
    
        // 이미 공격한 위치를 추적하는 HashSet
        HashSet<Vector3Int> attackedPositions = new HashSet<Vector3Int>();
    
        // 경로 위아래로 번개 웨이브 생성
        for (int waveDistance = 1; waveDistance <= 4; waveDistance++)
        {
            // 위쪽 웨이브
            if (pathY + waveDistance >= 0 && pathY + waveDistance <= 4)
            {
                yield return CreateLightningBranch(boss, gridStart.x, gridEnd.x, pathY + waveDistance, waveDistance, attackedPositions);
            }
        
            // 아래쪽 웨이브
            if (pathY - waveDistance >= 0 && pathY - waveDistance <= 4)
            {
                yield return CreateLightningBranch(boss, gridStart.x, gridEnd.x, pathY - waveDistance, waveDistance, attackedPositions);
            }

            yield return new WaitForSeconds(boss.Beat / 8);
        }
    }

    private IEnumerator CreateVerticalLightningWave(BaseBoss boss, Vector3Int gridStart, Vector3Int gridEnd)
    {
        // 보스가 지나간 경로의 X좌표
        int pathX = gridStart.x;
        if (pathX <= 3)
        {
            pathX = 3;
        }
        else
        {
            pathX = 6;
        }
    
        // 이미 공격한 위치를 추적하는 HashSet
        HashSet<Vector3Int> attackedPositions = new HashSet<Vector3Int>();
    
        // 경로 좌우로 번개 웨이브 생성
        for (int waveDistance = 1; waveDistance <= 4; waveDistance++)
        {
            // 오른쪽 웨이브
            if (pathX + waveDistance >= 0 && pathX + waveDistance <= 4)
            {
                yield return CreateLightningBranch(boss, pathX + waveDistance, gridStart.y, gridEnd.y, waveDistance, attackedPositions);
            }
        
            // 왼쪽 웨이브
            if (pathX - waveDistance >= 0 && pathX - waveDistance <= 4)
            {
                yield return CreateLightningBranch(boss, pathX - waveDistance, gridStart.y, gridEnd.y, waveDistance, attackedPositions);
            }

            yield return new WaitForSeconds(boss.Beat / 8);
        }
    }

    private IEnumerator CreateLightningBranch(BaseBoss boss, int startCoord, int endCoord, int fixedCoord, int waveDistance, HashSet<Vector3Int> attackedPositions)
    {
        // 번개가 지그재그로 뻗어나가는 효과
        int step = startCoord < endCoord ? 1 : -1;
    
        for (int coord = startCoord; coord != endCoord + step; coord += step)
        {
            // 번개의 불규칙한 패턴 생성
            List<Vector3Int> lightningPositions = new List<Vector3Int>();
        
            // 거리에 따라 번개 강도 조절
            int branchCount = Mathf.Max(1, 3 - waveDistance);
        
            for (int branch = 0; branch < branchCount; branch++)
            {
                int randomOffset = Random.Range(-1, 2); // -1, 0, 1
                Vector3Int lightningPos;
            
                if (startCoord == endCoord) // 수직 이동인 경우
                {
                    lightningPos = new Vector3Int(fixedCoord + randomOffset, coord, 0);
                }
                else // 수평 이동인 경우
                {
                    lightningPos = new Vector3Int(coord, fixedCoord + randomOffset, 0);
                }
            
                // 그리드 범위 체크 및 중복 체크
                if (lightningPos.x >= 0 && lightningPos.x <= 8 && lightningPos.y >= 0 && lightningPos.y <= 8 
                    && !attackedPositions.Contains(lightningPos))
                {
                    lightningPositions.Add(lightningPos);
                    attackedPositions.Add(lightningPos); // 공격한 위치 기록
                }
            }
        
            if (lightningPositions.Count > 0)
            {
                boss.StartCoroutine(ShowLightningWarning(boss, lightningPositions));
            }
        
            yield return new WaitForSeconds(boss.Beat/8);
        }
    }

    private IEnumerator ShowLightningWarning(BaseBoss boss, List<Vector3Int> positions)
    {
        boss.StartCoroutine(AttacKSoundPlay("KnightAttackActivate"));

        boss.BombHandler.ExecuteFixedBomb(positions, new Vector3Int(0, 0, 0), _AttackEffect,
                                      warningDuration: 1f, explosionDuration: 0.8f, damage: _weakDamage, WarningType.Type1);
        yield return null;
    }

    private IEnumerator AttacKSoundPlay(string SoundName)
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.KnightSoundClip(SoundName);
    }

}