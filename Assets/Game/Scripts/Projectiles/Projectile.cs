using UnityEngine;

/// <summary>
/// 기본 투사체 클래스
/// </summary>
public class Projectile : MonoBehaviour
{
    private int damage = 10;
    private float speed = 12f;
    private Vector3 direction;
    private Debuffs abnormalConditions = Debuffs.None; // 상태 이상
    private ProjectileTeam _team; // 투사체 소속 진영

    public enum ProjectileTeam
    {
        Player,
        Enemy
    }

    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public ProjectileTeam Team { get => _team; set => _team = value; }
    public Debuffs AbnormalConditions { get => abnormalConditions; set => abnormalConditions = value; }

    /// <summary>
    /// 투사체 초기화
    /// </summary>
    public void Initialize(Vector3 dir, ProjectileTeam projectileTeam, int givenDamage = 10)
    {
        direction = dir.normalized;
        _team = projectileTeam;
        damage = givenDamage;
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
        Debug.Log(other.name);

        // 적 진영 투사체가 아군에게 충돌
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
                if (abnormalConditions != Debuffs.None)
                {
                    enemy.AddAbnormalCondition(abnormalConditions); // 상태 이상 추가
                }
                Destroy(gameObject);
            }

            //BaseEnemy baseEnemy = other.GetComponent<BaseEnemy>();
            //if (enemy != null)
            //{
            //    baseEnemy.TakeDamage(damage);
            //    Destroy(gameObject);
            //}
        }
    }
}