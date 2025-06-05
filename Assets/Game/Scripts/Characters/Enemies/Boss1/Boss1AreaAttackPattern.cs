using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 1: 플레이어 위치 3x3 범위 공격 (매직소드 애니메이션)
/// </summary>
public class Boss1AreaAttackPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    private GameObject _magicSwordPrefab;
    
    public string PatternName => "Area Attack";
    
    /// <summary>
    /// 영역 공격 패턴 생성자
    /// </summary>
    public Boss1AreaAttackPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, GameObject magicSwordPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab; 
        _magicSwordPrefab = magicSwordPrefab;
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
    /// 영역 공격 실행
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
                    warningTiles[index] = ItemObject.Instantiate(_warningTilePrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }
        }

        // 타겟 영역 중앙 위치 계산
        Vector3 targetCenter = boss.GridSystem.GetWorldPosition(playerX, playerY);

        // 매직 소드 애니메이션 시작
        if (_magicSwordPrefab != null)
        {
            boss.StartCoroutine(MagicSwordAnimation(boss, targetCenter, 1.5f));
        }

        // 경고 대기
        yield return new WaitForSeconds(1.5f);

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
                ItemObject.Destroy(tile);
            }
        }
    }
    
    /// <summary>
    /// 매직 소드 애니메이션
    /// </summary>
    private IEnumerator MagicSwordAnimation(BaseBoss boss, Vector3 targetPosition, float waitTime)
    {
        Vector3 startPosition = boss.transform.position + Vector3.up * 2f;
        GameObject sword = ItemObject.Instantiate(_magicSwordPrefab, startPosition, Quaternion.identity);

        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        sword.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        float totalAnimTime = waitTime;
        float riseTime = totalAnimTime * 0.2f;
        float riseHeight = 4f;
        Vector3 risePosition = startPosition + Vector3.up * riseHeight;

        float elapsed = 0f;
        while (elapsed < riseTime)
        {
            sword.transform.position = Vector3.Lerp(startPosition, risePosition, elapsed / riseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        float hoverTime = totalAnimTime * 0.5f;
        yield return new WaitForSeconds(hoverTime);

        float fallTime = totalAnimTime * 0.3f;
        elapsed = 0f;

        while (elapsed < fallTime)
        {
            float t = elapsed / fallTime;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            sword.transform.position = Vector3.Lerp(risePosition, targetPosition, easedT);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sword.transform.position = targetPosition;
        ItemObject.Destroy(sword);
    }
}