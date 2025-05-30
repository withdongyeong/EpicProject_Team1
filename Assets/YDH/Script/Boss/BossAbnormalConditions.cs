using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//화상, 기절
public enum AbnormalConditions
{
    None,// 상태 이상 없음
    Burning,
    Faint
}

public class BossAbnormalConditions
{
    public List<AbnormalConditions> bossAbnormalConditions = new List<AbnormalConditions>();

    public void AddAbnormalCondition(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.Add(abnormalConditions);
    }
}