using UnityEngine;



public class KoreaTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    private int _damage;

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

    public override void InitializeTotem(InventoryItemData itemData)
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
        _damage = itemData.Damage;
        base.InitializeTotem(itemData);
    }

    public override void ActivateTotem(TotemContext context)
    {
        FireProjectile(_lesserProjectile, _damage * (context.order + 1));

    }

    public override void ActivateTotemBetter(TotemContext context)
    {
        FireProjectile(_betterProjectile, _damage * 2 * (context.order + 1));
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
