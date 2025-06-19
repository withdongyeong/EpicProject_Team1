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

    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        FindAnyObjectByType<BaseBoss>().TakeDamage(10);
        Debug.Log("TestSkill activated by " + user.name);
    }
}