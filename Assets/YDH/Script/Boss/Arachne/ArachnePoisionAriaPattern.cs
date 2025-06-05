using UnityEngine;
using System.Collections;
public class ArachnePoisionAriaPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    public string PatternName => "ArachnePoisionAria";

    /// <summary>
    /// ���� ���� ���� ������
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
    /// �� ����
    /// </summary>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        // �÷��̾� ��ġ ��������
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        // ��� Ÿ�� ǥ�� (3x3 ����)
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

        // Ÿ�� ���� �߾� ��ġ ���
        Vector3 targetCenter = boss.GridSystem.GetWorldPosition(playerX, playerY);

        // ��� ���
        yield return new WaitForSeconds(0.5f);

        // �÷��̾ ���� ���� �ִ��� Ȯ��
        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);
        if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
        {
            boss.ApplyDamageToPlayer(15);
        }

        // ���� ������ ���� ����Ʈ ����
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

        // ��� Ÿ�� ����
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                ItemObject.Destroy(tile);
            }
        }
    }
}
