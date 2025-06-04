using UnityEngine;

public class ProjectileSkill : SkillBase
{
    protected int damage = 10;
    [SerializeField] protected GameObject projectilePrefab;
    protected BaseBoss targetEnemy;

    public int Damage { get => damage; set => damage = value; }

    /// <summary>
    /// 타일 발동 - 투사체 발사
    /// </summary>
    protected override void Activate(GameObject user)
    {
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        if (targetEnemy != null)
        {
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
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }
}
