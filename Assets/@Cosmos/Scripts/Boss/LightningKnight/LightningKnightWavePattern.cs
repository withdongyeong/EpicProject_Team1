using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningKnightWavePattern : IBossAttackPattern
{
    private GameObject _lightningActtck;

    public string PatternName => "LightningKnightWavePattern";

    public LightningKnightWavePattern(GameObject lightningActtck)
    {
        _lightningActtck = lightningActtck;
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
        // 3번의 웨이브 실행 (각각 다른 방향)
        for (int wave = 0; wave < 3; wave++)
        {
            // 플레이어 위치를 매번 새로 확인
            Vector3Int playerPos =
                boss.GridSystem.WorldToGridPosition(boss.BombHandler.PlayerController.transform.position);

            yield return ExecuteWave(boss, playerPos, wave);
            if (wave < 2) yield return new WaitForSeconds(0.8f);
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
        int safeIndex = Random.Range(0, 9);

        // 9줄을 순차적으로 실행
        for (int step = 0; step < 9; step++)
        {
            Vector3Int center;
            List<Vector3Int> lineShape;

            switch (direction)
            {
                case WaveDirection.TopToBottom:
                    center = new Vector3Int(4, 8 - step, 0);
                    lineShape = CreateHorizontalLineWithGap(safeIndex);
                    break;
                case WaveDirection.BottomToTop:
                    center = new Vector3Int(4, step, 0);
                    lineShape = CreateHorizontalLineWithGap(safeIndex);
                    break;
                case WaveDirection.LeftToRight:
                    center = new Vector3Int(step, 4, 0);
                    lineShape = CreateVerticalLineWithGap(safeIndex);
                    break;
                case WaveDirection.RightToLeft:
                    center = new Vector3Int(8 - step, 4, 0);
                    lineShape = CreateVerticalLineWithGap(safeIndex);
                    break;
                default:
                    center = new Vector3Int(4, 8 - step, 0);
                    lineShape = CreateHorizontalLineWithGap(safeIndex);
                    break;
            }

            boss.StartCoroutine(boss.PlayOrcExplosionSoundDelayed("OrcMage_SpikeActivate", 0.8f));
            boss.BombHandler.ExecuteFixedBomb(lineShape, center, _lightningActtck,
                warningDuration: 0.8f, explosionDuration: 1f, damage: 25, WarningType.Type1);

            // 첫 번째만 0.3초, 나머지는 0.1초로 빠르게 연속
            if (step == 0)
            {
                yield return new WaitForSeconds(0.3f); // 첫 번째만 여유
            }
            else
            {
                yield return new WaitForSeconds(0.15f); // 나머지는 빠르게
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
    private List<Vector3Int> CreateHorizontalLineWithGap(int gapColumn)
    {
        List<Vector3Int> line = new List<Vector3Int>();

        for (int x = -4; x <= 4; x++)
        {
            int actualColumn = x + 4; // 0~8로 변환
            if (actualColumn != gapColumn)
            {
                line.Add(new Vector3Int(x, 0, 0));
            }
        }

        return line;
    }

    /// <summary>
    /// 랜덤 안전구간이 있는 세로 줄 생성
    /// </summary>
    private List<Vector3Int> CreateVerticalLineWithGap(int gapRow)
    {
        List<Vector3Int> line = new List<Vector3Int>();

        for (int y = -4; y <= 4; y++)
        {
            int actualRow = y + 4; // 0~8로 변환
            if (actualRow != gapRow)
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
}
