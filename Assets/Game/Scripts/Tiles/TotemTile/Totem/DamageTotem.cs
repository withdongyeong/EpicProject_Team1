using UnityEngine;

public class DamageTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    [SerializeField] GameObject _lesserProjectile;
    [SerializeField] GameObject _betterProjectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ActivateTotem()
    {
        FireProjectile(_lesserProjectile);

    }

    public override void ActivateTotemBetter()
    {
        FireProjectile(_betterProjectile);
    }

    private void FireProjectile(GameObject projectilePrefab)
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player);
        }
    }

}
