using System;
using UnityEngine;

public class TestSkill : SkillBase
{
    protected override void  Awake()
    {
        base.Awake();
        cooldown = 5;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("시작함");
    }

    protected override void Activate()
    {
        base.Activate();
        FindAnyObjectByType<BaseBoss>().TakeDamage(10);
    }
}