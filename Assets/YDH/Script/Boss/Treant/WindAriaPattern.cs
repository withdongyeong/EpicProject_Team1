using UnityEngine;
using System.Collections;

public class WindAriaPattern : IBossAttackPattern
{
    public GameObject _windAriaPrefab;

    public string PatternName => "WindAria";

    public WindAriaPattern(GameObject windAriaPrefab)
    {
        _windAriaPrefab = windAriaPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(WindAria(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _windAriaPrefab != null;
    }

    /// <summary>
    /// �ٶ� ���� -  �÷��̾ �̴� ������Ʈ ����
    /// </summary>
    private IEnumerator WindAria(BaseBoss boss)
    {
        //�÷��̾��� ��ġ�� �ٶ� ������Ʈ�� ����

        //�ش� ��ġ�� ������ �ڷ� ���� �̵�
        //1�� �� ����
        yield return 0;
    }
}
