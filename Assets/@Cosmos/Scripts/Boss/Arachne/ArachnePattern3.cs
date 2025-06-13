using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArachnePattern3 : IBossAttackPattern
{
    private GameObject _warningAriaPrefab;
    private GameObject _poisionAriaPrefeb;
    private GameObject _spiderLegPrefab;
    private PlayerController _playerController;

    public string PatternName => "ArachnePattern3";

    /// <summary>
    /// 영역 공격 패턴 생성자 - 거대화 이미지, 다리
    /// </summary>
    public ArachnePattern3(GameObject warningAriaPrefab,GameObject poisionAriaPrefeb, GameObject spiderLegPrefab, PlayerController playerController)
    {
        _warningAriaPrefab = warningAriaPrefab;
        _poisionAriaPrefeb = poisionAriaPrefeb;
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
        boss.StartCoroutine(ExecuteAreaAttack(boss));
        yield return new WaitForSeconds(0.7f);
        boss.StartCoroutine(ExecuteAreaAttack(boss));
        yield return new WaitForSeconds(0.7f);
        boss.StartCoroutine(ExecuteAreaAttack(boss));
        yield return new WaitForSeconds(0.7f);
        boss.StartCoroutine(ExecuteAreaAttack(boss));
        yield return new WaitForSeconds(0.9f);
        boss.StartCoroutine(SpiderLeg_DiagonalSlash1(boss));
        yield return new WaitForSeconds(0.2f);
        boss.StartCoroutine(SpiderLeg_DiagonalSlash2(boss));
    }

    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");

        List<Vector2Int> directionsToExclude = new List<Vector2Int>
        {
            new Vector2Int(-1, 0), // 왼쪽
            new Vector2Int(1, 0),  // 오른쪽
            new Vector2Int(0, 1),  // 위
            new Vector2Int(0, -1), // 아래
        };

        // 플레이어 위치 가져오기
        Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);

        int playerX = GridPosition.x;
        int playerY = GridPosition.y;

        // 경고 타일 표시 (3x3 영역)
        GameObject[] warningTiles = new GameObject[9];
        int index = 0;

        Vector2Int excludeDirection = directionsToExclude[Random.Range(0, directionsToExclude.Count)];

        Vector2Int safaAria = new Vector2Int();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;

                // 현재 좌표
                if ((x == excludeDirection.x && y == excludeDirection.y))
                {
                    safaAria.x = tileX;
                    safaAria.y = tileY;
                    continue;
                }

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                    warningTiles[index] = Object.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
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
            if (!(currentX == safaAria.x && currentY == safaAria.y))
            {
                boss.ApplyDamageToPlayer(20);
            }

        }

        // 공격 영역에 폭발 이펙트 생성
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // 현재 좌표
                if ((x == excludeDirection.x && y == excludeDirection.y)) continue;

                int tileX = playerX + x;
                int tileY = playerY + y;

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                    boss.CreateDamageEffect(tilePos, _poisionAriaPrefeb, 0.7f);
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
                Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                index++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(0.3f);

        boss.AttackAnimation();
        SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;


        if (Mathf.Abs(currentX - playerX) == Mathf.Abs(currentY - playerY) && (currentX - playerX) == (currentY - playerY))
        {
            boss.ApplyDamageToPlayer(20);
        }

        Vector3 tilePosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerX, playerY, 0));
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
                Vector3 tilePos = GridManager.Instance.GridToWorldPosition(new Vector3Int(tileX, tileY, 0));
                warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                index++;

                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return new WaitForSeconds(0.3f);

        boss.AttackAnimation();
        SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

        GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        int currentX = GridPosition.x;
        int currentY = GridPosition.y;

        if (Mathf.Abs(currentX - playerX) == Mathf.Abs(currentY - playerY) && (currentX - playerX) == -(currentY - playerY))
        {
            boss.ApplyDamageToPlayer(20);
        }

        Vector3 tilePosition = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerX, playerY, 0));
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
