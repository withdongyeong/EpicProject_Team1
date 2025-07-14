using UnityEngine;

public class Bomber : BaseBoss
{
    [Header("폭폭탄 전용 프리팹들")]
    public GameObject BombActtck;
    public GameObject Bombball;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(3).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(3).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(3).strongDamage;
        BPM = GlobalSetting.Instance.GetBossBpm(3);
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 6, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 6, 0), WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 4, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 6, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 4, 0), WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(8, 0, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(7, 1, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(5, 3, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(3, 5, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 6, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(1, 7, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(0, 8, 0), WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(0, 0, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(1, 1, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(3, 3, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(5, 5, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 6, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(7, 7, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(8, 8, 0), WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup()
            .AddPattern(new BomberBigBombPattern(BombActtck, Bombball, StrongDamage), Beat)
            .SetGroupInterval(Beat);
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.BomberSoundClip("BomberDamageActivate");
        base.DamageFeedback();
    }

    protected override void Die()
    {
        SoundManager.Instance.BomberSoundClip("BomberDeadActivate");
        SetAnimationTrigger("DeadTrigger");
        // 기본 사망 처리 호출
        base.Die();
    }
}

