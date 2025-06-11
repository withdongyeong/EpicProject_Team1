using UnityEngine;
using System.Collections;
public class ArachnePoisionAriaPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    private PlayerController _playerController;
    
    public string PatternName => "ArachnePoisionAria";

    /// <summary>
    /// 영역 공격 패턴 생성자
    /// </summary>
    public ArachnePoisionAriaPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, PlayerController playerController)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteAreaAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// 독 분출
    /// </summary>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");

        // 플레이어 위치 가져오기
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        // 경고 타일 표시 (3x3 영역)
        GameObject[] warningTiles = new GameObject[9];
        int index = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                    warningTiles[index] = Object.Instantiate(_warningTilePrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }
        }

        // 경고 대기
        yield return new WaitForSeconds(0.6f);

        boss.AttackAnimation();
        SoundManager.Instance.ArachneSoundClip("PoisionExplotionActivate");

        // 플레이어가 영역 내에 있는지 확인
        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
        {
            boss.ApplyDamageToPlayer(10);
        }

        // 공격 영역에 폭발 이펙트 생성
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                    boss.CreateDamageEffect(tilePos, _explosionEffectPrefab, 0.7f);
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
