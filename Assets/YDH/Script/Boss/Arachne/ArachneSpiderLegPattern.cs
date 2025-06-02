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
    /// 영역 공격 패턴 생성자 - 거대화 이미지, 다리
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
    /// 다리 패턴
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderLeg(BaseBoss boss)
    {
        GameObject BigSpiderImage = GameObject.Instantiate(_bigSpiderImage);

        //플레이어의 위치 가져오고 위험 발판 만들기
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        Vector3 tilePos = boss.GridSystem.GetWorldPosition(playerX, playerY);
        GameObject warningSpiderLeg = GameObject.Instantiate(_warningSpiderLegPrefab, tilePos, Quaternion.identity);
        
        yield return new WaitForSeconds(0.3f);
        GameObject.Destroy(warningSpiderLeg);

        //큰 이미지로 바꾸기

        //큰다리 찍기

        GameObject.Destroy(BigSpiderImage);
        yield return 0;
    }
}

