using UnityEngine;

public class IcicleSkill : ProjectileSkill
{
    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Icicle"); // Icicle 투사체 프리팹 로드
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null && targetEnemy != null && targetEnemy.gameObject.activeInHierarchy)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, 90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.BossDebuff = BossDebuff.Frostbite; // 동상 상태 이상 적용
            projectile.SetTileName(tileObject.GetTileData().TileName);
        }
    }
}  