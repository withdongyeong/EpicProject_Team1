using UnityEngine;

public class WraithSkill : SkillBase
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
            Debug.LogError("WraithSkill: Target enemy (BaseBoss) not found in the scene.");
        }
        if (bossDebuffs.MaxCurseCount < 40)
        {
            bossDebuffs.MaxCurseCount = 40; // 저주 상태 이상 최대치 설정
        }
    }

    protected override void Activate()
    {
        base.Activate();
        if (isEnchanted)
        {
            AddPain(20); // 강화된 상태면 저주를 20개 소모하고 영구 피해 증폭 10% 적용
        }
        else
        {
            AddPain(40); // 기본적으로 저주를 40개 소모하고 영구 피해 증폭 10% 적용
        }
        bossDebuffs.TurnEveryDebuffsToCurse(); // 모든 상태 이상을 저주로 변환
    }

    private void AddPain(int curseAmount)
    {
        // 저주를 curseAmount개 소모하고 적에게 영구 피해 증폭 10% 부여
        if (targetEnemy != null)
        {
            int curseCount = targetEnemy.GetDebuffCount(BossDebuff.Curse);
            if (curseCount >= curseAmount)
            {
                for (int i = 0; i < curseAmount; i++)
                {
                    targetEnemy.RemoveDebuff(BossDebuff.Curse); // 저주를 20개 소모
                }
                targetEnemy.AddDebuff(BossDebuff.Pain); // 영구 피해 증폭 10% 적용
            }
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(OnGameStart);
    }
}
