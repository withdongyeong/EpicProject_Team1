using UnityEngine;

public class Treant : BaseBoss
{
    [Header("트랜트 전용 프리팹들")]
    public GameObject TreeTrapPrefab;
    
    public GameObject CropsPrefeb;

    public GameObject WarningAriaPrefeb;
    public GameObject TreantWindMagic;
    
    private void Awake()
    {       
        // 기본 스탯 설정
        MaxHealth = 200;
        PatternCooldown = 0.6f;
    }
    
    /// <summary>
    /// 공격 패턴 초기화 - 3가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //바닥 나무 패턴
        AddAttackPattern(new TreeTrapPattern(TreeTrapPrefab));

        //작물 던지기 패턴
        AddAttackPattern(new RapidFirePattern(CropsPrefeb, 3, 0.1f));

        //강제 이동 패턴
        AddAttackPattern(new WindAriaPattern(WarningAriaPrefeb, TreantWindMagic));

        //덩굴채찍
        AddAttackPattern(new TreantVineWhipPattern(TreeTrapPrefab, 2));

       //종자 뿌리기 - HP 50이하 마다

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
