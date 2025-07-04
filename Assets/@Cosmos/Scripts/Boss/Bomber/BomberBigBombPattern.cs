using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BomberBigBombPattern : IBossAttackPattern
{
    private GameObject _bombActtck;
    private GameObject _Bigbombball;

    public string PatternName => "LightningKnightSpeardAttack";

    /// <summary>
    /// 보스 생성자
    /// </summary>
    public BomberBigBombPattern(GameObject BombActtck, GameObject Bigbombball)
    {
        _bombActtck = BombActtck;
        _Bigbombball = Bigbombball;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(BombCreate(boss));
        yield return boss.StartCoroutine(Boom(boss));
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _bombActtck != null && boss.BombHandler != null;
    }

    public IEnumerator BombCreate(BaseBoss boss)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        for (int x = -2; x <= 2; x++)
            for(int y = -2; y <= 2; y++)
            {
                if(x % 2 == 0 && y % 2 == 0)
                result.Add(new Vector3Int(x, y, 0));
            }

        boss.BombHandler.ExecuteFixedBomb(result, new Vector3Int (4,4,0), _Bigbombball,
                                             warningDuration: 0.8f, explosionDuration: 0.8f, damage: 0, warningType: WarningType.Type3);

        boss.AttackAnimation();

        yield return new WaitForSeconds(0.8f);
    }

    public IEnumerator Boom(BaseBoss boss)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // 9칸까지 확장 (거리 = 0~8)
        for (int x = -3; x <= 3; x++)
        { 
            for (int y = -3; y <= 3; y++)
            {
                result.Add(new Vector3Int(x, y, 0));
            }
        
        }
        boss.BombHandler.ExecuteFixedBomb(result, new Vector3Int(4, 4, 0), _bombActtck,
                                              warningDuration: 0.8f, explosionDuration: 1f, damage: 25, WarningType.Type1);

        yield return new WaitForSeconds(0.1f);
    }
}
