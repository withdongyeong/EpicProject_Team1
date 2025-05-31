using UnityEngine;

public class FireBallTile : ProjectileTile
{
    private void Awake()
    {
        _damage = 5;
        _chargeTime = 3f;
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.AbnormalConditions = AbnormalConditions.Burning; // 화염 상태 이상 적용   
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player);
        }
    }
}
