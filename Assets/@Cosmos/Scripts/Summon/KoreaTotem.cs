using UnityEngine;



public class KoreaTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    [SerializeField] GameObject _lesserProjectile;
    [SerializeField] GameObject _betterProjectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void InitializeTotem(int totemPower)
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
        base.InitializeTotem(totemPower);
    }

    public override void ActivateTotem(TotemContext context)
    {
        FireProjectile(_lesserProjectile, _totemPower * (context.order + 1));

    }

    public override void ActivateTotemBetter(TotemContext context)
    {
        FireProjectile(_betterProjectile, _totemPower * 3 * (context.order + 1));
    }

    private void FireProjectile(GameObject projectilePrefab, int damage)
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }
}
