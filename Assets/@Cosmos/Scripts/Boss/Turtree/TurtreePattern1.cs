using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern1 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private Vector3Int _startPoint;

    public string PatternName => "TurtreePattern1";
    public TurtreePattern1(GameObject TreeAttackPrefeb, Vector3Int StartPoint)
    {
        _treeAttackPrefeb = TreeAttackPrefeb;
        _startPoint = StartPoint;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
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

    private HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

    private IEnumerator BFSExplore(BaseBoss boss, Vector3Int startPos)
    {
        visited = new HashSet<Vector3Int>();
        Queue<(Vector3Int pos, int dir)> queue = new Queue<(Vector3Int, int)>();

        int initialDirection = 0;
        queue.Enqueue((startPos, initialDirection));
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            List<Vector3Int> BombPoints = new List<Vector3Int>();

            var (currentPos, currentDir) = queue.Dequeue();

            yield return new WaitForSeconds(0.03f);

            BombPoints.Add(new Vector3Int(currentPos.x - 4, currentPos.y - 4, 0));

            Vector3Int nextPos = currentPos + directions[currentDir];

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

                Vector3Int leftPos = currentPos + directions[leftDir];
                Vector3Int rightPos = currentPos + directions[rightDir];

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
                boss.BombHandler.ExecuteFixedBomb(BombPoints, new Vector3Int(4, 4, 0), _treeAttackPrefeb,
                                      warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);
            }

            // 💡 루프 중단 조건: 더 이상 유효한 탐색 대상이 없으면 탈출
            if (!added || visited.Count >= gridSize * gridSize)
            {
                break;
            }

            yield return new WaitForSeconds(0.001f);
        }
    }

    private bool IsInBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize && pos.y >= 0 && pos.y < gridSize;
    }
}
