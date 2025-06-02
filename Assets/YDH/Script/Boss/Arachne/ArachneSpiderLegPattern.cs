using UnityEngine;
using System.Collections;
using System;

public class ArachneSpiderLegPattern : IBossAttackPattern
{
    private GameObject _warningSpiderLegPrefab;
    private GameObject _bigSpiderImage;
    private GameObject _spiderLegPrefab;

    public string PatternName => "ArachneSpiderLeg";

    /// <summary>
    /// ���� ���� ���� ������ - �Ŵ�ȭ �̹���, �ٸ�
    /// </summary>
    public ArachneSpiderLegPattern(GameObject warningSpiderLegPrefab, GameObject bigSpiderImage, GameObject spiderLegPrefab)
    {
        _warningSpiderLegPrefab = warningSpiderLegPrefab;
        _bigSpiderImage = bigSpiderImage;
        _spiderLegPrefab = spiderLegPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderLeg(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _spiderLegPrefab != null;
    }

    /// <summary>
    /// �ٸ� ����
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderLeg(BaseBoss boss)
    {
        GameObject BigSpiderImage = GameObject.Instantiate(_bigSpiderImage);

        //�÷��̾��� ��ġ �������� ���� ���� �����
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        Vector3 tilePos = boss.GridSystem.GetWorldPosition(playerX, playerY);
        GameObject warningSpiderLeg = GameObject.Instantiate(_warningSpiderLegPrefab, tilePos, Quaternion.identity);
        
        yield return new WaitForSeconds(0.3f);
        GameObject.Destroy(warningSpiderLeg);

        //ū �̹����� �ٲٱ�

        //ū�ٸ� ���

        GameObject.Destroy(BigSpiderImage);
        yield return 0;
    }
}

