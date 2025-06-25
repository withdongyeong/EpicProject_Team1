using UnityEngine;

public class IcicleSkill : ProjectileSkill
{
    protected override void Awake()
    {
        base.Awake();
        damage = 5;
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            // 보스가 멈춰있는지 확인
            if (targetEnemy != null && targetEnemy.IsStopped)
            {
                projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage * 4); // 보스가 멈춰있으면 데미지 4배 증가
            }
            else
            {
                projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            }
            projectile.BossDebuff = BossDebuff.Frostbite; // 동상 상태 이상 적용
        }
    }
}  