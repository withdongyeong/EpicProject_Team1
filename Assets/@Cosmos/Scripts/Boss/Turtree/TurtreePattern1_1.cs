using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtreePattern1_1 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private Vector3Int _startPoint;
    private int _damage;

    public string PatternName => "5_2";

    public TurtreePattern1_1(GameObject treeAttackPrefeb, Vector3Int startPoint, int damage)
    {
        _treeAttackPrefeb = treeAttackPrefeb;
        _startPoint = startPoint;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();
        yield return BFSExplore(boss, _startPoint);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _treeAttackPrefeb != null;
    }

    private readonly Vector3Int[] directions = {
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(0, 1, 0),  // Up
        new Vector3Int(1, 0, 0),  // Right
        new Vector3Int(0, -1, 0), // Down
    };

    private const int gridSize = 9;
    private HashSet<Vector3Int> visited = new();

    private IEnumerator BFSExplore(BaseBoss boss, Vector3Int startPos)
    {
        visited = new();
        Queue<(Vector3Int pos, int dir)> queue = new();

        float beat = boss.HalfBeat;
        float lastSoundTime = -999f;
        float soundCooldown = beat;

        // ↓ 방향으로 시작
        int initialDir = 3;
        queue.Enqueue((startPos, initialDir));
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            List<Vector3Int> bombPoints = new();

            for (int i = 0; i < 6 && queue.Count > 0; i++)
            {
                var (currentPos, currentDir) = queue.Dequeue();
                Vector3Int nextPos = currentPos + directions[currentDir];
                bool added = false;

                if (IsInBounds(nextPos) && !visited.Contains(nextPos))
                {
                    visited.Add(nextPos);
                    bombPoints.Add(new Vector3Int(nextPos.x - 4, nextPos.y - 4, 0));
                    queue.Enqueue((nextPos, currentDir));
                    added = true;
                }
                else
                {
                    int leftDir = (currentDir + 3) % 4;
                    int rightDir = (currentDir + 1) % 4;

                    Vector3Int leftPos = currentPos + directions[leftDir];
                    Vector3Int rightPos = currentPos + directions[rightDir];

                    if (IsInBounds(leftPos) && !visited.Contains(leftPos))
                    {
                        visited.Add(leftPos);
                        bombPoints.Add(new Vector3Int(leftPos.x - 4, leftPos.y - 4, 0));
                        queue.Enqueue((leftPos, leftDir));
                        added = true;
                    }

                    if (IsInBounds(rightPos) && !visited.Contains(rightPos))
                    {
                        visited.Add(rightPos);
                        bombPoints.Add(new Vector3Int(rightPos.x - 4, rightPos.y - 4, 0));
                        queue.Enqueue((rightPos, rightDir));
                        added = true;
                    }
                }

                if (!added || visited.Count >= gridSize * gridSize)
                    break;
            }

            if (bombPoints.Count > 0)
            {
                boss.BombHandler.ExecuteFixedBomb(
                    bombPoints,
                    new Vector3Int(4, 4, 0),
                    _treeAttackPrefeb,
                    warningDuration: 1f,
                    explosionDuration: 2f,
                    damage: _damage,
                    patternName:PatternName
                );

                if (Time.time - lastSoundTime >= soundCooldown)
                {
                    boss.StartCoroutine(PlaySoundDelayed(1f));
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(beat);
        }
    }

    private bool IsInBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }

    private IEnumerator PlaySoundDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}
