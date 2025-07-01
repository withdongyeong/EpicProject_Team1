using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LightningKnightDashPattern : IBossAttackPattern
{
    private Vector3 _startTransform;
    private int _dashCount;

    public string PatternName => "LightningKnightDashPattern";

    public LightningKnightDashPattern(Vector3 startTransform,int dashCount)
    {
        _startTransform = startTransform;
        _dashCount = dashCount;
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
        boss.SetAnimationTrigger("DashTrigger");
        for (int i = 0; i < _dashCount; i++)
        {
            // 1. 순간적으로 카메라 밖으로 이동 (예: 왼쪽으로)
            boss.transform.position += new Vector3(10f, 0, 0);

            // 2. 8방향 중 하나를 선택 (정방향 및 대각선)
            Vector2[] directions = new Vector2[]
            {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
            };

            int dirIndex = Random.Range(0, directions.Length);
            Vector2 selectedDir = directions[dirIndex];

            if(selectedDir == Vector2.left) boss.GetComponent<SpriteRenderer>().flipX = false;
            else boss.GetComponent<SpriteRenderer>().flipX = true;

            boss.StartCoroutine(DamageAreaCreate(boss, selectedDir));

            // 이동 거리 설정
            float dashDistance = 10f;
            Vector3 targetPosition = new Vector3(selectedDir.x, selectedDir.y, 0) * dashDistance;

            // 3. 천천히 방향으로 이동
            boss.transform.position = targetPosition;

            // 4. 도착 후 1초 대기
            yield return new WaitForSeconds(1f);

            // 5. 현재 위치 기준 (0,0,0)에 대한 점대칭 좌표 계산
            Vector3 mirroredPosition = -boss.transform.position;

            // 6. 빠르게 점대칭 위치로 이동
            yield return MoveOverTime(boss.transform, mirroredPosition, 0.4f);
        }

        // 7. 잠깐 대기 후 복귀
        yield return new WaitForSeconds(0.4f);
        boss.SetAnimationTrigger("NoDashTrigger");
        boss.GetComponent<SpriteRenderer>().flipX = true;
        boss.transform.position = originalPosition;
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

    IEnumerator DamageAreaCreate(BaseBoss boss, Vector2 direction)
    {
        yield return new WaitForSeconds(0.2f);
        // 방향을 정규화해서 4방향 체크
        direction = direction.normalized;

        // 9칸의 웨이브 생성 (-4 ~ 4)
        for (int i = 4; i >= -4; i--)
        {
            List<Vector3Int> dashPositions = new List<Vector3Int>();

            float x = direction.x * i;
            float y = direction.y * i;

            for(int X = -1; X <= 1; X++)
            {
                for (int Y = -1; Y <= 1; Y++)
                {
                    dashPositions.Add(new Vector3Int(Mathf.RoundToInt(x) + X, Mathf.RoundToInt(y) + Y, 0));
                }
            }

            boss.BombHandler.ExecuteWarningThenDamage(dashPositions, new Vector3Int(4, 4, 0),
                                         warningDuration: 0.8f, damage: 0, WarningType.Type2);

            yield return new WaitForSeconds(0.05f);
        }

    }


}
