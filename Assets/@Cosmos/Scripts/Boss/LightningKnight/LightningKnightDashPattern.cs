using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LightningKnightDashPattern : IBossAttackPattern
{
    private Vector3 _startTransform;
    private List<Vector3Int> _gridDashPoint;
    private GameObject _AttackEffect;
    private int _weakDamage;
    private int _strongDamage;
    
    public string PatternName => "LightningKnightDashPattern";

    public LightningKnightDashPattern(Vector3 startTransform, List<Vector3Int> GridDashPoint, GameObject attackEffect, int weakDamage, int strongDamage)
    {
        _startTransform = startTransform;
        _gridDashPoint = GridDashPoint;
        _AttackEffect = attackEffect;
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

        boss.boolAnimation("IsDash", true);

        foreach (var target in dashPoints)
        {
            // 방향 설정 (flipX)
            Vector3 direction = (target - boss.transform.position).normalized;

            // 공격 경고 생성
            if (_startTransform != target)
                boss.StartCoroutine(DamageAreaCreate(boss, boss.transform.position, target));

            //웨이브 생성
            if (target != originalPosition)
                boss.StartCoroutine(WaveAreaCreate(boss, boss.transform.position, target));

            yield return new WaitForSeconds(0.65f);

            // 이동
            yield return MoveOverTime(boss.transform, target, 0.05f);

            // 도착 후 약간 대기
            yield return new WaitForSeconds(0.2f);
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
            for(int vertical =1; vertical < 8; vertical++)
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

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0,0,0) ,0.8f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(0.08f);
            }
        }

        if (GridStartPoint.x == GridEndPoint.x && GridStartPoint.x == 7)
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

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 0.8f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(0.08f);
            }
        }

        if (GridStartPoint.y == GridEndPoint.y && GridStartPoint.x == 1)
        {
            for (int horizontal = 1; horizontal < 8; horizontal++)
            {
                List<Vector3Int> DangerAreas = new List<Vector3Int>();

                for (int x =-1; x<=1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        DangerAreas.Add(new Vector3Int(horizontal + x, GridStartPoint.y + y, 0));
                    }
                }

                boss.StartCoroutine(AttacKSoundPlay("KnightDashActivate"));

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 0.8f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(0.08f);
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

                boss.BombHandler.ExecuteWarningThenDamage(DangerAreas, new Vector3Int(0, 0, 0), 0.8f, _strongDamage, WarningType.Type2);

                yield return new WaitForSeconds(0.08f);
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
            if (dir.x > 0) // 오른쪽
            {
                int y = gridEnd.y - 2;

                for (int x = 0; x <= 8; x++)
                {
                    int EndY;

                    if (x <= 1 || x >= 7) EndY = y;
                    else if (x <= 3 || x >= 5) EndY = y - 1;
                    else EndY = y - 2;

                    for (int dy = y; dy >= EndY; dy--)
                    {
                        List<Vector3Int> danger = new List<Vector3Int> { new Vector3Int(x, dy, 0) };
                        boss.StartCoroutine(ShowWarningAsync(boss, danger, 0.8f, WarningType.Type1));

                         yield return new WaitForSeconds(0.03f);
                    }
                }
            }
            else // 왼쪽
            {
                int y = gridEnd.y + 2;

                for (int x = 8; x >= 0; x--)
                {
                    int EndY;

                    if (x <= 1 || x >= 7) EndY = y;
                    else if (x <= 3 || x >= 5) EndY = y + 1;
                    else EndY = y + 2;

                    for (int dy = y; dy <= EndY; dy++)
                    {
                        List<Vector3Int> danger = new List<Vector3Int> { new Vector3Int(x, dy, 0) };
                        boss.StartCoroutine(ShowWarningAsync(boss, danger, 0.8f, WarningType.Type1));

                        yield return new WaitForSeconds(0.03f);
                    }
                }
            }
        }
        // 수직 이동 (위 또는 아래)
        else if (dir.y != 0 && dir.x == 0)
        {
            if (dir.y > 0) // 위로
            {
                int x = gridEnd.x + 2;

                for (int y = 0; y <= 8; y++)
                {
                    int EndX;

                    if (y <= 1 || y >= 7) EndX = x;
                    else if (y <= 3 || y >= 5) EndX = x + 1;
                    else EndX = x + 2;

                    for (int dx = x; dx <= EndX; dx++)
                    {
                        List<Vector3Int> danger = new List<Vector3Int> { new Vector3Int(dx, y, 0) };
                        boss.StartCoroutine(ShowWarningAsync(boss, danger, 0.8f, WarningType.Type1));

                        yield return new WaitForSeconds(0.03f);
                    }
                }
            }
            else // 아래로
            {
                int x = gridEnd.x - 2;

                for (int y = 8; y >= 0; y--)
                {
                    int EndX;

                    if (y <= 1 || y >= 7) EndX = x;
                    else if (y <= 3 || y >= 5) EndX = x - 1;
                    else EndX = x - 2;

                    for (int dx = x; dx >= EndX; dx--)
                    {
                        List<Vector3Int> danger = new List<Vector3Int> { new Vector3Int(dx, y, 0) };
                        boss.StartCoroutine(ShowWarningAsync(boss, danger, 0.8f, WarningType.Type1));

                        yield return new WaitForSeconds(0.03f);
                    }
                }
            }
        }

        yield return null;
    }

    private IEnumerator ShowWarningAsync(BaseBoss boss, List<Vector3Int> pos, float duration, WarningType type)
    {
        boss.StartCoroutine(AttacKSoundPlay("KnightAttackActivate"));

        boss.BombHandler.ExecuteFixedBomb(pos, new Vector3Int(0, 0, 0), _AttackEffect,
                                      warningDuration: 0.8f, explosionDuration: duration, damage: _weakDamage, type);
        yield return null;
    }

    private IEnumerator AttacKSoundPlay(string SoundName)
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.KnightSoundClip(SoundName);
    }
}


