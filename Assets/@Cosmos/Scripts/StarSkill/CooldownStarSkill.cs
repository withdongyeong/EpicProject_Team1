using UnityEngine;

public class CooldownStarSkill : StarBase
{
    public float cooldownFactor = 0.1f; // 10% 감소
    public TileCategory targetCategory = TileCategory.Weapon; // 적용할 타일 카테고리

    public override void Activate()
    {
        base.Activate();
        if (tileInfo.TileCategory == targetCategory)
        {
            skillUseManager.MultiplyCooldown(cooldownFactor);
        }
    }
}
