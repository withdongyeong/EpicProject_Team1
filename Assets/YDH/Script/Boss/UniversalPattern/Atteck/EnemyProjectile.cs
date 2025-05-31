using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage = 10;
    private float speed = 12f;
    private Vector3 direction;
    private ProjectileTeam _team; // 투사체 소속 진영

    public enum ProjectileTeam
    {
        Player,
        Enemy
    }

    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public ProjectileTeam Team { get => _team; set => _team = value; }

    /// <summary>
    /// 투사체 초기화
    /// </summary>
    public void Initialize(Vector3 dir, ProjectileTeam projectileTeam)
    {
        direction = dir.normalized;
        _team = projectileTeam;
    }
    void Update()
    {
        transform.position += Time.deltaTime * direction * speed;

        // 화면 밖으로 나가면 제거
        if (Mathf.Abs(transform.position.x) > 20 || Mathf.Abs(transform.position.y) > 20)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 충돌 처리
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // 적 진영 투사체가 적에게 충돌
        if (_team == ProjectileTeam.Enemy)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        // 아군 진영 투사체가 적에게 충돌
        else if (_team == ProjectileTeam.Player)
        {
            BaseBoss enemy = other.GetComponent<BaseBoss>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
