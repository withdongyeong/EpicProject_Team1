using UnityEngine;

public class Reaper : BaseBoss
{
    [Header("리퍼 전용 프리팹들")]
    public GameObject ReaperActtck;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 1000;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {

    }
}
