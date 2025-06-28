using Unity.VisualScripting;
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
        MaxHealth = 1000;
    }

    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected void Start()
    {

    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 2, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 2, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 6, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 6, 0)), 0.2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 2, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 6, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 4, 0)), 0.2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 2, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 6, 0)), 0.2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 4, 0)), 0.2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 6, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 2, 0)), 0.2f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 6, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0)), 0.2f)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 2, 0)), 0.2f)
            .SetGroupInterval(1.5f);
    }
}

