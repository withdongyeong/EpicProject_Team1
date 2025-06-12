using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ArachneSpiderWebPattern2 : MonoBehaviour
{
    public int _spiderWebCount = 10;
    public GameObject _spiderWebPrefab;
    public GameObject _warningPrefab;

    private PlayerController _playerController;

    void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        StartCoroutine(SpawnSpiderWebRoutine());
    }

    /// <summary>
    /// 거미집 생성
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnSpiderWebRoutine()
    {
        //처음 쿨타임
        yield return new WaitForSeconds(1f);

        while (true)
        {
            List<Vector3Int> spawnPositions = new List<Vector3Int>();
            List<GameObject> warnings = new List<GameObject>();

            for (int i = 0; i < _spiderWebCount; i++)
            {
                int X = Random.Range(0, 9);
                int Y = Random.Range(0, 9);

                bool isNearPlayer = false;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int cx = _playerController.CurrentX + x;
                        int cy = _playerController.CurrentY + y;

                        if (cx == X && cy == Y)
                        {
                            isNearPlayer = true;
                        }
                    }
                }

                Vector3Int gridPos = new Vector3Int(X, Y, 0);

                if (!isNearPlayer && GridManager.Instance.IsWithinGrid(gridPos))
                {
                    spawnPositions.Add(gridPos);

                    Vector3 worldPos = GridManager.Instance.GridToWorldPosition(gridPos);
                    GameObject warning = Instantiate(_warningPrefab, worldPos, Quaternion.identity);
                    warnings.Add(warning);
                }
            }

            // 1초 대기: 경고 표시 시간
            yield return new WaitForSeconds(1f);

            // 경고 제거 후 거미줄 생성
            foreach (GameObject warning in warnings)
            {
                Destroy(warning);
            }

            foreach (Vector3Int pos in spawnPositions)
            {
                Vector3 worldPos = GridManager.Instance.GridToWorldPosition(pos);
                Instantiate(_spiderWebPrefab, worldPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(5f);
        }
    }
}
