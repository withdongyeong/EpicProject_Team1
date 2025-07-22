using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightPattern1 : IBossAttackPattern
{
    private GameObject _lightningActtck;
    private int _damage;
    public string PatternName => "8_3";

    public LightningKnightPattern1(GameObject lightningActtck, int damage)
    {
        _lightningActtck = lightningActtck;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _lightningActtck != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(CirclePattern(boss));
    }

    public IEnumerator CirclePattern(BaseBoss boss)
    {
        boss.AttackAnimation();

        Vector3Int center = new Vector3Int(4, 4, 0); // 그리드 중앙

        int randomDirection = Random.Range(0, 4); // 0:오른쪽, 1:아래, 2:왼쪽, 3:위

        // 5단계로 점점 작아지는 사각형 (5 → 1)
        for (int size = 5; size >= 1; size--)
        {
            List<Vector3Int> squareShape = CreateHollowSquare(size, randomDirection);

            boss.StartCoroutine(KnightAttackSound());

            boss.BombHandler.ExecuteFixedBomb(squareShape, center, _lightningActtck,
                                              warningDuration: 1f, explosionDuration: 1f, damage: _damage, warningType:WarningType.Type1, patternName:PatternName);

            if (size == 5 || size == 4)
            {
                yield return new WaitForSeconds(boss.Beat); // 처음은 여유
            }
            else
            {
                yield return new WaitForSeconds(boss.Beat / 2); // 점점 빨라짐
            }
        }
    }

    /// <summary>
    /// 가운데가 비어있는 사각형 생성
    /// </summary>
    private List<Vector3Int> CreateHollowSquare(int size, int baseDirection)
    {
        List<Vector3Int> square = new List<Vector3Int>();

        // 사각형 테두리만 생성 (가운데는 비워둠)
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                // 테두리만 포함 (가장자리이거나 모서리)
                if (x == -size || x == size || y == -size || y == size)
                {
                    // 안전지대 확인
                    if (!IsInSafeZone(x, y, size, baseDirection))
                    {
                        square.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
        }

        return square;
    }

    /// <summary>
    /// 안전지대 확인 (중앙은 규칙적, 외곽은 랜덤)
    /// </summary>
    private bool IsInSafeZone(int x, int y, int size, int baseDirection)
    {
        // 작은 사이즈(1-2)는 규칙적으로
        if (size <= 2)
        {
            return GetFixedSafeZone(x, y, size, baseDirection);
        
        }

        // 큰 사이즈(3-5)는 랜덤성 추가
        bool isFixedSafe = GetFixedSafeZone(x, y, size, baseDirection);

        // 기본 안전지대가 아닌 경우, 25% 확률로 추가 안전지대
        if (!isFixedSafe && Random.Range(0f, 1f) < 0.25f)
        {
            return true;
        }

        return isFixedSafe;
    }

    /// <summary>
    /// 고정 안전지대 위치 계산
    /// </summary>
    private bool GetFixedSafeZone(int x, int y, int size, int baseDirection)
    {
        // 각 사이즈별 기본 안전지대 (baseDirection에 따라 회전)
        Vector3Int safePos = Vector3Int.zero;

        switch (size)
        {
            case 1:
                switch (baseDirection)
                {
                    case 0: safePos = new Vector3Int(1, 0, 0); break;   // 오른쪽
                    case 1: safePos = new Vector3Int(0, -1, 0); break;  // 아래
                    case 2: safePos = new Vector3Int(-1, 0, 0); break;  // 왼쪽
                    case 3: safePos = new Vector3Int(0, 1, 0); break;   // 위
                }
                break;
            case 2:
                switch (baseDirection)
                {
                    case 0: safePos = new Vector3Int(2, 0, 0); break;   // 오른쪽
                    case 1: safePos = new Vector3Int(0, -2, 0); break;  // 아래
                    case 2: safePos = new Vector3Int(-2, 0, 0); break;  // 왼쪽
                    case 3: safePos = new Vector3Int(0, 2, 0); break;   // 위
                }
                break;
            case 3:
                switch (baseDirection)
                {
                    case 0: safePos = new Vector3Int(3, 0, 0); break;   // 오른쪽
                    case 1: safePos = new Vector3Int(0, -3, 0); break;  // 아래
                    case 2: safePos = new Vector3Int(-3, 0, 0); break;  // 왼쪽
                    case 3: safePos = new Vector3Int(0, 3, 0); break;   // 위
                }
                break;
            case 4:
                switch (baseDirection)
                {
                    case 0: safePos = new Vector3Int(4, 0, 0); break;   // 오른쪽
                    case 1: safePos = new Vector3Int(0, -4, 0); break;  // 아래
                    case 2: safePos = new Vector3Int(-4, 0, 0); break;  // 왼쪽
                    case 3: safePos = new Vector3Int(0, 4, 0); break;   // 위
                }
                break;
            case 5:
                switch (baseDirection)
                {
                    case 0: safePos = new Vector3Int(5, 0, 0); break;   // 오른쪽
                    case 1: safePos = new Vector3Int(0, -5, 0); break;  // 아래
                    case 2: safePos = new Vector3Int(-5, 0, 0); break;  // 왼쪽
                    case 3: safePos = new Vector3Int(0, 5, 0); break;   // 위
                }
                break;
        }

        return x == safePos.x && y == safePos.y;
    }

    private IEnumerator KnightAttackSound()
    {
        yield return new WaitForSeconds(1f); // 사운드 재생을 위한 대기
        SoundManager.Instance.KnightSoundClip("KnightAttackActivate");
    }

}
