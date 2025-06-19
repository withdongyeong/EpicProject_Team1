using UnityEngine;

public class FireBoltSkill : ProjectileSkill
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
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, new Quaternion (0,0,180,0));
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.BossDebuff = BossDebuff.Burning; // 화염 상태 이상 적용   
        }
    }
}
