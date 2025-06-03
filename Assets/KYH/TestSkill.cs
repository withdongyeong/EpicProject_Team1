using System;
using UnityEngine;

public class TestSkill : SkillBase
{
    private void Awake()
    {
        cooldown = 3;
    }

    protected override void Activate(GameObject user)
    {
        Debug.Log("TestSkill activated by " + user.name);
    }
}