using UnityEngine;

public class FrostStaffSkill : ProjectileSkill
{
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 direction = (targetEnemy.transform.position - spawnPos).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            projectileObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.BossDebuff = BossDebuff.Frostbite; // 동상 상태 이상 적용
        }
    }
}
