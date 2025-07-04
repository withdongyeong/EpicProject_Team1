using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TurtreePattern1_1 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private Vector3Int _startPoint;

    public string PatternName => "TurtreePattern1_1";
    public TurtreePattern1_1(GameObject TreeAttackPrefeb, Vector3Int StartPoint)
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

        queue.Enqueue((startPos, 3)); // 시작 방향: 아래(↓)
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            List<Vector3Int> BombPoints = new List<Vector3Int>();

            var (currentPos, currentDir) = queue.Dequeue();

            yield return new WaitForSeconds(0.03f);

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
                // 🔁 방향에 따라 수직 or 수평으로 갈라짐
                int[] splitDirs;

                if (currentDir % 2 == 0)
                {
                    // 현재 방향이 좌/우 → 분기 방향: 위/아래
                    splitDirs = new int[] { 1, 3 }; // Up, Down
                }
                else
                {
                    // 현재 방향이 위/아래 → 분기 방향: 좌/우
                    splitDirs = new int[] { 0, 2 }; // Left, Right
                }

                foreach (int newDir in splitDirs)
                {
                    Vector3Int newPos = currentPos + directions[newDir];

                    if (IsInBounds(newPos) && !visited.Contains(newPos))
                    {
                        visited.Add(newPos);
                        BombPoints.Add(new Vector3Int(newPos.x - 4, newPos.y - 4, 0));
                        queue.Enqueue((newPos, newDir));
                        added = true;
                    }
                }
            }

            if (BombPoints.Count > 0)
            {
                boss.BombHandler.ExecuteFixedBomb(BombPoints, new Vector3Int(4, 4, 0), _treeAttackPrefeb,
                                      warningDuration: 0.8f, explosionDuration: 2, damage: 20);
            }

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