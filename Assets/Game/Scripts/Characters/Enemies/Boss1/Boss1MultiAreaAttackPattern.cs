using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 5: 연속 영역 공격 (매직소드 애니메이션)
/// </summary>
public class Boss1MultiAreaAttackPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    private GameObject _magicSwordPrefab;
    private int _attackCount;
    private float _attackInterval;
    
    public string PatternName => "Multi Area Attack";
    
    public Boss1MultiAreaAttackPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, GameObject magicSwordPrefab, int attackCount = 3, float attackInterval = 0.3f)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _magicSwordPrefab = magicSwordPrefab;
        _attackCount = attackCount;
        _attackInterval = attackInterval;
    }
    
    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteMultiAreaAttack(boss));
    }
    
    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && _warningTilePrefab != null;
    }
    
    /// <summary>
    /// 연속 영역 공격 실행
    /// </summary>
    private IEnumerator ExecuteMultiAreaAttack(BaseBoss boss)
    {
        for (int i = 0; i < _attackCount; i++)
        {
            // 랜덤 위치 선택
            int targetX = Random.Range(0, boss.GridSystem.Width);
            int targetY = Random.Range(0, boss.GridSystem.Height);

            // 경고 타일 생성 (2x2 영역)
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> attackPositions = new List<Vector3>();

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int tileX = targetX + x;
                    int tileY = targetY + y;

                    if (boss.GridSystem.IsValidPosition(tileX, tileY))
                    {
                        Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                        attackPositions.Add(tilePos);
                        warningTiles.Add(TileObject.Instantiate(_warningTilePrefab, tilePos, Quaternion.identity));
                    }
                }
            }

            // 타겟 영역 중앙 위치 계산
            Vector3 targetCenter = boss.GridSystem.GetWorldPosition(targetX, targetY) + 
                                  new Vector3(boss.GridSystem.CellSize / 2, boss.GridSystem.CellSize / 2, 0);

            // 매직 소드 애니메이션 시작
            if (_magicSwordPrefab != null)
            {
                boss.StartCoroutine(MagicSwordAnimation(boss, targetCenter, 0.6f));
            }

            yield return new WaitForSeconds(0.6f);

            // 공격 실행
            boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);
            bool isHit = playerX >= targetX && playerX < targetX + 2 && 
                         playerY >= targetY && playerY < targetY + 2;

            if (isHit)
            {
                boss.ApplyDamageToPlayer(10);
            }

            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                boss.CreateDamageEffect(pos, _explosionEffectPrefab, 0.7f);
            }

            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                TileObject.Destroy(tile);
            }

            yield return new WaitForSeconds(_attackInterval);
        }
    }
    
    /// <summary>
    /// 매직 소드 애니메이션
    /// </summary>
    private IEnumerator MagicSwordAnimation(BaseBoss boss, Vector3 targetPosition, float waitTime)
    {
        Vector3 startPosition = boss.transform.position + Vector3.up * 2f;
        GameObject sword = TileObject.Instantiate(_magicSwordPrefab, startPosition, Quaternion.identity);

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
        TileObject.Destroy(sword);
    }
}