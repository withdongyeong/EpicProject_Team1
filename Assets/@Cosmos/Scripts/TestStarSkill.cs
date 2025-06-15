using UnityEngine;

public class TestStarSkill : StarBase
{
    public override void Activate()
    {
        base.Activate();
        if (tileInfo.TileCategory == TileCategory.Weapon)
        {
            skillUseManager.SetSkillActivationCount(5);
            skillUseManager.MultiplyCooldown(0.99f); // 50% 감소
        }
    }
}
