using UnityEngine;

public class HellFireSkill : SkillBase
{
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수
    private BaseBoss targetEnemy; // 보스 인스턴스 참조
    private BossDebuffs bossDebuffs; // 보스의 상태 이상 관리 클래스 참조

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(OnGameStart);
    }

    private void OnGameStart()
    {
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        bossDebuffs = targetEnemy.GetComponent<BossDebuffs>();
        if (targetEnemy == null)
        {
            Debug.LogError("HellFireSkill: Target enemy (BaseBoss) not found in the scene.");
        }
    }

    protected override void Activate()
    {
        base.Activate();
        int burnStacks = isEnchanted ? 10 : 5;
    
        bool hasMaxDamage = false; // 최대 데미지 발동 여부
    
        for (int i = 0; i < burnStacks; i++)
        {
            int burnDamage = bossDebuffs.ApplyBurningEffect();
            Debug.LogWarning(burnDamage);
            // 최대 데미지(50) 발동 확인
            if (burnDamage >= 50)
            {
                hasMaxDamage = true;
            }
        }
    
        // 도전과제: 지옥불로 최대 데미지 화상 발동
        if (hasMaxDamage)
        {
            SteamAchievement.Achieve("ACH_HELLFIRE_MAX_BURN"); // 최대 화상 데미지 발동
            Debug.Log("도전과제 달성: 지옥불 최대 데미지 발동!");
        }
    
        bossDebuffs.RemoveAllDebuff(BossDebuff.Burning);
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(OnGameStart);
    }
}
