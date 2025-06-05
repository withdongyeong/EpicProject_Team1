using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//화상, 기절
public enum Debuffs
{
    None,// 상태 이상 없음
    Burning,
    Frostbite
}

public class BossDebuffConditions
{
    public List<Debuffs> bossAbnormalConditions = new List<Debuffs>();

    public void AddAbnormalCondition(Debuffs abnormalConditions)
    {
        bossAbnormalConditions.Add(abnormalConditions);
    }

    public void AbnormalConditionDestruction(Debuffs abnormalConditions)
    {
        bossAbnormalConditions.RemoveAll(condition => condition == abnormalConditions);
    }
}