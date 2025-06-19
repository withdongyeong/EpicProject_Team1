using UnityEngine;
using System.Collections;

public class GoblinJunkPattern : IBossAttackPattern
{
    private GameObject _junkPrefab;
    private int _objectCount;
    private float _shotInterval;
    private Transform _transform;
    // ����� ���� ���� ����
    private float minForce = 10f;
    private float maxForce = 16f;

    private float minAngle = 100f; // ���� �� ���� (���� ����: ��)
    private float maxAngle = 160f;


    public string PatternName => "GoblrinJunkPattern";

    public GoblinJunkPattern(GameObject junkPrefab, int objectCount, float shotInterval, Transform transform)
    {
        _junkPrefab = junkPrefab;
        _objectCount = objectCount;
        _shotInterval = shotInterval;
        _transform = transform;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(GoblrinJunkAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.BombHandler.PlayerController != null && _junkPrefab != null;
    }

    /// <summary>
    /// �⵿��� ������ ����
    /// </summary>
    private IEnumerator GoblrinJunkAttack(BaseBoss boss)
    {
        for(int i  =0; i < _objectCount; i++)
        {
            // 1. ����
            GameObject junk = TileObject.Instantiate(_junkPrefab, _transform.position, Quaternion.identity);

            // 2. Rigidbody2D ���
            Rigidbody2D rb = junk.GetComponent<Rigidbody2D>();

            // 3. ���� ������ �� ���
            float angle = Random.Range(minAngle, maxAngle);
            float force = Random.Range(minForce, maxForce);

            // 4. ���� ���� ���
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // 5. ���� ����
            rb.AddForce(direction * force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(_shotInterval);
        }
        yield return 0;
    }

}
