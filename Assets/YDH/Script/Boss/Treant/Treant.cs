using UnityEngine;

public class Treant : BaseBoss
{
    [Header("Ʈ��Ʈ ���� �����յ�")]

    public GameObject CropsPrefeb;

    public GameObject WindAriaPrefeb;

    public GameObject WarningTilePrefab;
    public GameObject TreeTrapPrefab;
    /// <summary>
    /// ���� �ʱ�ȭ - ������ ���� ����
    /// </summary>
    protected override void Start()
    {
        // �⺻ ���� ����
        MaxHealth = 200;
        PatternCooldown = 1f;

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
        //AddAttackPattern(new RapidFirePattern(SlimeMucus, 3, 0.05f));

        //���� �̵� ����
        //AddAttackPattern(new EnemyStraightAttack(warningAriaPrefab, SlimeActtckTentacle, this.transform));


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
