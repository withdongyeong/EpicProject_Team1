using UnityEngine;

public class DamageTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    private int _damage;

    [SerializeField] GameObject _lesserProjectile;
    [SerializeField] GameObject _betterProjectile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
    }

    public override void InitializeTotem(InventoryItemData itemData)
    {
        _damage = itemData.Damage;
    }

    public override void ActivateTotem()
    {
        FireProjectile(_lesserProjectile,_damage);

    }

    public override void ActivateTotemBetter()
    {
        FireProjectile(_betterProjectile, _damage * 2);
    }

    private void FireProjectile(GameObject projectilePrefab,int damage)
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (_targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player,damage);
        }
    }

}
