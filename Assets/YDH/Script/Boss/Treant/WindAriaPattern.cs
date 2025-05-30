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
    /// 바람 공격 -  플레이어를 미는 오브젝트 생성
    /// </summary>
    private IEnumerator WindAria(BaseBoss boss)
    {
        //플레이어의 위치에 바람 오브젝트를 생성

        //해당 위치에 닿으면 뒤로 강제 이동
        //1초 뒤 삭제
        yield return 0;
    }
}
