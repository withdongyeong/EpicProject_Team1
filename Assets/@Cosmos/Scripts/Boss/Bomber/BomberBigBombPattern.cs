using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BomberBigBombPattern : IBossAttackPattern
{
    private GameObject _bombActtck;
    private GameObject _Bigbombball;
    private int _damage;
    private float beat;
    private float halfBeat;
    private float quarterBeat;
    private float _lastSoundTime = -10f; // 클래스 멤버 필드에 추가
    
    public string PatternName => "BomberBigBombPattern";

    private readonly List<Vector3Int> bombPositions = new()
    {
        new Vector3Int(0, 0, 0),     // center
        new Vector3Int(-2, 2, 0),    // top-left
        new Vector3Int(-2, -2, 0),   // bottom-left
        new Vector3Int(2, 2, 0),     // top-right
        new Vector3Int(2, -2, 0),    // bottom-right
    };

    public BomberBigBombPattern(GameObject BombActtck, GameObject Bigbombball, int damage)
    {
        _bombActtck = BombActtck;
        _Bigbombball = Bigbombball;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        beat = boss.Beat;
        quarterBeat = boss.QuarterBeat;

        Vector3Int basePos = new Vector3Int(4, 4, 0);

        // 1. 중앙 폭탄 배치
        List<Vector3Int> centerOnly = new() { bombPositions[0] };
        boss.BombHandler.ExecuteFixedBomb(
            centerOnly,
            basePos,
            _Bigbombball,
            warningDuration: 1f,
            explosionDuration: 0.8f,
            damage: 0,
            warningType: WarningType.Type3
        );

        boss.AttackAnimation();
        yield return new WaitForSeconds(beat); // 중앙 폭탄 경고 시간

        // 2. 중앙 확산 - 사운드는 여기서 한 번만
        Vector3Int center = basePos + bombPositions[0];
        boss.StartCoroutine(BombSound()); // ✅ 폭발 사운드 예약 (1번만)
        yield return boss.StartCoroutine(LightningAttack(boss, center)); // 동기 실행

        // 3. 나머지 4개 폭탄 배치
        List<Vector3Int> others = bombPositions.GetRange(1, 4);
        boss.BombHandler.ExecuteFixedBomb(
            others,
            basePos,
            _Bigbombball,
            warningDuration: 1f,
            explosionDuration: 0.8f,
            damage: 0,
            warningType: WarningType.Type3
        );

        yield return new WaitForSeconds(beat); // 나머지 폭탄 경고 시간

        // 4. 나머지 4개 확산 - 동시에 실행
        foreach (var offset in others)
        {
            Vector3Int pos = basePos + offset;
            boss.StartCoroutine(LightningAttack(boss, pos)); // 병렬 실행
        }

        // 5. 고정된 확산 시간 만큼 대기 (beat 정렬)
        float othersDuration = quarterBeat * 9;
        float rounded = Mathf.Ceil(othersDuration / beat) * beat;
        float remainder = rounded - othersDuration;
        yield return new WaitForSeconds(othersDuration + remainder);
    }


    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _bombActtck != null && boss.BombHandler != null;
    }

    public IEnumerator BombCreate(BaseBoss boss)
    {
        boss.BombHandler.ExecuteFixedBomb(
            bombPositions,
            new Vector3Int(4, 4, 0),
            _Bigbombball,
            warningDuration: 1f,
            explosionDuration: 0.8f,
            damage: 0,
            warningType: WarningType.Type3
        );

        boss.AttackAnimation();
        yield return new WaitForSeconds(beat);
    }

    private IEnumerator LightningAttack(BaseBoss boss, Vector3Int centerPos)
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

        for (int dist = 0; dist < 3; dist++) // ✅ 확산 길이 3 (0,1,2)
        {
            List<Vector3Int> result = new();
            foreach (var dir in directions)
                result.Add(dir * dist);

            boss.BombHandler.ExecuteFixedBomb(
                result,
                centerPos,
                _bombActtck,
                warningDuration: 1f,
                explosionDuration: 1f,
                damage: _damage,
                warningType: WarningType.Type1
            );

            boss.StartCoroutine(BombSound()); // ✅ 웨이브마다 정확히 1번 사운드
            yield return new WaitForSeconds(quarterBeat);
        }

        float total = quarterBeat * 3;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;

        yield return new WaitForSeconds(remainder);
    }



    public IEnumerator BombSound()
    {
        yield return new WaitForSeconds(quarterBeat * 7);

        float now = Time.unscaledTime;
        float minInterval = 0.05f; // 최소 간격 (초 단위)

        if (now - _lastSoundTime >= minInterval)
        {
            _lastSoundTime = now;
            SoundManager.Instance.BomberSoundClip("BomberAttackActivate");
        }
    }
}
