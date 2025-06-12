using UnityEngine;
using System.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Collections.Generic;

public class ArachnePattern1 : IBossAttackPattern
{
    private GameObject _warningAriaPrefab;
    private GameObject _poisionAriaPrefeb;
    private GameObject _spiderLegPrefab;
    private PlayerController _playerController;

    public string PatternName => "ArachnePattern1";

    /// <summary>
    /// 영역 공격 패턴 생성자 - 거대화 이미지, 다리
    /// </summary>
    public ArachnePattern1(GameObject warningAriaPrefab, GameObject poisionAriaPrefeb, GameObject spiderLegPrefab, PlayerController playerController)
    {
        _warningAriaPrefab = warningAriaPrefab;
        _poisionAriaPrefeb = poisionAriaPrefeb;
        _spiderLegPrefab = spiderLegPrefab;
        _playerController = playerController;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderSlash1(boss));
        boss.StartCoroutine(SpiderSlash2(boss));
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
    private IEnumerator SpiderSlash1(BaseBoss boss)
    {
        for (int i = 0; i < 9; i++)
        {
            int x = i;
            int y = 8 - i;

            List<Vector3Int> attackTiles = new List<Vector3Int>();
            List<GameObject> warningTiles = new List<GameObject>();

            for (int j = -2; j <= 2; j++)
            {
                int tileX = x + j;
                int tileY = y + j; // ↘ 방향: x == y

                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);

                if (GridManager.Instance.IsWithinGrid(tilePos))
                {
                    Vector3 worldPos = GridManager.Instance.GridToWorldPosition(tilePos);
                    warningTiles.Add(GameObject.Instantiate(_warningAriaPrefab, worldPos, Quaternion.identity));
                    attackTiles.Add(tilePos);
                }
            }

            yield return new WaitForSeconds(0.2f);
            boss.AttackAnimation();
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

            Vector3Int playerPos = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);

            foreach (Vector3Int attackTile in attackTiles)
            {
                if (attackTile == playerPos)
                {
                    boss.ApplyDamageToPlayer(1);
                    break;
                }
            }

            // 중심 타일 기준 이펙트 생성
            Vector3 effectPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(x, y, 0));
            boss.CreateDamageEffect_Inversion(effectPos, _spiderLegPrefab, 0.3f);

            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    GameObject.Destroy(tile);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator SpiderSlash2(BaseBoss boss)
    {
        for (int i = 0; i < 9; i++)
        {
            int x = 8 - i;
            int y = 8 - i;

            List<Vector3Int> attackTiles = new List<Vector3Int>();
            List<GameObject> warningTiles = new List<GameObject>();

            for (int j = -2; j <= 2; j++)
            {
                int tileX = x + j;
                int tileY = y - j; // ↙ 방향: x == -y

                Vector3Int attackPos = new Vector3Int(tileX, tileY, 0);

                if (GridManager.Instance.IsWithinGrid(attackPos))
                {
                    Vector3 tilePos = GridManager.Instance.GridToWorldPosition(attackPos);
                    warningTiles.Add(GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity));
                    attackTiles.Add(attackPos);
                }
            }

            yield return new WaitForSeconds(0.2f);

            boss.AttackAnimation();
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

            Vector3Int playerPos = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);

            foreach (Vector3Int attackTile in attackTiles)
            {
                if (attackTile == playerPos)
                {
                    boss.ApplyDamageToPlayer(1);
                    break;
                }
            }

            Vector3 effectPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(x, y, 0));
            boss.CreateDamageEffect(effectPos, _spiderLegPrefab, 0.3f);

            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    GameObject.Destroy(tile);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
