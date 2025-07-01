using UnityEngine;

public class Turtree : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject Mushroom;
    public GameObject PlantFrand;

    public GameObject AttackPrefeb;


    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 200;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //1. 남은 HP에 따라 식물 친구들을 소환. 기본(빠름 데미지). 특수(느림 오브젝트에 닿으면 쿨타임 초기화)
        //2. 공격패턴 -  
        //3. 공격패턴
        //4. 공격패턴
    }
}
