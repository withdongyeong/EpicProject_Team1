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

        // 대시 종료
        boss.boolAnimation("IsDash", false);
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

        //포지션 2개를 받고 x가 같다 혹은 y가 같으면 위험 생성 다르면 패스
        if (GridStartPoint.x == GridEndPoint.x && GridStartPoint.x == 1)
        {
            for (int vertical = 7; vertical > 0; vertical--)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 1f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        if (GridStartPoint.x == GridEndPoint.x && GridStartPoint.x == 7)
        {
            for (int vertical = 1; vertical < 8; vertical++)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(GridStartPoint.x + x, vertical + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 1f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }

        if (GridStartPoint.y == GridEndPoint.y && GridStartPoint.x == 1)
        {
            for (int horizontal = 1; horizontal < 8; horizontal++)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(horizontal + x, GridStartPoint.y + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));

                boss.BombHandler.ShowWarningOnly(DangerAreas, 1f, WarningType.Type2); boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 0.8f, 25, WarningType.Type2);

                yield return new WaitForSeconds(boss.Beat / 8);
            }
        }


        if (GridStartPoint.y == GridEndPoint.y && GridStartPoint.x == 7)
        {
            for (int horizontal = 7; horizontal > 0; horizontal--)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(horizontal + x, GridStartPoint.y + y, 0));
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
        
        // 경로 위아래로 번개 웨이브 생성
        for (int waveDistance = 1; waveDistance <= 4; waveDistance++)
        {
            // 위쪽 웨이브
            if (pathY + waveDistance >= 0 && pathY + waveDistance <= 8)
            {
                yield return CreateLightningBranch(boss, gridStart.x, gridEnd.x, pathY + waveDistance, waveDistance);
            }
            
            // 아래쪽 웨이브
            if (pathY - waveDistance >= 0 && pathY - waveDistance <= 8)
            {
                yield return CreateLightningBranch(boss, gridStart.x, gridEnd.x, pathY - waveDistance, waveDistance);
            }

            yield return new WaitForSeconds(boss.Beat / 8);
        }
    }

    private IEnumerator CreateVerticalLightningWave(BaseBoss boss, Vector3Int gridStart, Vector3Int gridEnd)
    {
        // 보스가 지나간 경로의 X좌표
        int pathX = gridStart.x;
        
        // 경로 좌우로 번개 웨이브 생성
        for (int waveDistance = 1; waveDistance <= 4; waveDistance++)
        {
            // 오른쪽 웨이브
            if (pathX + waveDistance >= 0 && pathX + waveDistance <= 8)
            {
                yield return CreateLightningBranch(boss, pathX + waveDistance, gridStart.y, gridEnd.y, waveDistance);
            }
            
            // 왼쪽 웨이브
            if (pathX - waveDistance >= 0 && pathX - waveDistance <= 8)
            {
                yield return CreateLightningBranch(boss, pathX - waveDistance, gridStart.y, gridEnd.y, waveDistance);
            }

            yield return new WaitForSeconds(boss.Beat / 8);
        }
    }

    private IEnumerator CreateLightningBranch(BaseBoss boss, int startCoord, int endCoord, int fixedCoord, int waveDistance)
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
                
                // 그리드 범위 체크
                if (lightningPos.x >= 0 && lightningPos.x <= 8 && lightningPos.y >= 0 && lightningPos.y <= 8)
                {
                    lightningPositions.Add(lightningPos);
                }
            }
            
            if (lightningPositions.Count > 0)
            {
                boss.StartCoroutine(ShowLightningWarning(boss, lightningPositions));
            }
            
            yield return new WaitForSeconds(boss.Beat / 8);
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