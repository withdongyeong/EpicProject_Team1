using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ArachneSpiderLegPattern : IBossAttackPattern
{
    private GameObject _warningAriaPrefab;
    private GameObject _spiderLegPrefab;
    private PlayerController _playerController;

    public string PatternName => "ArachneSpiderLeg";

    /// <summary>
    /// 영역 공격 패턴 생성자 - 거대화 이미지, 다리
    /// </summary>
    public ArachneSpiderLegPattern(GameObject warningAriaPrefab, GameObject spiderLegPrefab, PlayerController playerController)
    {
        _warningAriaPrefab = warningAriaPrefab;
        _spiderLegPrefab = spiderLegPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderLeg(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _spiderLegPrefab != null;
    }

    /// <summary>
    /// 다리 패턴
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderLeg(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderLeg_DiagonalSlash1(boss));
        yield return new WaitForSeconds(0.3f);
        boss.StartCoroutine(SpiderLeg_DiagonalSlash2(boss));
    }

    private IEnumerator SpiderLeg_DiagonalSlash1(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        GameObject[] warningTiles = new GameObject[5];
        int index = 0;

        for (int i = -2; i <= 2; i++)
        {
            int tileX = playerX + i;
            int tileY = playerY + i; // ↘ 방향: x == y

            if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
            {
                Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                index++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(1f);

        boss.AttackAnimation();
        SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;


        if (Mathf.Abs(currentX - playerX) == Mathf.Abs(currentY - playerY) && (currentX - playerX) == (currentY - playerY))
        {
            boss.ApplyDamageToPlayer(10);
        }

        Vector3 tilePosition = boss.GridSystem.GetWorldPosition(playerX, playerY);
        boss.CreateDamageEffect_Inversion(tilePosition, _spiderLegPrefab, 0.3f);

        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                GameObject.Destroy(tile);
            }
        }
    }

    private IEnumerator SpiderLeg_DiagonalSlash2(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        GameObject[] warningTiles = new GameObject[5];
        int index = 0;

        for (int i = -2; i <= 2; i++)
        {
            int tileX = playerX + i;
            int tileY = playerY - i; // ↙ 방향: x == -y

            if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
            {
                Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                index++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        boss.AttackAnimation();
        SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        if (Mathf.Abs(currentX - playerX) == Mathf.Abs(currentY - playerY) && (currentX - playerX) == -(currentY - playerY))
        {
            boss.ApplyDamageToPlayer(10);
        }

        Vector3 tilePosition = boss.GridSystem.GetWorldPosition(playerX, playerY);
        boss.CreateDamageEffect(tilePosition, _spiderLegPrefab, 0.3f);

        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                GameObject.Destroy(tile);
            }
        }

    }
}


