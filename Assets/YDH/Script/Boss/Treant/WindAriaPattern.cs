using UnityEngine;
using System.Collections;

public class WindAriaPattern : IBossAttackPattern
{
    public int gridSize = 8;
    public float cellSize = 1f;

    private GameObject _warningPrefab;
    private GameObject _tentaclePrefab;

    public Transform gridOrigin; // ���� ���� ��ġ (�»�� or ���ϴ�)

    public string PatternName => "StraightAttack";

    public WindAriaPattern(GameObject warningPrefab, GameObject tentaclePrefab, Transform transform)
    {
        _warningPrefab = warningPrefab;
        _tentaclePrefab = tentaclePrefab;
        gridOrigin = transform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteAttackPattern());
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _warningPrefab != null;
    }

    /// <summary>
    /// �˼� ��� ����
    /// </summary>
    IEnumerator ExecuteAttackPattern()
    {
        // 1. ���� �� ��ȣ ���� (-4~3)
        int column = Random.Range(-4, 3);

        // 2. ������ ��� ����
        Vector3 warningPos = gridOrigin.position + new Vector3(-7, column * cellSize, 0);
        GameObject warning = ItemObject.Instantiate(_warningPrefab, warningPos, Quaternion.identity);
        warning.transform.localScale = new Vector3(gridSize, 1, 1); // ���η� ���

        // 3. 1�� ���
        yield return new WaitForSeconds(1f);

        // 4. ��� ����
        ItemObject.Destroy(warning);

        // 5. �˼� ����
        Vector3 tentaclePos = warningPos; // ����� ���� ��ġ�� ����
        GameObject tentacle = ItemObject.Instantiate(_tentaclePrefab, tentaclePos, Quaternion.identity);
        tentacle.transform.localScale = new Vector3(0.1f, 0.9f, 1); // x�� �۰� ����

        float growTime = 0.3f;
        float elapsed = 0f;

        Vector3 basePos = tentaclePos; // ���� ��ġ�� warningPos
        Vector3 startScale = tentacle.transform.localScale;
        Vector3 endScale = new Vector3(gridSize, 0.9f, 1); // �������� X������ ���

        while (elapsed < growTime)
        {
            if (tentacle == null) break;

            elapsed += Time.deltaTime;
            float t = elapsed / growTime;

            float currentWidth = Mathf.Lerp(startScale.x, endScale.x, t);
            tentacle.transform.localScale = new Vector3(currentWidth, 0.9f, 1);

            // ���Ǻ�ó�� �������� Ŀ���� ��ġ ����
            tentacle.transform.position = basePos - new Vector3((currentWidth - startScale.x) / 2f, 0, 0);

            yield return null;
        }

        // 6. �˼� ����
        ItemObject.Destroy(tentacle);
        yield return 0;
    }
}
