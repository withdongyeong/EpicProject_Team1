using UnityEngine;
using System.Collections;

public class WindAriaPattern : IBossAttackPattern
{
    public int gridSize = 8;
    public float cellSize = 1f;

    private GameObject _warningPrefab;
    private GameObject _teantWindMagicPrefab;

    public string PatternName => "StraightAttack";

    public WindAriaPattern(GameObject warningPrefab, GameObject teantWindMagicPrefab)
    {
        _warningPrefab = warningPrefab;
        _teantWindMagicPrefab = teantWindMagicPrefab;
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
        // 1. 랜덤 열 번호 선택 (0 - 7)
        int Y = Random.Range(0, 8);

        // 2. 경고 생성
        Vector3 warningPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(4, Y, 0));
        GameObject warning = Object.Instantiate(_warningPrefab, warningPos + new Vector3(-0.5f, 0, 0), Quaternion.identity);
        warning.transform.localScale = new Vector3(gridSize, 1, 1); // 세로로 길게

        // 3. 1초 대기
        yield return new WaitForSeconds(1f);

        // 4. 경고 제거
        Object.Destroy(warning);

        // 5. 바람 생성
        Vector3 WindMagicPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(7, Y, 0)); ; // 경고와 같은 위치로 설정
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
}
