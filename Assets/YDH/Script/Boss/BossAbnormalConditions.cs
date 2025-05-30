using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//화상, 기절
public enum AbnormalConditions
{
    burn,
    Faint
}

public class BossAbnormalConditions
{
    public List<AbnormalConditions> bossAbnormalConditions = new List<AbnormalConditions>();

    /// <summary>
    /// 상태이상 추가
    /// </summary>
    /// <param name="abnormalConditions"></param>
    public void AddAbnormalCondition(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.Add(abnormalConditions);
    }

    /// <summary>
    /// 상태이상 전부 소멸
    /// </summary>
    /// <param name="abnormalConditions"></param>
    public void AbnormalConditionDestruction(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.RemoveAll(cond => cond == abnormalConditions);
    }
}