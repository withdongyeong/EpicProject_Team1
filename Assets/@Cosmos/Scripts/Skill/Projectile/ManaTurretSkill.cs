using UnityEngine;
using System.Collections;

public class ManaTurretSkill : NonActivateSkill
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
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/ManaTurret"); // ManaTurret 투사체 프리팹 로드
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
        StartCoroutine(WaitAndFire());
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

    private IEnumerator WaitAndFire()
    {
        // 무기 스킬이 먼저 나가도록 대기
        yield return new WaitForSeconds(0.1f);
        if (projectilePrefab != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, 90);
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
