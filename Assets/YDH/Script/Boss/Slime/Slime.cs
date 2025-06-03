using UnityEngine;

public class Slime : BaseBoss
{
    [Header("������ ���� �����յ�")]
    public GameObject warningTilePrefab;

    public GameObject SlimeMucus;

    public GameObject warningAriaPrefab;
    public GameObject SlimeActtckTentacle;

    public GameObject SlimeTrapTentacle;
    /// <summary>
    /// ���� �ʱ�ȭ - ������ ���� ����
    /// </summary>
    protected override void Start()
    {
        // �⺻ ���� ����
        MaxHealth = 200;
        PatternCooldown = 0.5f;

        // �θ� Ŭ���� �ʱ�ȭ ȣ��
        base.Start();
    
    }

    /// <summary>
    /// ���� ���� �ʱ�ȭ - 2���� ���� ��� ���
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //���� ����
        AddAttackPattern(new RapidFirePattern(SlimeMucus, 3, 0.05f));

        //��� �˼� ����
        AddAttackPattern(new EnemyStraightAttack(warningAriaPrefab, SlimeActtckTentacle, this.transform));

        //�ٴ� ���� �˼� ����
        AddAttackPattern(new DiagonalCrossPattern(warningTilePrefab, SlimeTrapTentacle));

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
