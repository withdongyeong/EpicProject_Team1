using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightPattern2 : IBossAttackPattern
{
    private GameObject _lightningActtck;

    public string PatternName => "LightningKnightPattern2";

    public LightningKnightPattern2(GameObject lightningActtck)
    {
        _lightningActtck = lightningActtck;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _lightningActtck != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 웨이브 패턴 실행 - 매 웨이브마다 다른 방향에서 시작
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        //랜덤으로 4번

        //플레이어 기준 3*3
        //플레이어 기준 5*5 단 플레이어 현재 위치 
        for (int i = 0; i < 4; i++)
        {
             yield return boss.StartCoroutine(PlayerMovePattern(boss));
        }

    }

    public IEnumerator PlayerMovePattern(BaseBoss boss)
    {
        boss.AttackAnimation();

        List<Vector3Int> AttackPoints = new List<Vector3Int>();
        int i = Random.Range(0, 4);

        for(int x = -1;  x<= 1; x++)
        {
            for(int y = -1; y <=1; y++)
            {
                if (x == 0 && y == 0) continue;

                if (i == 0)
                {

                    AttackPoints.Add(new Vector3Int(x - 1, y, 0));
                }

                if (i == 1)
                {
                    AttackPoints.Add(new Vector3Int(x + 1, y, 0));
                }

                if (i == 2)
                {
                    AttackPoints.Add(new Vector3Int(x, y - 1, 0));
                }

                if (i == 3) 
                {
                    AttackPoints.Add(new Vector3Int(x, y + 1, 0));
                }

            }
        }

        boss.BombHandler.ExecuteTargetingBomb(AttackPoints, _lightningActtck,
                                                  warningDuration: 0.8f, explosionDuration: 0.7f, damage: 10);

        yield return new WaitForSeconds(1f);
    }
}