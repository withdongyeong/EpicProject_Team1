using UnityEngine;
using System.Collections;

public class ManaTurretSkill : ProjectileSkill
{
    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/ManaTurret"); // ManaTurret 투사체 프리팹 로드
    }

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
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
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
