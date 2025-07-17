using UnityEngine;
using System.Collections;

public class SlimeTentaclePattern : IBossAttackPattern
{
    private GameObject _slimeTentacle;
    private int _tentatacleCount;

    private float tentacleLength = 5f;
    private float beat;
    private float halfBeat;

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
        boss.Unstoppable = true;
        beat = boss.Beat;
        halfBeat = boss.HalfBeat;
        Vector3 startPos = boss.transform.position;

        for (int i = 0; i < _tentatacleCount; i++)
        {
            int randomY = boss.BombHandler.PlayerController.CurrentY;

            for (int j = 8; j >= 0; j--)
            {
                Vector3Int warn = new Vector3Int(j, randomY, 0);
                boss.BombHandler.ShowWarningOnly(warn, 1f, WarningType.Type2);
            }
            yield return new WaitForSeconds(1-beat);
            
            Vector3Int endPos = new Vector3Int(11, randomY, 0);
            yield return MoveBossToPosition(boss, endPos);

            GameObject tentacle = GameObject.Instantiate(_slimeTentacle, boss.transform.position, Quaternion.identity);
            boss.StartCoroutine(GrowTentacle(tentacle, halfBeat)); // 성장 시간도 beat에 맞춤

            GameObject.Destroy(tentacle, halfBeat);
            yield return new WaitForSeconds(beat); // 다음 패턴 전 대기
        }

        yield return MoveBossToWorldPosition(boss, startPos); // beat 맞춤 이동
    }


    IEnumerator GrowTentacle(GameObject tentacle, float duration)
    {
        Vector3 originalScale = tentacle.transform.localScale;
        Vector3 targetScale = new Vector3(tentacleLength, originalScale.y, originalScale.z);

        BoxCollider2D collider = tentacle.GetComponent<BoxCollider2D>();
        Vector2 originalSize = collider.size;
        Vector2 originalOffset = collider.offset;

        float elapsed = 0;
        SoundManager.Instance.SlimeSoundClip("SlimeTentacleActivate");

        while (elapsed < duration && tentacle != null)
        {
            float t = elapsed / duration;

            float currentLength = Mathf.Lerp(originalScale.x, targetScale.x, t);
            tentacle.transform.localScale = new Vector3(currentLength, originalScale.y, originalScale.z);

            float colliderT = Mathf.Clamp01(t / 2);
            float colliderLength = Mathf.Lerp(originalScale.x, targetScale.x, colliderT);
            collider.size = new Vector2(colliderLength, originalSize.y);
            collider.offset = new Vector2(-colliderLength * 0.5f, originalOffset.y);

            elapsed += Time.deltaTime;
            yield return null;
        }
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
        float elapsed = 0f;

        Vector3 startPos = boss.transform.position;

        while (elapsed < halfBeat)
        {
            float t = elapsed / halfBeat;
            boss.transform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = targetWorldPos;
        boss.Unstoppable = false;
    }

}
