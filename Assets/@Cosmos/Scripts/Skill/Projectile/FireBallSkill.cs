using UnityEngine;
using System.Collections;

public class FireBallSkill : ProjectileSkill
{
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수

    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireBall"); // 화염구 프리팹 로드
    }

    protected override void Activate()
    {
        base.Activate();
        if (targetEnemy != null)
        {
            if (isEnchanted)
            {
                StartCoroutine(FireProjectileWithDelay(3));
            }
            else
            {
                StartCoroutine(FireProjectileWithDelay(2));
            }
        }
    }

    protected override void FireProjectile()
    {
        if (projectilePrefab != null && targetEnemy != null && targetEnemy.gameObject.activeInHierarchy)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, 90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
            projectile.BossDebuff = BossDebuff.Burning; // 화염 상태 이상 적용   
            projectile.SetTileName(tileObject.GetTileData().TileName);
        }
    }

    /// <summary>
    /// 발사체를 0.15초 간격으로 여러번 발사하는 코루틴입니다.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    protected IEnumerator FireProjectileWithDelay(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.15f); // 0.15초 간격으로 발사
            SoundManager.Instance.PlayTileSoundClip(GetType().Name + "Activate");
            FireProjectile();
        }
    }

    /// <summary>
    /// 타일 버프 적용 - 마법진 버프를 받으면 스킬이 강화됨
    /// </summary>
    /// <param name="buffData"></param>
    public override void ApplyStatBuff(TileBuffData buffData)
    {
        base.ApplyStatBuff(buffData);
        if (buffData.TileStat == BuffableTileStat.MagicCircle)
        {
            isEnchanted = true; // 스킬이 강화됨
        }
    }

    /// <summary>
    /// 스킬 강화 해제 - 스킬이 강화된 상태를 초기화
    /// </summary>
    protected override void ClearStarBuff()
    {
        base.ClearStarBuff();
        isEnchanted = false; // 스킬 강화 비활성화
    }
}
