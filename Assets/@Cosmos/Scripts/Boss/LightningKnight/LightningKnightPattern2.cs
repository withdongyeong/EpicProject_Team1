using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightPattern2 : IBossAttackPattern
{
    private GameObject _lightningActtck;
    private HashSet<Vector2Int> bannedArea = new HashSet<Vector2Int>();
    private int _damage;

    public string PatternName => "LightningKnightPattern2";

    public LightningKnightPattern2(GameObject lightningActtck, int damage)
    {
        _lightningActtck = lightningActtck;
        _damage = damage;
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
        for (int i = 0; i < 10; i++)
        {
             boss.StartCoroutine(BombPattern(boss));

            yield return new WaitForSeconds(0.3f);
        }

    }

    public IEnumerator BombPattern(BaseBoss boss)
    {
        Vector2Int selectedPos = Vector2Int.zero;
        int attempts = 0;
        int maxAttempts = 100;

        // 랜덤 좌표 뽑기 시도
        do
        {
            int X = Random.Range(1, 8);
            int Y = Random.Range(1, 8);
            selectedPos = new Vector2Int(X, Y);
            attempts++;
        }
        while (IsInBannedArea(selectedPos) && attempts < maxAttempts);

        // 새 위치에 대해 3x3 금지 영역 추가
        AddBannedArea(selectedPos);

        boss.AttackAnimation();

        List<Vector3Int> AttackPoints = new List<Vector3Int>();
        int i = Random.Range(0, 4);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                AttackPoints.Add(new Vector3Int(x, y, 0));
            }
        }

        boss.StartCoroutine(LightningKnightAttackSound());

        boss.BombHandler.ExecuteFixedBomb(
            AttackPoints,
            new Vector3Int(selectedPos.x, selectedPos.y, 0),
            _lightningActtck,
            warningDuration: 0.8f,
            explosionDuration: 0.7f,
            damage: _damage
        );

        yield return new WaitForSeconds(1f);
    }

    private bool IsInBannedArea(Vector2Int pos)
    {
        return bannedArea.Contains(pos);
    }

    private void AddBannedArea(Vector2Int center)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int banPos = new Vector2Int(center.x + dx, center.y + dy);
                bannedArea.Add(banPos);
            }
        }
    }
    private IEnumerator LightningKnightAttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 사운드 재생을 위한 대기
        SoundManager.Instance.KnightSoundClip("KnightAttackActivate");
    }
}