using UnityEngine;

public class RainbowSkill : SkillBase
{
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수

    protected override void Activate()
    {
        base.Activate();
        BaseBoss boss = FindAnyObjectByType<BaseBoss>();
        boss.AddDebuff(BossDebuff.Burning);
        boss.AddDebuff(BossDebuff.Frostbite);
        boss.AddDebuff(BossDebuff.Curse);
        if (isEnchanted) //강화되면 두번
        {
            boss.AddDebuff(BossDebuff.Burning);
            boss.AddDebuff(BossDebuff.Curse);
        }

    }

    public override void ApplyStatBuff(TileBuffData buffData)
    {
        base.ApplyStatBuff(buffData);
        if (buffData.TileStat == BuffableTileStat.MagicCircle)
        {
            isEnchanted = true; // 스킬이 강화됨
        }

    }
}
