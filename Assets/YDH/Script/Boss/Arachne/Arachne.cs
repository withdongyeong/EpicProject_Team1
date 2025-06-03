using UnityEngine;
using System.Collections.Generic;

public class Arachne : BaseBoss
{
    [Header("���� ���� �����յ�")]
    public GameObject SpiderWebPrefeb;

    public List<GameObject> SummonSpiders;

    public GameObject spiderSilkPrefeb;

    public GameObject warningAria;
    public GameObject poisionAriaPrefeb;

    public GameObject bigSpiderImage;
    public GameObject SpiderLeg;

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
    /// ���� ���� �ʱ�ȭ - 5���� ���� ��� ���
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // ���� 1: �Ź���
        AddAttackPattern(new ArachneSpiderWebPattern(SpiderWebPrefeb, 2, this.transform));

        // ���� 2: ���� �Ź� ����
        AddAttackPattern(new ArachneSummonSpiderPattern(SummonSpiders, 2, this.transform));

        // ���� 3: �Ź��� ���
        AddAttackPattern(new ArachneSpiderSilkPattern(spiderSilkPrefeb, 1, this.transform));

        // ���� 4: �� ����
        AddAttackPattern(new ArachnePoisionAriaPattern(warningAria, poisionAriaPrefeb));

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
