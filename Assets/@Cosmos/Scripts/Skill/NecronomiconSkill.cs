using UnityEngine;

public class NecronomiconSkill : ProjectileSkill
{
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Activate()
    {
        int currentDamage = damage;
        if (targetEnemy != null)
        {
            if (isEnchanted)
            {
                damage += targetEnemy.GetDebuffCount(BossDebuff.Curse) * 2; // 강화된 상태면 저주 개수에 따라 추가 피해
            }
            else
            {
                damage += targetEnemy.GetDebuffCount(BossDebuff.Curse); // 기본적으로 저주 개수만큼 피해
            }
        }
        base.Activate();
        damage = currentDamage; // 원래 피해량으로 되돌림
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