using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ReaperPattern3 : IBossAttackPattern
{
    private GameObject _deathAriaPrefeb;
    private int _damage;

    public string PatternName => "ReaperPattern3";

    public ReaperPattern3(GameObject DeathAriaPrefeb, int damage)
    {
        _deathAriaPrefeb = DeathAriaPrefeb;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _deathAriaPrefeb != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        int X1 = Random.Range(1, 3);
        int X2 = Random.Range(6, 8);
        int Y1 = Random.Range(1, 3);
        int Y2 = Random.Range(6, 8);

        yield return AddThreeByThreePos(new Vector3Int(X1, Y1), boss);
        yield return AddThreeByThreePos(new Vector3Int(X2, Y1), boss);
        yield return AddThreeByThreePos(new Vector3Int(X2, Y2), boss);
        yield return AddThreeByThreePos(new Vector3Int(X1, Y2), boss);
    }

    private IEnumerator AddThreeByThreePos(Vector3Int centerPos, BaseBoss boss)
    {
        List<Vector3Int> BombPos = new List<Vector3Int>();

        for(int x  = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                BombPos.Add(new Vector3Int(x, y, 0));
            }
        }

        boss.StartCoroutine(ReaperAttackSound());

        boss.BombHandler.ExecuteFixedBomb(BombPos, centerPos, _deathAriaPrefeb,
            warningDuration: 1f, explosionDuration: 1f, damage: _damage, WarningType.Type1);

        boss.AttackAnimation();

        yield return new WaitForSeconds(boss.Beat/2);
    }

    private IEnumerator ReaperAttackSound()
    {
        yield return new WaitForSeconds(1f); // 사운드 재생을 위한 대기
        SoundManager.Instance.ReaperSoundClip("ReaperAttackActivate");
    }
}