using UnityEngine;

public class Reaper : BaseBoss
{
    [Header("리퍼 전용 프리팹들")]
    public GameObject ReaperActtck;
    public GameObject DeathAria;

    private bool IsPase2;
    private bool IsPase3;

    protected override void Awake()
    {
        IsPase2 = false; 
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 1000;
    }

    private void Update()
    {
        if ((CurrentHealth) < 500 && !IsPase2)
        {
            this.GetComponent<ReaperDeathAria>().DeathAriaCreationStart();
            IsPase2 = true;
        }

        if ((CurrentHealth) < 200 && !IsPase3)
        {
            this.GetComponent<ReaperDeathAria>().DeathAriaCreation2Start();
            IsPase3 = true;
        }
    }

    /// <summary>
    ///
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new ReaperShortDeathAriaPattern(DeathAria, 9), 1f)
            .AddPattern(new ReaperPattern1(ReaperActtck), 2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new ReaperShortDeathAriaPattern(DeathAria, 4), 1f)
            .AddPattern(new ReaperPattern2(ReaperActtck), 2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new ReaperShortDeathAriaPattern(DeathAria, 3), 1f)
            .AddPattern(new ReaperPattern3(ReaperActtck), 2f)
            .SetGroupInterval(2f);
    }
}
