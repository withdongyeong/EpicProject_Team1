﻿using UnityEngine;
using System.Collections;

public class EnemyStraightAttack : IBossAttackPattern
{
    public int gridSize = 8;
    public float cellSize = 1f;

    private GameObject _warningPrefab;
    private GameObject _tentaclePrefab;

    public string PatternName => "StraightAttack";

    public EnemyStraightAttack(GameObject warningPrefab, GameObject tentaclePrefab)
    {
        _warningPrefab = warningPrefab;
        _tentaclePrefab = tentaclePrefab;
    }
    
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteAttackPattern());
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombManager.PlayerController != null && _warningPrefab != null;
    }

    /// <summary>
    /// 촉수 찌르기 공격
    /// </summary>
    IEnumerator ExecuteAttackPattern()
    {
        // 1. 랜덤 열 번호 선택 (0 - 7)
        int Y = Random.Range(0, 8);

        // 2. 붉은색 경고 생성
        Vector3 warningPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(4, Y, 0));
        GameObject warning = Object.Instantiate(_warningPrefab, warningPos + new Vector3(-0.5f, 0, 0), Quaternion.identity);
        warning.transform.localScale = new Vector3(gridSize, 1, 1); // 세로로 길게

        // 3. 1초 대기
        yield return new WaitForSeconds(1f);

        // 4. 경고 제거
        Object.Destroy(warning);

        // 5. 촉수 생성
        Vector3 tentaclePos = warningPos + new Vector3 (4,0,0); // 경고와 같은 위치로 설정
        GameObject tentacle = Object.Instantiate(_tentaclePrefab, tentaclePos, Quaternion.identity);
        tentacle.transform.localScale = new Vector3(0.1f, 1, 1); // x축 작게 시작

        float growTime = 0.3f;
        float elapsed = 0f;

        Vector3 basePos = tentaclePos; // 기준 위치는 warningPos
        Vector3 startScale = tentacle.transform.localScale;
        Vector3 endScale = new Vector3(gridSize, 1, 1); // 왼쪽으로 X축으로 길게

        while (elapsed < growTime)
        {
            if (tentacle == null) break;

            elapsed += Time.deltaTime;
            float t = elapsed / growTime;

            float currentWidth = Mathf.Lerp(startScale.x, endScale.x, t);
            tentacle.transform.localScale = new Vector3(currentWidth, 1, 1);

            // 여의봉처럼 왼쪽으로 커지게 위치 보정
            tentacle.transform.position = basePos - new Vector3((currentWidth - startScale.x) / 2f, 0, 0);

            yield return null;
        }

        // 6. 촉수 제거
        Object.Destroy(tentacle);
        yield return 0;
    }
}
