using UnityEngine;

public class HauntedDollSkill : SkillBase
{
    private BaseBoss targetEnemy;
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(OnGameStart);
    }

    private void OnGameStart()
    {
        targetEnemy = FindAnyObjectByType<BaseBoss>();
    }

    protected override void Activate()
    {
        base.Activate();
        if (targetEnemy != null)
        {
            // 저주 개수에 따라 추가 효과를 주는 로직
            if (targetEnemy.GetDebuffCount(BossDebuff.Curse) >= 20)
            {
                for (int i = 0; i < 2; i++)
                {
                    targetEnemy.AddDebuff(BossDebuff.Frostbite); // 동상을 2번 부여
                }
            }
            if (targetEnemy.GetDebuffCount(BossDebuff.Curse) >= 30)
            {
                for (int i = 0; i < 5; i++)
                {
                    targetEnemy.AddDebuff(BossDebuff.Burning); // 화염을 5번 부여
                }
            }
            if (targetEnemy.GetDebuffCount(BossDebuff.Curse) >= 40)
            {
                targetEnemy.AddDebuff(BossDebuff.Mark); // 낙인을 1번 부여
            }

            if (isEnchanted)
            {
                targetEnemy.AddDebuff(BossDebuff.Curse); // 강화된 상태면 저주를 2번 부여
                targetEnemy.AddDebuff(BossDebuff.Curse);
            }
            else
            {
                targetEnemy.AddDebuff(BossDebuff.Curse);
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