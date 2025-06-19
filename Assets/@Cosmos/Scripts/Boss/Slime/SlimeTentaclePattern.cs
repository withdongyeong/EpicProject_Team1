using UnityEngine;
using System.Collections;

public class SlimeTentaclePattern : IBossAttackPattern
{
    private GameObject _slimeTentacle;
    private int _tentatacleCount;

    private float waitDuration = 0.2f;
    private float tentacleLength = 15f;
    private float tentacleGrowTime = 0.4f;
    private float tentacleLifetime = 0.6f;

    public string PatternName => "SlimeTentaclePattern";

    public SlimeTentaclePattern(GameObject SlimeTentacle, int TentatacleCount)
    {
        _slimeTentacle = SlimeTentacle;
        _tentatacleCount = TentatacleCount;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return SlimeTentacle(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeTentacle != null;
    }

    public IEnumerator SlimeTentacle(BaseBoss boss)
    {
        Vector3 Startposition = boss.transform.position;

        for (int i = 0; i < _tentatacleCount; i++)
        {
            // 1. 랜덤 위치 선택 (0~9)
            int randomY = boss.BombHandler.PlayerController.CurrentY;

            // 2. 위험 알림
            for(int j = 8; j >= 0; j--)
            {
                Vector3Int WarningPosision = new Vector3Int(j, randomY, 0);
                boss.BombHandler.ShowWarningOnly(WarningPosision, 0.8f, WarningType.Type2);
                yield return new WaitForSeconds(0.05f);
            }

            Vector3Int endPosition = new Vector3Int(11, randomY, 0);

            // 3. 천천히 이동
            yield return MoveBossToPosition(boss, endPosition);

            // 4. 0.2초 대기
            yield return new WaitForSeconds(waitDuration);

            // 5. 촉수 생성
            GameObject tentacle = GameObject.Instantiate(_slimeTentacle, boss.gameObject.transform.position, Quaternion.identity);
            boss.StartCoroutine(GrowTentacle(tentacle));

            // 6. 일정 시간 후 삭제
            GameObject.Destroy(tentacle, tentacleLifetime);

            yield return new WaitForSeconds(1f);
        }

        yield return MoveBossToWorldPosition(boss, Startposition);
    }

    IEnumerator GrowTentacle(GameObject tentacle)
    {
        Vector3 originalScale = tentacle.transform.localScale;
        Vector3 targetScale = new Vector3(tentacleLength, originalScale.y, originalScale.z);
        Vector3 startPosition = tentacle.transform.position;

        float elapsed = 0;

        SoundManager.Instance.SlimeSoundClip("SlimeTentacleActivate");
        while (elapsed < tentacleGrowTime)
        {
            float t = elapsed / tentacleGrowTime;

            // Scale 증가
            float currentLength = Mathf.Lerp(originalScale.x, targetScale.x, t);
            tentacle.transform.localScale = new Vector3(currentLength, originalScale.y, originalScale.z);

            // 왼쪽 방향으로 길어지게 위치 보정
            tentacle.transform.position = startPosition + new Vector3(-(currentLength / 2f), 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        tentacle.transform.localScale = targetScale;
        tentacle.transform.position = startPosition + new Vector3(-(targetScale.x / 2f), 0, 0);
    }

    /// <summary>
    /// 보스를 특정 위치로 이동 (그리드 좌표)
    /// </summary>
    private IEnumerator MoveBossToPosition(BaseBoss boss, Vector3Int targetGridPos)
    {
        Vector3 targetWorldPos = boss.GridSystem.GridToWorldPosition(targetGridPos);
        yield return MoveBossToWorldPosition(boss, targetWorldPos);
    }

    /// <summary>
    /// 보스를 월드 좌표로 이동
    /// </summary>
    private IEnumerator MoveBossToWorldPosition(BaseBoss boss, Vector3 targetWorldPos)
    {
        float moveSpeed = 15f; // 이동 속도

        while (Vector3.Distance(boss.transform.position, targetWorldPos) > 0.1f)
        {
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        boss.transform.position = targetWorldPos;
    }
}
