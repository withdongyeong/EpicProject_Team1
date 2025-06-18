using System;
using UnityEngine;

public class TestSkill : SkillBase
{
    private void Awake()
    {
        cooldown = 5;
    }

    private void Start()
    {
        Debug.Log("시작함");
    }

    protected override void Activate()
    {
        base.Activate();
        FindAnyObjectByType<BaseBoss>().TakeDamage(10);
    }
}