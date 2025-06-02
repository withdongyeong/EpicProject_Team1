using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindAriaPattern : IBossAttackPattern
{
    public int gridSize = 8;
    public float cellSize = 1f;

    private GameObject _warningPrefab;
    private GameObject _teantWindMagicPrefab;

    public Transform gridOrigin; // 격자 시작 위치 (좌상단 or 좌하단)

    public string PatternName => "StraightAttack";

    public WindAriaPattern(GameObject warningPrefab, GameObject teantWindMagicPrefab, Transform transform)
    {
        _warningPrefab = warningPrefab;
        _teantWindMagicPrefab = teantWindMagicPrefab;
        gridOrigin = transform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(WindMagicPattern());
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _warningPrefab != null;
    }

    /// <summary>
    /// 바람 마법 패턴
    /// </summary>
    IEnumerator WindMagicPattern()
    {
        // 1. 랜덤 열 번호 선택 (-4~3)
        int column = Random.Range(-4, 3);

        // 2. 붉은색 경고 생성
        Vector3 warningPos = gridOrigin.position + new Vector3(-7, column * cellSize, 0);
        GameObject warning = Object.Instantiate(_warningPrefab, warningPos, Quaternion.identity);
        warning.transform.localScale = new Vector3(gridSize, 1, 1); // 세로로 길게

        // 3. 1초 대기
        yield return new WaitForSeconds(1f);

        // 4. 경고 제거
        Object.Destroy(warning);

        // 5. 나무 생성
        Vector3 WindMagicPos = warningPos; // 경고와 같은 위치로 설정
        GameObject teantWindMagic = Object.Instantiate(_teantWindMagicPrefab, WindMagicPos, Quaternion.identity);
        teantWindMagic.transform.localScale = new Vector3(0.1f, 0.9f, 1); // x축 작게 시작

        float growTime = 0.3f;
        float elapsed = 0f;

        Vector3 basePos = WindMagicPos; // 기준 위치는 warningPos
        Vector3 startScale = teantWindMagic.transform.localScale;
        Vector3 endScale = new Vector3(gridSize, 0.9f, 1); // 왼쪽으로 X축으로 길게

        while (elapsed < growTime)
        {
            if (teantWindMagic == null) break;

            elapsed += Time.deltaTime;
            float t = elapsed / growTime;

            float currentWidth = Mathf.Lerp(startScale.x, endScale.x, t);
            teantWindMagic.transform.localScale = new Vector3(currentWidth, 0.9f, 1);

            teantWindMagic.transform.position = basePos - new Vector3((currentWidth - startScale.x) / 2f, 0, 0);

            yield return null;
        }

        // 6. 나무
        Object.Destroy(teantWindMagic);
        yield return 0;
    }

    public IEnumerator SAR(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // 플레이어의 위치에 맞춰서 직선 
        int y = playerY;

        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            if (boss.GridSystem.IsValidPosition(x, y))
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(x, y);
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningPrefab, pos, Quaternion.identity));
            }
        }
        yield return new WaitForSeconds(0.8f);

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        // 플레이어가 해당 직선에 있으면 맨 뒤로 - 여기 구현해야함
        bool isOnDiagonal1 = currentY == y;

        if (isOnDiagonal1)
        {
            currentX = 0;
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }
}
