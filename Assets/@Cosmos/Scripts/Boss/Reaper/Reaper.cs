using UnityEngine;
using UnityEngine.Rendering;

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
        MaxHealth = GlobalSetting.Instance.GetBossBalance(7).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(7).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(7).strongDamage;
        BPM = GlobalSetting.Instance.GetBossBpm(7);
    }

    protected override void Update()
    {
        base.Update();
        if ((CurrentHealth) < 1700 && !IsPase2)
        {
            this.GetComponent<ReaperDeathArea>().DeathAreaCreationStart();
            IsPase2 = true;
        }

        if ((CurrentHealth) < 1000 && !IsPase3)
        {
            this.GetComponent<ReaperDeathArea>().DeathAreaCreation2Start();
            IsPase3 = true;
        }
    }

    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new ReaperShortDeathAreaPattern(DeathAria, 9), Beat)
            .AddPattern(new ReaperPattern1(ReaperActtck, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup()
            .AddPattern(new ReaperShortDeathAreaPattern(DeathAria, 4), Beat)
            .AddPattern(new ReaperPattern2(ReaperActtck, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup()
            .AddPattern(new ReaperShortDeathAreaPattern(DeathAria, 3), Beat)
            .AddPattern(new ReaperPattern3(ReaperActtck, StrongDamage), Beat)
            .SetGroupInterval(Beat);
    }

    protected override void Die()
    {
        SoundManager.Instance.ReaperSoundClip("ReaperDeadActivate");

        base.Die();
    }
}
