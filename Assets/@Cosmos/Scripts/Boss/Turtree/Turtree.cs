using UnityEngine;

public class Turtree : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject Mushroom;
    public GameObject Frog;

    public GameObject AttackPrefeb;


    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 800;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new TurtreePattern1(AttackPrefeb, new Vector3Int(8, 4, 0)), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new TurtreePattern1(AttackPrefeb, new Vector3Int(8, 8, 0)), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb), 1f)
            .SetGroupInterval(1f);


        AddGroup()
            .AddPattern(new TurtreePattern1(AttackPrefeb, new Vector3Int(8, 0, 0)), 1f)
            .AddPattern(new TurtreePattern3(AttackPrefeb), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new TurtreePattern4(AttackPrefeb, true), 0.3f)
            .AddPattern(new TurtreePattern1_1(AttackPrefeb, new Vector3Int(0, 8, 0)), 1f)
            .AddPattern(new TurtreePattern4(AttackPrefeb, false), 0.3f)
            .AddPattern(new TurtreePattern1_1(AttackPrefeb, new Vector3Int(8, 8, 0)), 1f)
            .SetGroupInterval(1f);
    }
}
