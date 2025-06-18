using UnityEngine;

public class ManaAIStarSkill : StarBase
{
    [SerializeField] private SkillBase skill;

    public override void Activate(TileObject tileObject)
    {
        base.Activate(tileObject);
        skill.TryActivate();
    }
}
