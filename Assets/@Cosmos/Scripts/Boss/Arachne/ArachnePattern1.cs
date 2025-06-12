using UnityEngine;
using System.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        for(int i = 0; i< 8; i++)
        {
            int x = i;
            int y = 8 - i;

            GameObject[] warningTiles = new GameObject[5];
            int index = 0;

            for (int j = -2; j <= 2; j++)
            {
                int tileX = x + j;
                int tileY = y + j; // ↘ 방향: x == y

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                    warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }

            //시간
            yield return new WaitForSeconds(0.3f);
            boss.AttackAnimation();
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

            //데미지
            Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
            int currentX = GridPosition.x;
            int currentY = GridPosition.y;


            if (Mathf.Abs(currentX - x) == Mathf.Abs(currentY - y) && (currentX - x) == (currentY - y))
            {
                boss.ApplyDamageToPlayer(10);
            }

            Vector3 tilePosition = boss.GridSystem.GetWorldPosition(x, y);
            boss.CreateDamageEffect_Inversion(tilePosition, _spiderLegPrefab, 0.3f);

            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    GameObject.Destroy(tile);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator SpiderSlash2(BaseBoss boss)
    {
        for (int i = 0; i < 8; i++)
        {
            int x = 8 - i;
            int y = 8 - i;

            GameObject[] warningTiles = new GameObject[5];
            int index = 0;

            for (int j = -2; j <= 2; j++)
            {
                int tileX = x + j;
                int tileY = y - j; // ↙ 방향: x == -y

                if (GridManager.Instance.IsWithinGrid(new Vector3Int(tileX, tileY, 0)))
                {
                    Vector3 tilePos = boss.GridSystem.GetWorldPosition(tileX, tileY);
                    warningTiles[index] = GameObject.Instantiate(_warningAriaPrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }

            //시간
            yield return new WaitForSeconds(0.3f);
            boss.AttackAnimation();
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");

            //데미지
            Vector3Int GridPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
            int currentX = GridPosition.x;
            int currentY = GridPosition.y;


            if (Mathf.Abs(currentX - x) == Mathf.Abs(currentY - y) && (currentX - x) == -(currentY - y))
            {
                boss.ApplyDamageToPlayer(10);
            }

            Vector3 tilePosition = boss.GridSystem.GetWorldPosition(x, y);
            boss.CreateDamageEffect(tilePosition, _spiderLegPrefab, 0.3f);

            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    GameObject.Destroy(tile);
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
