using UnityEngine;

public class CooldownStarSkill : StarBase
{
    [SerializeField] protected TileCategory tileCategory;
    [SerializeField] protected BuffableTileStat tileStat;
    [SerializeField] protected float amount;

    TileBuffData initBuff;

    protected override void Awake()
    {
        base.Awake();
        initBuff = new(tileStat,amount);
        starBuff.RegisterGameStartAction(ApplyCooldown);
    }

    protected void ApplyCooldown(SkillBase skillBase)
    {
        if(skillBase.TileObject.GetTileData().TileCategory == tileCategory)
        {
            skillBase.ApplyStatBuff(initBuff);
        }
    }
}
