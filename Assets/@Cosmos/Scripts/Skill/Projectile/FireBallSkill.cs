using UnityEngine;
using System.Collections;

public class FireBallSkill : FireBoltSkill
{
    private void Awake()
    {
        damage = 5;
    }

    /// <summary>
    /// 세 번의 발사체를 0.3초 간격으로 발사합니다.
    /// </summary>
    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        if (targetEnemy != null)
        {
            StartCoroutine(FireProjectileWithDelay());
        }
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.BossDebuff = BossDebuff.Burning; // 화염 상태 이상 적용   
        }
    }

    private IEnumerator FireProjectileWithDelay()
    {
        yield return new WaitForSeconds(0.15f); // 0.15초 딜레이 후 발사

        SoundManager.Instance.PlayTileSoundClip(GetType().Name + "Activate");
        FireProjectile();

        yield return new WaitForSeconds(0.15f); // 0.15초 딜레이 후 발사

        SoundManager.Instance.PlayTileSoundClip(GetType().Name + "Activate");
        FireProjectile();
    }
}
