using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private int damage = 10;
    private float speed = 12f;
    private Vector3 direction;
    private ProjectileTeam _team; // ����ü �Ҽ� ����

    public enum ProjectileTeam
    {
        Player,
        Enemy
    }

    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public ProjectileTeam Team { get => _team; set => _team = value; }

    /// <summary>
    /// ����ü �ʱ�ȭ
    /// </summary>
    public void Initialize(Vector3 dir, ProjectileTeam projectileTeam)
    {
        direction = dir.normalized;
        _team = projectileTeam;
    }
    void Update()
    {
        transform.position += Time.deltaTime * direction * speed;

        // ȭ�� ������ ������ ����
        if (Mathf.Abs(transform.position.x) > 20 || Mathf.Abs(transform.position.y) > 20)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �浹 ó��
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // �� ���� ����ü�� ������ �浹
        if (_team == ProjectileTeam.Enemy)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        // �Ʊ� ���� ����ü�� ������ �浹
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
