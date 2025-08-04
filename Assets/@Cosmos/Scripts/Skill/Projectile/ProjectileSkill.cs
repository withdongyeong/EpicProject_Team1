using UnityEngine;

public class ProjectileSkill : SkillBase
{
    [SerializeField] protected int firstDamage;
    protected int damage = 10;
    protected GameObject projectilePrefab;
    protected BaseBoss targetEnemy;

    public int Damage { get => damage; set => damage = value; }

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(HandleGameStart);
    }

    private void HandleGameStart()
    {
        targetEnemy = FindAnyObjectByType<BaseBoss>();
    }

    /// <summary>
    /// 타일 발동 - 투사체 발사
    /// </summary>
    protected override void Activate()
    {
        base.Activate();
        if (targetEnemy != null)
        {
            FireProjectile();
        }
    }

    /// <summary>
    /// 투사체 생성 및 발사
    /// </summary>
    protected virtual void FireProjectile()
    {
        if (projectilePrefab != null && targetEnemy != null && targetEnemy.gameObject.activeInHierarchy)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 direction = (targetEnemy.transform.position - spawnPos).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.SetTileName(tileObject.GetTileData().TileName);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 구독 해제
        EventBus.UnsubscribeGameStart(HandleGameStart);
    }

    protected override void ClearStarBuff()
    {
        base.ClearStarBuff();
        damage = firstDamage;
    }

    public override void ApplyStatBuff(TileBuffData buffData)
    {
        base.ApplyStatBuff(buffData);
        if (buffData.TileStat == BuffableTileStat.Damage)
            damage += Mathf.FloorToInt(buffData.Value);
    }
}
