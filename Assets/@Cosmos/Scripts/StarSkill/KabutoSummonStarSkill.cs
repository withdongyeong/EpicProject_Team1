using UnityEngine;

public class KabutoSummonStarSkill : StarBase
{
    private KabutoSummonSkill skill;

    protected override void Awake()
    {
        base.Awake();
        skill = transform.parent.GetComponentInChildren<KabutoSummonSkill>();
        starBuff.RegisterGameStartAction(GatherSummons);
    }

    private void GatherSummons(SkillBase skillBase)
    {
        Debug.Log("1");
        if (CheckCondition(skillBase))
        {
            skill.AddSummonList(skillBase.TileObject.GetTileData().TileName);
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if (skillBase.TileObject.GetTileData().TileCategory == TileCategory.Summon)
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
