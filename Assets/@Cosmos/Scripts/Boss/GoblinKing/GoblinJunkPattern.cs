using UnityEngine;
using System.Collections;

public class GoblinJunkPattern : IBossAttackPattern
{
    private GameObject _junkPrefab;
    private int _objectCount;
    private float _shotInterval;
    private Transform _transform;
    
    // 투사체 힘 범위 설정
    private float minForce = 10f;
    private float maxForce = 16f;

    private float minAngle = 100f; // 발사 각도 범위 (기준 방향: 오른쪽)
    private float maxAngle = 160f;

    public string PatternName => "GoblinJunkPattern";

    public GoblinJunkPattern(GameObject junkPrefab, int objectCount, float shotInterval, Transform transform)
    {
        _junkPrefab = junkPrefab;
        _objectCount = objectCount;
        _shotInterval = shotInterval;
        _transform = transform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(GoblinJunkAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _junkPrefab != null && _transform != null;
    }

    /// <summary>
    /// 고블린 쓰레기 투척 공격
    /// </summary>
    private IEnumerator GoblinJunkAttack(BaseBoss boss)
    {
        for(int i = 0; i < _objectCount; i++)
        {
            // 1. 생성
            GameObject junk = Object.Instantiate(_junkPrefab, _transform.position, Quaternion.identity);

            // 2. Rigidbody2D 확인
            Rigidbody2D rb = junk.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 3. 랜덤 각도와 힘 설정
                float angle = Random.Range(minAngle, maxAngle);
                float force = Random.Range(minForce, maxForce);

                // 4. 발사 방향 계산
                Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.right;

                // 5. 물리 적용
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(_shotInterval);
        }
    }
}