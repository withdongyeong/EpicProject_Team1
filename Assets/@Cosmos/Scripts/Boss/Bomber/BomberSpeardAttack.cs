using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BomberSpeardAttack : IBossAttackPattern
{
    private GameObject _bombActtck;
    private GameObject _bombball;
    private Vector3Int _centerPos;
    private int _damage;

    private float beat;
    private float quarterBeat;
    public string PatternName => "LightningKnightSpeardAttack";

    /// <summary>
    /// 보스 생성자
    /// </summary>
    public BomberSpeardAttack(GameObject BombActtck, GameObject Bombball, Vector3Int centerPos, int damage)
    {
        _bombActtck = BombActtck;
        _bombball = Bombball;
        _centerPos = centerPos;
        _damage = damage;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        beat = boss.Beat;
        quarterBeat = boss.QuarterBeat;
        //boss.StartCoroutine(BombCreate(boss, _centerPos));
        boss.StartCoroutine(lightningActtck(boss, _centerPos));
        yield return new WaitForSeconds(beat);
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

    public IEnumerator BombCreate(BaseBoss boss, Vector3Int centerPos)
    {
        boss.BombHandler.ExecuteFixedBomb(new List<Vector3Int>() { new Vector3Int(0, 0, 0) }, centerPos, _bombball,
                                             warningDuration: 0f, explosionDuration: 0.3f, damage: 0, warningType: WarningType.Type3);
        
        yield return new WaitForSeconds(quarterBeat);
    }

    public IEnumerator lightningActtck(BaseBoss boss, Vector3Int centerPos)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(1, -1, 0)
        };



        // 9칸까지 확장 (거리 = 0~8)
        for (int dist = 0; dist < 9; dist++)
        {
            List<Vector3Int> result = new List<Vector3Int>();

            foreach (var dir in directions)
            {
                Vector3Int next = dir * dist;

                result.Add(next);
            }

            // 공격 실행
            boss.BombHandler.ExecuteFixedBomb(result, centerPos, _bombActtck,
                                              warningDuration: 1f, explosionDuration: 1f, damage: _damage, WarningType.Type1);

            boss.StartCoroutine(BombSound());

            yield return new WaitForSeconds(quarterBeat);
        }

        // 전체 소요시간: lightningActtck (quarterBeat * 9)
        float total = quarterBeat * 9;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;

        yield return new WaitForSeconds(remainder);
    }

    public IEnumerator BombSound()
    {
        yield return new WaitForSeconds(quarterBeat * 7);
        SoundManager.Instance.BomberSoundClip("BomberAttackActivate");
    }
}
