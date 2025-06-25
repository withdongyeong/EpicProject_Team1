using UnityEngine.SceneManagement;
using UnityEngine;

public class ArchmageStaffStarSkill : StarBase
{
    private ArchmageStaffSkill skill;

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterGameStartAction(GatherAxodia);

        // ProjectileSkill 컴포넌트를 가져옵니다.
        skill = transform.parent.GetComponentInChildren<ArchmageStaffSkill>();
    }

    /// <summary>
    /// 전투 시작 시 Axodia를 모으는 함수입니다.
    /// </summary>
    /// <param name="skillBase"></param>
    private void GatherAxodia(SkillBase skillBase)
    {
        if (skillBase.TileObject.name.Contains("Goddess") || skillBase.TileObject.name.Contains("Clock"))
        {
            skill.AddAdjacentTile(skillBase.TileObject.name);
        }
    }

    private void OnDestroy()
    {
        starBuff.UnregisterGameStartAction(GatherAxodia);
    }
}
