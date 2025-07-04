using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EnhancementStarSkill : StarBase
{
    /// <summary>
    /// 버프를 넣을 카테고리입니다.
    /// </summary>
    [SerializeField] protected TileCategory tileCategory;
    /// <summary>
    /// 버프를 넣을 스탯입니다.
    /// </summary>
    [SerializeField] protected BuffableTileStat tileStat;
    /// <summary>
    /// 버프를 넣을 양입니다.
    /// </summary>
    [SerializeField] protected float amount;
    private List<SkillBase> skillBaseList = new List<SkillBase>();

    protected TileBuffData initBuff;

    protected override void Awake()
    {
        base.Awake();
        initBuff = new(tileStat, amount);
        skillBaseList.AddRange(transform.parent.GetComponentsInChildren<SkillBase>());
        //게임 시작시 버프 넣어주는걸 시도합니다.
        starBuff.RegisterGameStartAction(TryApplyBuff);
    }

    /// <summary>
    /// 게임 시작시 해당 스킬에 버프를 넣어줍니다.
    /// </summary>
    /// <param name="skillBase"></param>
    protected void TryApplyBuff(SkillBase skillBase)
    {
        //만약 대상 카테고리가 None이면 모두에게 버프를 넣습니다.
        if (CheckCondition(skillBase))
        {
            foreach (SkillBase skill in skillBaseList)
            {
                skill.ApplyStatBuff(initBuff);
            }
        }
    }

    public override bool CheckCondition(SkillBase skillBase)
    {
        if (tileCategory == TileCategory.None || skillBase.TileObject.GetTileData().TileCategory == tileCategory)
        {
            return true;
        }
        return false;
    }
}
