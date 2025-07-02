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
            .AddPattern(new TurtreeSummonFrog(Frog, 0.7f), 1f)
            .AddPattern(new TurtreePattern1(AttackPrefeb), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new TurtreeSummonFriends(Mushroom, 0.7f), 1f)
            .AddPattern(new TurtreePattern1(AttackPrefeb), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb), 1f)
            .SetGroupInterval(1f);
        //2. 공격패턴 -  
        //3. 공격패턴
        //4. 공격패턴
        //5, 공격패턴
    }
}
