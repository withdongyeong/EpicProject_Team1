using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern3 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private int _damage;

    public string PatternName => "TurtreePattern3";

    public TurtreePattern3(GameObject TreeAttackPrefeb, int Damage)
    {
        _treeAttackPrefeb = TreeAttackPrefeb;
        _damage = Damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null &&
               _treeAttackPrefeb != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        Vector3Int centerPos = new Vector3Int(4, 4, 0); // 9x9 중심

        int totalRotationSteps = 5;
        float rotationOffset = 45f; // 각 회차당 회전량 (15도)

        for (int step = 0; step < totalRotationSteps; step++)
        {
            boss.AttackAnimation();

            List<Vector3Int> spokesPattern = new List<Vector3Int>();
            spokesPattern.Add(new Vector3Int(0, 0, 0)); // 중심 포함

            for (int spoke = 0; spoke < 5; spoke++)
            {
                float baseAngle = spoke * 72f;
                float angle = (baseAngle + step * rotationOffset) * Mathf.Deg2Rad;

                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                for (int length = 1; length <= 5; length++)
                {
                    Vector2 pos = direction * length;
                    int x = Mathf.FloorToInt(pos.x + 0.5f); // 오차 방지
                    int y = Mathf.FloorToInt(pos.y + 0.5f);

                    Vector3Int spokePos = new Vector3Int(x, y, 0);
                    Vector3Int absolutePos = centerPos + spokePos;

                    if (IsWithin9x9Grid(absolutePos))
                        spokesPattern.Add(spokePos);
                }
            }

            boss.StartCoroutine(TurtreeAttackSound());

            boss.BombHandler.ExecuteFixedBomb(
                spokesPattern,
                centerPos,
                _treeAttackPrefeb,
                warningDuration: 0.8f,
                explosionDuration: 2f,
                damage: _damage,
                warningType: WarningType.Type1
            );

            yield return new WaitForSeconds(0.8f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private bool IsWithin9x9Grid(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x <= 8 &&
               pos.y >= 0 && pos.y <= 8;
    }

    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}