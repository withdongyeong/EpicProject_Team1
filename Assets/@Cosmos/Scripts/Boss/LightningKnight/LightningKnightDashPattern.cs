using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LightningKnightDashPattern : IBossAttackPattern
{
    private Vector3 _startTransform;
    private List<Vector3Int> _gridDashPoint;
    public string PatternName => "LightningKnightDashPattern";

    public LightningKnightDashPattern(Vector3 startTransform, List<Vector3Int> GridDashPoint)
    {
        _startTransform = startTransform;
        _gridDashPoint = GridDashPoint;
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

        boss.SetAnimationTrigger("DashTrigger");

        foreach (var target in dashPoints)
        {
            // 방향 설정 (flipX)
            Vector3 direction = (target - boss.transform.position).normalized;

            // 공격 경고 생성
            if (_startTransform != target)
                boss.StartCoroutine(DamageAreaCreate(boss, boss.transform.position, target));

            yield return new WaitForSeconds(0.65f);

            // 이동
            yield return MoveOverTime(boss.transform, target, 0.05f);

            // 도착 후 약간 대기
            yield return new WaitForSeconds(0.2f);
        }

        // 대시 종료
        boss.SetAnimationTrigger("NoDashTrigger");
        boss.GetComponent<SpriteRenderer>().flipX = true;
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

                boss.BombHandler.ShowWarningOnly(DangerAreas, 0.8f, WarningType.Type2);

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

                boss.BombHandler.ShowWarningOnly(DangerAreas, 0.8f, WarningType.Type2);

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

                boss.BombHandler.ShowWarningOnly(DangerAreas, 0.8f, WarningType.Type2);

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

                boss.BombHandler.ShowWarningOnly(DangerAreas, 0.8f, WarningType.Type2);

                yield return new WaitForSeconds(0.08f);
            }
        }

        yield return 0;
    }
}
