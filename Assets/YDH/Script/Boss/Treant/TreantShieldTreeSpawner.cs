using System.Collections;
using UnityEngine;

public class TreantShieldTreeSpawner : MonoBehaviour
{
    public GameObject shieldTreePrefab;

    private void Start()
    {
        StartCoroutine(SpawnShieldTreeRoutine());
    }

    private IEnumerator SpawnShieldTreeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            // 이 오브젝트 위치에 ShieldTree 생성
            Vector3 spawnPos = transform.position;
            Instantiate(shieldTreePrefab, spawnPos + new Vector3(-2,0,0), Quaternion.identity);      }
    }
}
