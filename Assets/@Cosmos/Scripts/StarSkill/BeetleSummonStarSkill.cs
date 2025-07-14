using UnityEngine;

public class BeetleSummonStarSkill : StarBase
{
    private BeetleSummonSkill skill;

    protected override void Awake()
    {
        base.Awake();
        skill = transform.parent.GetComponentInChildren<BeetleSummonSkill>();
        starBuff.RegisterGameStartAction(GatherSummons);
    }

    private void GatherSummons(SkillBase skillBase)
    {
        if(CheckCondition(skillBase))
        {
            skill.AddSummonList(skillBase.TileObject.GetTileData().TileName);
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if(skillBase.TileObject.GetTileData().TileCategory == TileCategory.Summon)
        {
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {
        starBuff.UnregisterGameStartAction(GatherSummons);
    }
}
