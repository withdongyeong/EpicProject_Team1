using UnityEngine;

public class ProjectileSkill : SkillBase
{
    [SerializeField] protected int damage = 10;
    [SerializeField] protected GameObject projectilePrefab;
    protected BaseBoss targetEnemy;

    public int Damage { get => damage; set => damage = value; }

    /// <summary>
    /// 타일 발동 - 투사체 발사
    /// </summary>
    protected override void Activate()
    {
        base.Activate();
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        if (targetEnemy != null)
        {
            Debug.Log("★★★★★★★★★★");
            FireProjectile();
        }
    }

    /// <summary>
    /// 투사체 생성 및 발사
    /// </summary>
    protected virtual void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 direction = (targetEnemy.transform.position - spawnPos).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }
}
