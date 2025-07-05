using UnityEngine;

public class DamageTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    [SerializeField] GameObject _lesserProjectile;
    [SerializeField] GameObject _betterProjectile;

    private void Awake()
    {
        _lesserProjectile = Resources.Load<GameObject>("Prefabs/Projectiles/Totem1");
        _betterProjectile = Resources.Load<GameObject>("Prefabs/Projectiles/Totem1 Better");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public override void InitializeTotem(int totemPower)
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
        base.InitializeTotem(totemPower);
    }

    protected override void ActivateTotem(TotemContext context)
    {
        base.ActivateTotem(context);
        FireProjectile(_lesserProjectile, _totemPower);

    }

    protected override void ActivateTotemBetter(TotemContext context)
    {
        FireProjectile(_betterProjectile, _totemPower * 2);
    }

    private void FireProjectile(GameObject projectilePrefab,int damage)
    {
        if (projectilePrefab != null && _targetEnemy != null && _targetEnemy.gameObject.activeInHierarchy)
        {
            Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player,damage);
        }
    }

}
