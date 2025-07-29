using UnityEngine;

public class DamageTotem : BaseTotem
{
    private BaseBoss _targetEnemy;

    //GameObject _lesserProjectile;
    GameObject _betterProjectile;

    private void Awake()
    {
        //_lesserProjectile = Resources.Load<GameObject>("Prefabs/Projectiles/Totem1");
        _betterProjectile = Resources.Load<GameObject>("Prefabs/Projectiles/Totem1 Better");
    }


    public override void InitializeTotem(int totemPower)
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
        base.InitializeTotem(totemPower);
    }

    protected override void ActivateTotem(TotemContext context)
    {
        base.ActivateTotem(context);
        _targetEnemy.AddDebuff(BossDebuff.Curse);
        _targetEnemy.AddDebuff(BossDebuff.Curse);
        //FireProjectile(_lesserProjectile, _totemPower);

    }

    protected override void ActivateTotemBetter(TotemContext context)
    {
        _targetEnemy.AddDebuff(BossDebuff.Curse);
        _targetEnemy.AddDebuff(BossDebuff.Curse);
        FireProjectile(_betterProjectile, _targetEnemy.GetDebuffCount(BossDebuff.Curse));
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
