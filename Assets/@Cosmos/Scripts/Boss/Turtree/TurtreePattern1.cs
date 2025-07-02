using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern1 : IBossAttackPattern
{
    private GameObject _TreeAttackPrefeb;

    public string PatternName => "TurtreePattern1";
    public TurtreePattern1(GameObject SlimeFloorPrefeb)
    {
        _TreeAttackPrefeb = SlimeFloorPrefeb;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return BFSExplore(boss, new Vector2Int(8, 4));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _TreeAttackPrefeb != null;
    }

    private readonly Vector2Int[] directions = {
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
    };

    private const int gridSize = 9;

    private HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    private IEnumerator BFSExplore(BaseBoss boss, Vector2Int startPos)
    {
        visited = new HashSet<Vector2Int>();
        Queue<(Vector2Int pos, int dir)> queue = new Queue<(Vector2Int, int)>();

        int initialDirection = 0; // 시작 방향 (왼쪽)
        queue.Enqueue((startPos, initialDirection));
        visited.Add(startPos);

        List<Vector3Int> FirstBombPoint = new List<Vector3Int>();
        FirstBombPoint.Add(new Vector3Int(startPos.x, startPos.y, 0));

        boss.BombHandler.ExecuteFixedBomb(FirstBombPoint, new Vector3Int(4, 4, 0), _TreeAttackPrefeb,
                      warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);

        while (queue.Count > 0)
        {
            List<Vector3Int> BombPoints = new List<Vector3Int>();

            var (currentPos, currentDir) = queue.Dequeue();

            yield return new WaitForSeconds(0.05f);

            Vector2Int nextPos = currentPos + directions[currentDir];

            bool added = false;

            if (IsInBounds(nextPos) && !visited.Contains(nextPos))
            {
                visited.Add(nextPos);
                BombPoints.Add(new Vector3Int(nextPos.x - 4, nextPos.y - 4, 0));
                queue.Enqueue((nextPos, currentDir));
                added = true;
            }
            else
            {
                int leftDir = (currentDir + 3) % 4;
                int rightDir = (currentDir + 1) % 4;

                Vector2Int leftPos = currentPos + directions[leftDir];
                Vector2Int rightPos = currentPos + directions[rightDir];

                if (IsInBounds(leftPos) && !visited.Contains(leftPos))
                {
                    visited.Add(leftPos);
                    BombPoints.Add(new Vector3Int(leftPos.x - 4, leftPos.y - 4, 0));
                    queue.Enqueue((leftPos, leftDir));
                    added = true;
                }

                if (IsInBounds(rightPos) && !visited.Contains(rightPos))
                {
                    visited.Add(rightPos);
                    BombPoints.Add(new Vector3Int(rightPos.x - 4, rightPos.y - 4, 0));
                    queue.Enqueue((rightPos, rightDir));
                    added = true;
                }
            }

            if (BombPoints.Count > 0)
            {
                boss.BombHandler.ExecuteFixedBomb(BombPoints, new Vector3Int(4, 4, 0), _TreeAttackPrefeb,
                                      warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);
            }

            // 💡 루프 중단 조건: 더 이상 유효한 탐색 대상이 없으면 탈출
            if (!added || visited.Count >= gridSize * gridSize)
            {
                break;
            }

            yield return new WaitForSeconds(0.005f);
        }
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }
}
