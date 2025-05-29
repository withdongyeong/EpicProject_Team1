using UnityEngine;
using System.Collections;

public class GoblinJunkPattern : IBossAttackPattern
{
    private GameObject _junkPrefab;
    private int _objectCount;
    private float _shotInterval;
    private Transform _transform;
    // 세기와 각도 범위 설정
    private float minForce = 10f;
    private float maxForce = 16f;

    private float minAngle = 100f; // 왼쪽 위 방향 (각도 단위: 도)
    private float maxAngle = 160f;


    public string PatternName => "GoblrinJunkPattern";

    public GoblinJunkPattern(GameObject junkPrefab, int objectCount, float shotInterval, Transform transform)
    {
        _junkPrefab = junkPrefab;
        _objectCount = objectCount;
        _shotInterval = shotInterval;
        _transform = transform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(GoblrinJunkAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _junkPrefab != null;
    }

    /// <summary>
    /// 잡동사니 던지기 공격
    /// </summary>
    private IEnumerator GoblrinJunkAttack(BaseBoss boss)
    {
        for(int i  =0; i < _objectCount; i++)
        {
            // 1. 생성
            GameObject junk = Object.Instantiate(_junkPrefab, _transform.position, Quaternion.identity);

            // 2. Rigidbody2D 얻기
            Rigidbody2D rb = junk.GetComponent<Rigidbody2D>();

            // 3. 랜덤 각도와 힘 계산
            float angle = Random.Range(minAngle, maxAngle);
            float force = Random.Range(minForce, maxForce);

            // 4. 방향 벡터 계산
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

            // 5. 힘을 가함
            rb.AddForce(direction * force, ForceMode2D.Impulse);

            yield return new WaitForSeconds(_shotInterval);
        }
        yield return 0;
    }

}
