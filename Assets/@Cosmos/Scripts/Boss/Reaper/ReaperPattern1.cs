using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ReaperPattern1 : IBossAttackPattern
{
    private GameObject _deathAriaPrefeb;
    private int _damage;

    public string PatternName => "7_3";

    public ReaperPattern1(GameObject DeathAriaPrefeb, int damage)
    {
        _deathAriaPrefeb = DeathAriaPrefeb;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _deathAriaPrefeb != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 웨이브 패턴 실행 - 매 웨이브마다 다른 방향에서 시작
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        // 3번의 웨이브 실행 (각각 다른 방향)
        for (int wave = 0; wave < 3; wave++)
        {
            // 플레이어 위치를 매번 새로 확인
            Vector3Int playerPos =
                boss.GridSystem.WorldToGridPosition(boss.BombHandler.PlayerController.transform.position);

            boss.AttackAnimation();

            yield return ExecuteWave(boss, playerPos, wave);
            if (wave < 2) yield return new WaitForSeconds(boss.Beat);


        }
    }

    /// <summary>
    /// 단일 웨이브 실행 - 웨이브별로 다른 방향 선택
    /// </summary>
    private IEnumerator ExecuteWave(BaseBoss boss, Vector3Int playerPos, int waveIndex)
    {
        // 웨이브별로 다른 방향 선택 (상하좌우)
        WaveDirection direction = GetWaveDirection(playerPos, waveIndex);

        // 랜덤 안전구간 생성
        int safeIndex1 = Random.Range(0, 4);
        int safeIndex2 = Random.Range(5, 9);

        // 9줄을 순차적으로 실행
        for (int step = 0; step < 9; step++)
        {
            Vector3Int center;
            List<Vector3Int> lineShape1;

            switch (direction)
            {
                case WaveDirection.TopToBottom:
                    center = new Vector3Int(4, 8 - step, 0);
                    lineShape1 = CreateHorizontalLineWithGaps(safeIndex1, safeIndex2);
                    break;
                case WaveDirection.BottomToTop:
                    center = new Vector3Int(4, step, 0);
                    lineShape1 = CreateHorizontalLineWithGaps(safeIndex1, safeIndex2);
                    break;
                case WaveDirection.LeftToRight:
                    center = new Vector3Int(step, 4, 0);
                    lineShape1 = CreateVerticalLineWithGaps(safeIndex1, safeIndex2);
                    break;
                case WaveDirection.RightToLeft:
                    center = new Vector3Int(8 - step, 4, 0);
                    lineShape1 = CreateVerticalLineWithGaps(safeIndex1, safeIndex2);
                    break;
                default:
                    center = new Vector3Int(4, 8 - step, 0);
                    lineShape1 = CreateHorizontalLineWithGaps(safeIndex1, safeIndex2);
                    break;
            }

            boss.StartCoroutine(AttackSoundSound());

            boss.BombHandler.ExecuteFixedBomb(lineShape1, center, _deathAriaPrefeb,
                warningDuration: 1f, explosionDuration: 1f, damage: _damage, warningType:WarningType.Type1, patternName:PatternName);
            
            if (step == 0)
            {
                yield return new WaitForSeconds(boss.Beat);
            }
            else
            {
                yield return new WaitForSeconds(boss.Beat / 4); // 나머지는 빠르게
            }
        }
    }

    /// <summary>
    /// 플레이어 위치와 웨이브 인덱스에 따른 방향 결정
    /// </summary>
    private WaveDirection GetWaveDirection(Vector3Int playerPos, int waveIndex)
    {
        // 플레이어가 있는 영역을 피해서 방향 선택
        WaveDirection[] availableDirections;

        if (playerPos.y <= 2) // 아래쪽에 있으면
        {
            availableDirections = new[]
                { WaveDirection.TopToBottom, WaveDirection.LeftToRight, WaveDirection.RightToLeft };
        }
        else if (playerPos.y >= 6) // 위쪽에 있으면
        {
            availableDirections = new[]
                { WaveDirection.BottomToTop, WaveDirection.LeftToRight, WaveDirection.RightToLeft };
        }
        else if (playerPos.x <= 2) // 왼쪽에 있으면
        {
            availableDirections = new[]
                { WaveDirection.RightToLeft, WaveDirection.TopToBottom, WaveDirection.BottomToTop };
        }
        else if (playerPos.x >= 6) // 오른쪽에 있으면
        {
            availableDirections = new[]
                { WaveDirection.LeftToRight, WaveDirection.TopToBottom, WaveDirection.BottomToTop };
        }
        else // 중앙에 있으면 모든 방향 가능
        {
            availableDirections = new[]
            {
                WaveDirection.TopToBottom, WaveDirection.BottomToTop, WaveDirection.LeftToRight,
                WaveDirection.RightToLeft
            };
        }

        // 웨이브 인덱스에 따라 순환 선택
        return availableDirections[waveIndex % availableDirections.Length];
    }

    /// <summary>
    /// 랜덤 안전구간이 있는 가로 줄 생성
    /// </summary>
    private List<Vector3Int> CreateHorizontalLineWithGaps(int gapColumn1, int gapColumn2)
    {
        List<Vector3Int> line = new List<Vector3Int>();

        for (int x = -4; x <= 4; x++)
        {
            int actualColumn = x + 4;
            if (actualColumn != gapColumn1 && actualColumn != gapColumn2)
            {
                line.Add(new Vector3Int(x, 0, 0));
            }
        }

        return line;
    }

    /// <summary>
    /// 랜덤 안전구간이 있는 세로 줄 생성
    /// </summary>
    private List<Vector3Int> CreateVerticalLineWithGaps(int gapRow1, int gapRow2)
    {
        List<Vector3Int> line = new List<Vector3Int>();

        for (int y = -4; y <= 4; y++)
        {
            int actualRow = y + 4;
            if (actualRow != gapRow1 && actualRow != gapRow2)
            {
                line.Add(new Vector3Int(0, y, 0));
            }
        }

        return line;
    }

    /// <summary>
    /// 웨이브 방향 열거형
    /// </summary>
    private enum WaveDirection
    {
        TopToBottom, // 위→아래
        BottomToTop, // 아래→위
        LeftToRight, // 왼쪽→오른쪽
        RightToLeft // 오른쪽→왼쪽
    }

    private IEnumerator AttackSoundSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.ReaperSoundClip("ReaperAttackActivate");
    }
}
