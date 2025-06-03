using UnityEngine;

public class GoblinKing : BaseBoss
{
    [Header("���ŷ ���� �����յ�")]
    public GameObject GoblinJunk;
    public GameObject Goblrin;

    private Transform BattleFieldTransform;
    /// <summary>
    /// ���� �ʱ�ȭ - ������ ���� ����
    /// </summary>

    protected override void Start()
    {
        // �⺻ ���� ����
        MaxHealth = 200;
        PatternCooldown = 1f;

        BattleFieldTransform = FindAnyObjectByType<BattleField>().transform;

        // �θ� Ŭ���� �ʱ�ȭ ȣ��
        base.Start();
    }

    /// <summary>
    /// ���� ���� �ʱ�ȭ - 2���� ���� ��� ���
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //���� ������
        AddAttackPattern(new GoblinJunkPattern(GoblinJunk, 6, 0.1f, this.transform));

        //��� ��ȯ
        AddAttackPattern(new SummonGoblinPattern(Goblrin, 1, BattleFieldTransform));


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
