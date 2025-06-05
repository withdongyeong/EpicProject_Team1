using UnityEngine;

public class FireBoltSkill : ProjectileSkill
{
    private void Awake()
    {
        damage = 5;
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.AbnormalConditions = Debuffs.Burning; // 화염 상태 이상 적용   
        }
    }
}
