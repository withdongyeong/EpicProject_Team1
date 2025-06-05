using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeTrapPattern : IBossAttackPattern
{
    public GameObject _warningTilePrefab;
    public GameObject _treeTrapPrefab;
    public string PatternName => "Tree Trap";

    public TreeTrapPattern(GameObject warningTilePrefab, GameObject treeTrapPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _treeTrapPrefab = treeTrapPrefab;
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
    /// ���� ���� ����
    /// </summary>
    private IEnumerator TreeTrap(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        ////���� ����
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            Vector3 pos = boss.GridSystem.GetWorldPosition(x, playerY);
            attackPositions.Add(pos);
            warningTiles.Add(ItemObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
        }

        // ���� ����
        for (int y = 0; y < boss.GridSystem.Height; y++)
        {
            if (y != playerY)
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(0, y);
                attackPositions.Add(pos);
                warningTiles.Add(ItemObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.8f);

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        //���� ���� ������
        if (currentX == 0)
        {
            boss.ApplyDamageToPlayer(20);
        }

        //���� ���� ������
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
            ItemObject.Destroy(tile);
        }
    }
}
