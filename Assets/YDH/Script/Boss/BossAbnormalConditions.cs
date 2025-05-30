using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//ȭ��, ����
public enum AbnormalConditions
{
    burn,
    Faint
}

public class BossAbnormalConditions
{
    public List<AbnormalConditions> bossAbnormalConditions = new List<AbnormalConditions>();

    /// <summary>
    /// �����̻� �߰�
    /// </summary>
    /// <param name="abnormalConditions"></param>
    public void AddAbnormalCondition(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.Add(abnormalConditions);
    }

    /// <summary>
    /// �����̻� ���� �Ҹ�
    /// </summary>
    /// <param name="abnormalConditions"></param>
    public void AbnormalConditionDestruction(AbnormalConditions abnormalConditions)
    {
        bossAbnormalConditions.RemoveAll(cond => cond == abnormalConditions);
    }
}