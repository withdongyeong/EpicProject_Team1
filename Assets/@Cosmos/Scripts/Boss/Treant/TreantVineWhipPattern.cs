using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreantVineWhipPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    private int _whipCount;

    private PlayerController _playerController;
    public string PatternName => "Diagonal Attack";

    public TreantVineWhipPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, int whipCount, PlayerController playerController)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _whipCount = whipCount;
        _playerController = playerController;
    }
    
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteDiagonalAttack(boss));
    }
    

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.BombManager.PlayerController != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// 덩굴 채찍 공격
    /// </summary>
    private IEnumerator ExecuteDiagonalAttack(BaseBoss boss)
    {
        for(int i = 0; i < _whipCount; i++)
        {
            if (i % 2 == 0)
            {
                boss.StartCoroutine(TopLeftDiagonalAttack(boss));
            }
            else
            {
                boss.StartCoroutine(TopRightDiagonalAttack(boss));
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator TopLeftDiagonalAttack(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // 좌상단-우하단 대각선
        int offset = playerY + playerX;
        for (int x = 0; x < 8; x++)
        {
            int y = offset - x;
            if (GridManager.Instance.IsWithinGrid(new Vector3Int (x, y, 0)))
            {
                Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int (x, y, 0));
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.3f);

        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        // 좌상단-우하단 대각선 검사
        bool isOnDiagonal2 = (currentY + currentX) == (playerY + playerX);

        if (isOnDiagonal2)
        {
            boss.ApplyDamageToPlayer(15);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab, 0.7f);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }

    private IEnumerator TopRightDiagonalAttack(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

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

        yield return new WaitForSeconds(0.5f);

        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        // 우상단-좌하단 대각선 검사
        bool isOnDiagonal1 = (currentY - currentX) == (playerY - playerX);

        if (isOnDiagonal1)
        {
            boss.ApplyDamageToPlayer(15);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab, 0.7f);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }
}
