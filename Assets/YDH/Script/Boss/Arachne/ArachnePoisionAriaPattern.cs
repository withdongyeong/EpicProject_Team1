using UnityEngine;
using System.Collections;
public class ArachnePoisionAriaPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    public string PatternName => "ArachnePoisionAria";

    /// <summary>
    /// 영역 공격 패턴 생성자
    /// </summary>
    public ArachnePoisionAriaPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteAreaAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// 독 분출
    /// </summary>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        // 플레이어 위치 가져오기
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        // 경고 타일 표시 (3x3 영역)
        GameObject[] warningTiles = new GameObject[9];
        int index = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;

                if (boss.GridSystem.IsValidPosition(tileX, tileY))
                {
                    Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                    warningTiles[index] = Object.Instantiate(_warningTilePrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }
        }

        // 타겟 영역 중앙 위치 계산
        Vector3 targetCenter = boss.GridSystem.GetWorldPosition(playerX, playerY);

        // 경고 대기
        yield return new WaitForSeconds(0.5f);

        // 플레이어가 영역 내에 있는지 확인
        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);
        if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
        {
            boss.ApplyDamageToPlayer(15);
        }

        // 공격 영역에 폭발 이펙트 생성
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;

                if (boss.GridSystem.IsValidPosition(tileX, tileY))
                {
                    Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                    boss.CreateDamageEffect(tilePos, _explosionEffectPrefab);
                }
            }
        }

        // 경고 타일 제거
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                Object.Destroy(tile);
            }
        }
    }
}
