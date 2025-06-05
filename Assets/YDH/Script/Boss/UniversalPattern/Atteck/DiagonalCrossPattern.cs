using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DiagonalCrossPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    private PlayerController _playerController;

    public string PatternName => "Diagonal Cross Attack";

    public DiagonalCrossPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, PlayerController playerController)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteDiagonalCrossAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// 대각선 + 십자 공격 실행
    /// </summary>
    private IEnumerator ExecuteDiagonalCrossAttack(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        // 1단계: 대각선 경고 생성
        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // 우상단-좌하단 대각선
        int offset = playerY - playerX;
        for (int x = 0; x < 8; x++)
        {
            int y = x + offset;
            if (GridManager.Instance.IsWithinGrid(new Vector3Int(x, y, 0)))
            {
                Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(x, y, 0));
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.8f);

        // 대각선 공격 실행
        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        bool isOnDiagonal = (currentY - currentX) == (playerY - playerX);

        if (isOnDiagonal)
        {
            boss.ApplyDamageToPlayer(10);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }

        // 새로운 플레이어 위치 가져오기
        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        playerX = GridPosition.x;
        playerY = GridPosition.y;

        // 2단계: 가로 경고 생성
        warningTiles = new List<GameObject>();
        attackPositions = new List<Vector3>();

        yield return new WaitForSeconds(0.3f);

        // 가로 방향
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(x, playerY, 0));
            attackPositions.Add(pos);
            warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
        }

        yield return new WaitForSeconds(0.8f);

        // 가로방향
        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        currentX = GridPosition.x;
        currentY = GridPosition.y;

        if (currentY == playerY)
        {
            boss.ApplyDamageToPlayer(15);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }
}