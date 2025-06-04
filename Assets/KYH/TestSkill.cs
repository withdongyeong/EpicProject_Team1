using System;
using UnityEngine;

public class TestSkill : SkillBase
{
    private void Awake()
    {
        cooldown = 5;
    }

    protected override void Activate(GameObject user)
    {
        FindAnyObjectByType<BaseBoss>().TakeDamage(10);
        Debug.Log("TestSkill activated by " + user.name);
    }
}