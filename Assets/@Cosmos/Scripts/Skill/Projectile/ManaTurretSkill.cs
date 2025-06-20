using UnityEngine;
using System.Collections;

public class ManaTurretSkill : ProjectileSkill
{
    protected override void FireProjectile()
    {
        StartCoroutine(WaitAndFire());
    }

    private IEnumerator WaitAndFire()
    {
        // 무기 스킬이 먼저 나가도록 대기
        yield return new WaitForSeconds(0.1f);
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }

    /// <summary>
    /// 마나 터렛을 활성화합니다.(스타스킬용)
    /// </summary>
    public void ActivateManaTurret()
    {
        FireProjectile();
    }
}
