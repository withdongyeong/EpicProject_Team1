using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ArachnePattern2 : IBossAttackPattern
{
    private GameObject _warningAriaPrefab;
    private GameObject _poisionAriaPrefeb;
    private GameObject _spiderLegPrefab;
    private PlayerController _playerController;

    public string PatternName => "ArachnePattern2";

    /// <summary>
    /// 영역 공격 패턴 생성자 - 거대화 이미지, 다리
    /// </summary>
    public ArachnePattern2(GameObject warningAriaPrefab, GameObject poisionAriaPrefeb, GameObject spiderLegPrefab, PlayerController playerController)
    {
        _warningAriaPrefab = warningAriaPrefab;
        _poisionAriaPrefeb = poisionAriaPrefeb;
        _spiderLegPrefab = spiderLegPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _spiderLegPrefab != null;
    }

    /// <summary>
    /// 단발공격 + 범위공격
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderAttack(BaseBoss boss)
    {
        for (int i = 0; i < 6; i++)
        {
            boss.StartCoroutine(PoisonAttack(boss));

            yield return new WaitForSeconds(0.25f);
        }

        boss.StartCoroutine(PoisonBigAttack(boss, true));
        yield return new WaitForSeconds(0.2f);
        boss.StartCoroutine(PoisonBigAttack(boss, false));
    }

    private IEnumerator PoisonAttack(BaseBoss boss)
    {
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);

        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        GameObject warningTiles = new GameObject();

        if (GridManager.Instance.IsWithinGrid(new Vector3Int(playerX, playerY, 0)))
        {
            Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerX, playerY, 0));
            warningTiles = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);

        boss.AttackAnimation();

        SoundManager.Instance.ArachneSoundClip("PoisionExplotionActivate");

        // 플레이어가 영역 내에 있는지 확인
        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;


        if ((currentX == playerX) && (currentY == playerY))
        {
            boss.ApplyDamageToPlayer(10);
        }

        // 공격 영역에 폭발 이펙트 생성
        Vector3 DamagePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerX, playerY, 0));
        boss.CreateDamageEffect(DamagePos, _poisionAriaPrefeb, 0.7f);

        // 경고 타일 제거
        GameObject.Destroy(warningTiles);
    }

    private IEnumerator PoisonBigAttack(BaseBoss boss, bool isHorizontal)
    {
        int splitValue = 5;

        List<Vector3> attackPositions = new List<Vector3>();
        GameObject[] warningTiles = new GameObject[9 * 9 / 2];
        int index = 0;

        Vector3 targetCenter = Vector3.zero;
        int positionCount = 0;

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                bool shouldAttack = isHorizontal ? y < splitValue : x < splitValue;

                if (shouldAttack)
                {
                    if (x % 2 == 0 && y % 2 == 0) continue;

                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(x, y, 0));

                    attackPositions.Add(tilePos);
                    warningTiles[index] = TileObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                    index++;

                    targetCenter += tilePos;
                    positionCount++;
                }
            }
        }

        targetCenter /= positionCount;

        yield return new WaitForSeconds(0.8f);

        // 플레이어 위치 가져오기
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        bool isPlayerHit = false;

        foreach (Vector3 pos in attackPositions)
        {
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(pos);

            if (gridPos == playerGridPos)
            {
                isPlayerHit = true;
                break;
            }
        }

        if (isPlayerHit)
        {
            boss.ApplyDamageToPlayer(20);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _poisionAriaPrefeb, 0.7f);
        }

        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                TileObject.Destroy(tile);
            }
        }
    }
}
