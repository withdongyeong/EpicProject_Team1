using UnityEngine;

public class Treant : BaseBoss
{
    [Header("Ʈ��Ʈ ���� �����յ�")]
    public GameObject WarningTilePrefab;
    public GameObject TreeTrapPrefab;
    
    public GameObject CropsPrefeb;

    public GameObject WarningAriaPrefeb;
    public GameObject TreantWindMagic;

    /// <summary>
    /// ���� �ʱ�ȭ - ������ ���� ����
    /// </summary>
    protected override void Start()
    {
        // �⺻ ���� ����
        MaxHealth = 200;
        PatternCooldown = 0.3f;

        // �θ� Ŭ���� �ʱ�ȭ ȣ��
        base.Start();

    }

    /// <summary>
    /// ���� ���� �ʱ�ȭ - 3���� ���� ��� ���
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //�ٴ� ���� ����
        AddAttackPattern(new TreeTrapPattern(WarningTilePrefab, TreeTrapPrefab));

        //�۹� ������ ����
        AddAttackPattern(new RapidFirePattern(CropsPrefeb, 3, 0.1f));

        //���� �̵� ����
        AddAttackPattern(new EnemyStraightAttack(WarningAriaPrefeb, TreantWindMagic, this.transform));


        Debug.Log($"{GetType().Name}: {GetAttackPatterns().Count} attack patterns initialized");
    }

    /// <summary>
    /// ��ϵ� ���� ���� ��� ��ȯ (����׿�)
    /// </summary>
    private System.Collections.Generic.List<IBossAttackPattern> GetAttackPatterns()
    {
        return new System.Collections.Generic.List<IBossAttackPattern>();
    }
}
