using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DiagonalCrossPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    public string PatternName => "Diagonal Cross Attack";

    public DiagonalCrossPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteDiagonalCrossAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// �밢�� + ���� ���� ����
    /// </summary>
    private IEnumerator ExecuteDiagonalCrossAttack(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        // 1�ܰ�: �밢�� ��� ����
        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // ����-���ϴ� �밢��
        int offset = playerY - playerX;
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            int y = x + offset;
            if (boss.GridSystem.IsValidPosition(x, y))
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(x, y);
                attackPositions.Add(pos);
                warningTiles.Add(ItemObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.8f);

        // �밢�� ���� ����
        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);
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
            ItemObject.Destroy(tile);
        }

        // ���ο� �÷��̾� ��ġ ��������
        boss.GridSystem.GetXY(boss.Player.transform.position, out playerX, out playerY);

        // 2�ܰ�: ���� ��� ����
        warningTiles = new List<GameObject>();
        attackPositions = new List<Vector3>();

        yield return new WaitForSeconds(0.3f);

        // ���� ����
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            Vector3 pos = boss.GridSystem.GetWorldPosition(x, playerY);
            attackPositions.Add(pos);
            warningTiles.Add(ItemObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
        }

        yield return new WaitForSeconds(0.8f);

        // ���� ���� ����
        boss.GridSystem.GetXY(boss.Player.transform.position, out currentX, out currentY);
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
            ItemObject.Destroy(tile);
        }
    }
}