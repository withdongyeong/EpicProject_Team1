using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//ȭ��, ����
public enum AbnormalConditions
{
    None,// ���� �̻� ����
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

    public void AbnormalConditionDestruction(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.RemoveAll(condition => condition == abnormalConditions);
    }
}