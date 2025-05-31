using UnityEngine;

public class FireBallTIle : ProjectileTile
{
    [SerializeField] private GameObject fireBallProjectilePrefab;

    private void Awake()
    {
        projectilePrefab = fireBallProjectilePrefab;
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
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player);
            projectile.AbnormalConditions = AbnormalConditions.Burning; // ȭ�� ���� �̻� ����   
        }
    }
}
