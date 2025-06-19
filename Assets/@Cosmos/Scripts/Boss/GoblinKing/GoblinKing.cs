using UnityEngine;

public class GoblinKing : BaseBoss
{
    [Header("고블린킹 전용 프리팹들")]
    public GameObject GoblinJunk;
    public GameObject Goblrin;

    private Transform BattleFieldTransform;
    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 200;
        // PatternCooldown = 1f;
    }

    private void Start()
    {
        BattleFieldTransform = FindAnyObjectByType<BattleField>().transform;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //마구 던지기
        // AddAttackPattern(new GoblinJunkPattern(GoblinJunk, 6, 0.1f, this.transform));
        //
        // //고블린 소환
        // AddAttackPattern(new SummonGoblinPattern(Goblrin, 1, BattleFieldTransform));


        Debug.Log($"{GetType().Name}: {GetAttackPatterns().Count} attack patterns initialized");
    }

    /// <summary>
    /// 등록된 공격 패턴 목록 반환 (디버그용)
    /// </summary>
    private System.Collections.Generic.List<IBossAttackPattern> GetAttackPatterns()
    {
        return new System.Collections.Generic.List<IBossAttackPattern>();
    }
}
