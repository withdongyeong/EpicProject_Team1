using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game4.Scripts.Character.Player;

public class TreeTrapPattern : IBossAttackPattern
{
    public GameObject _warningTilePrefab;
    public GameObject _treeTrapPrefab;

    public PlayerController _playerController;
    public string PatternName => "Tree Trap";

    public TreeTrapPattern(GameObject warningTilePrefab, GameObject treeTrapPrefab, PlayerController playerController)
    {
        _warningTilePrefab = warningTilePrefab;
        _treeTrapPrefab = treeTrapPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(TreeTrap(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _warningTilePrefab != null && _treeTrapPrefab != null;
    }

    /// <summary>
    /// 나무 함정 생성
    /// </summary>
    private IEnumerator TreeTrap(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        ////가로 라인
        for (int x = 0; x < 8; x++)
        {
            Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int (x, playerY, 0));
            attackPositions.Add(pos);
            warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
        }

        // 세로 라인
        for (int y = 0; y < boss.GridSystem.Height; y++)
        {
            if (y != playerY)
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(0, y);
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.8f);

       GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        //세로 라인 데미지
        if (currentX == 0)
        {
            boss.ApplyDamageToPlayer(20);
        }

        //가로 라인 데미지
        if (currentY == playerY)
        {
            boss.ApplyDamageToPlayer(20);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _treeTrapPrefab);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }
}
