using UnityEngine;

public class PhantomSkill : SkillBase
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
            Debug.LogError("PhantomSkill: Target enemy (BaseBoss) not found in the scene.");
        }
        if (bossDebuffs.MaxCurseCount < 45)
        {
            bossDebuffs.MaxCurseCount = 45; // 저주 상태 이상 최대치 설정
        }
    }

    protected override void Activate()
    {
        base.Activate();
        LoseAllCurse(); // 모든 저주 상태 이상을 제거
    }

    /// <summary>
    /// 모든 저주 상태 이상 제거 - 보스의 저주 상태 이상을 모두 제거합니다.
    /// </summary>
    private void LoseAllCurse()
    {
        bossDebuffs.RemoveAllDebuff(BossDebuff.Curse); // 보스의 모든 저주 상태 이상 제거
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
            if (bossDebuffs.MaxCurseCount < 60)
            {
                bossDebuffs.MaxCurseCount = 60; // 저주 상태 이상 최대치 설정
            }
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
