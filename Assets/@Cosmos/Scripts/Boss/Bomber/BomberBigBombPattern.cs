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
    private float _lastSoundTime = -10f;

    public string PatternName => "3_1";

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
        halfBeat = boss.HalfBeat;
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
            warningType: WarningType.Type3,
            patternName:PatternName
        );

        boss.AttackAnimation();
        yield return new WaitForSeconds(beat);

        // 2. 중앙 확산
        Vector3Int center = basePos + bombPositions[0];
        boss.StartCoroutine(BombSound());
        yield return boss.StartCoroutine(LightningAttack(boss, center));

        yield return new WaitForSeconds(beat);

        // 3. 나머지 4개 폭탄 배치
        List<Vector3Int> others = bombPositions.GetRange(1, 4);
        boss.BombHandler.ExecuteFixedBomb(
            others,
            basePos,
            _Bigbombball,
            warningDuration: 1f,
            explosionDuration: 0.8f,
            damage: 0,
            warningType: WarningType.Type3,
            patternName:PatternName
        );

        yield return new WaitForSeconds(beat);

        // 4. 나머지 4개 확산 병렬 실행
        foreach (var offset in others)
        {
            Vector3Int pos = basePos + offset;
            boss.StartCoroutine(LightningAttack(boss, pos));
        }

        // 5. 확산 길이 보정 대기
        float othersDuration = halfBeat * 3;
        float rounded = Mathf.Ceil(othersDuration / beat) * beat;
        float remainder = rounded - othersDuration;
        yield return new WaitForSeconds(othersDuration + remainder);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _bombActtck != null && boss.BombHandler != null;
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

        for (int dist = 0; dist < 3; dist++)
        {
            List<Vector3Int> result = new();

            if (dist == 0)
            {
                result.Add(Vector3Int.zero); // 중복 없이 자기 자리는 1회만 추가
            }
            else
            {
                foreach (var dir in directions)
                    result.Add(dir * dist);
            }

            boss.BombHandler.ExecuteFixedBomb(
                result,
                centerPos,
                _bombActtck,
                warningDuration: 1f,
                explosionDuration: 1f,
                damage: _damage,
                warningType: WarningType.Type1,
                patternName:PatternName
            );

            boss.StartCoroutine(BombSound());
            yield return new WaitForSeconds(halfBeat);
        }

        float total = halfBeat * 3;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;
        yield return new WaitForSeconds(remainder);
    }

    public IEnumerator BombSound()
    {
        yield return new WaitForSeconds(halfBeat * 3);

        float now = Time.unscaledTime;
        float minInterval = 0.05f;

        if (now - _lastSoundTime >= minInterval)
        {
            _lastSoundTime = now;
            SoundManager.Instance.BomberSoundClip("BomberAttackActivate");
        }
    }
}
