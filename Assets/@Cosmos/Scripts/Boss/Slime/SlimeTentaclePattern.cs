using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class SlimeTentaclePattern : IBossAttackPattern
{
    private GameObject _slimeTentacle;
    private int _tentatacleCount;

    private float moveDuration = 0.3f;
    private float waitDuration = 0.2f;
    private float tentacleLength = 3f;
    private float tentacleGrowTime = 0.5f;
    private float tentacleLifetime = 2f;

    public string PatternName => "SlimeTentaclePattern";

    public SlimeTentaclePattern(GameObject SlimeTentacle, int TentatacleCount)
    {
        _slimeTentacle = SlimeTentacle;
        _tentatacleCount = TentatacleCount;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return 0;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return true;
    }

    public IEnumerator SlimeTentacle(BaseBoss boss)
    {
        // 1. 랜덤 위치 선택 (0~9)
        float randomY = Random.Range(0, 9);
        Vector3 targetPosition = new Vector3(0, randomY, 0);

        // 2. 천천히 이동
        Vector3 startPos = boss.gameObject.transform.position;
        float elapsed = 0;
        while (elapsed < moveDuration)
        {
            boss.gameObject.transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.gameObject.transform.position = targetPosition;

        // 3. 0.2초 대기
        yield return new WaitForSeconds(waitDuration);

        // 4. 촉수 생성
        GameObject tentacle = GameObject.Instantiate(_slimeTentacle, boss.gameObject.transform.position, Quaternion.identity);
        boss.StartCoroutine(GrowTentacle(tentacle));

        // 5. 일정 시간 후 삭제
        GameObject.Destroy(tentacle, tentacleLifetime);

        // 패턴 반복 대기 시간 (옵션)
        yield return new WaitForSeconds(1f);
    }

    IEnumerator GrowTentacle(GameObject tentacle)
    {
        Vector3 originalScale = tentacle.transform.localScale;
        Vector3 targetScale = new Vector3(tentacleLength, originalScale.y, originalScale.z);
        float elapsed = 0;

        while (elapsed < tentacleGrowTime)
        {
            float t = elapsed / tentacleGrowTime;
            tentacle.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tentacle.transform.localScale = targetScale;
    }
}
