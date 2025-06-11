using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지속적인 거미줄 설치 패턴 - 플레이어 주변 제외하고 랜덤 설치
/// </summary>
public class ArachneSpiderWebPattern2 : MonoBehaviour
{
    private int _spiderWebCount = 8;
    private GameObject _spiderWebPrefab;
    private bool _isActive = false;

    public void Initialize(GameObject spiderWebPrefab, int webCount = 8)
    {
        _spiderWebPrefab = spiderWebPrefab;
        _spiderWebCount = webCount;
    }

    public void StartPattern()
    {
        if (!_isActive)
        {
            _isActive = true;
            InvokeRepeating(nameof(ExecuteWebAttack), 1f, 3f);
        }
    }

    public void StopPattern()
    {
        _isActive = false;
        CancelInvoke(nameof(ExecuteWebAttack));
    }

    private void ExecuteWebAttack()
    {
        if (!_isActive || AttackPreviewManager.Instance == null) return;

        // 플레이어 주변 3x3 영역을 제외한 랜덤 위치들에 거미줄 설치
        List<Vector3Int> webPositions = GetRandomPositionsExcludingPlayerArea();
        
        if (webPositions.Count > 0)
        {
            AttackPreviewManager.Instance.CreateSpecificPositionAttack(
                gridPositions: webPositions,
                attackPrefab: _spiderWebPrefab,
                previewDuration: 0.8f,
                damage: 0,
                onAttackComplete: () => {
                    SoundManager.Instance?.ArachneSoundClip("SpiderWebPlace");
                }
            );
        }
    }

    /// <summary>
    /// 플레이어 주변 3x3 영역을 제외한 랜덤 위치들 반환
    /// </summary>
    private List<Vector3Int> GetRandomPositionsExcludingPlayerArea()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        PlayerController player = AttackPreviewManager.Instance.PlayerController;
        
        if (player == null) return positions;

        Vector3Int playerPos = GridManager.Instance.WorldToGridPosition(player.transform.position);
        List<Vector3Int> excludedPositions = GetPlayerSurroundingPositions(playerPos);
        
        // 유효한 모든 위치에서 플레이어 주변 제외
        List<Vector3Int> availablePositions = new List<Vector3Int>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (GridManager.Instance.IsWithinGrid(pos) && !excludedPositions.Contains(pos))
                {
                    availablePositions.Add(pos);
                }
            }
        }
        
        // 랜덤하게 선택
        int actualCount = Mathf.Min(_spiderWebCount, availablePositions.Count);
        for (int i = 0; i < actualCount; i++)
        {
            if (availablePositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                positions.Add(availablePositions[randomIndex]);
                availablePositions.RemoveAt(randomIndex);
            }
        }
        
        return positions;
    }

    /// <summary>
    /// 플레이어 주변 3x3 영역 위치들 반환
    /// </summary>
    private List<Vector3Int> GetPlayerSurroundingPositions(Vector3Int playerPos)
    {
        List<Vector3Int> surroundingPositions = new List<Vector3Int>();
        
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int pos = new Vector3Int(playerPos.x + x, playerPos.y + y, 0);
                if (GridManager.Instance.IsWithinGrid(pos))
                {
                    surroundingPositions.Add(pos);
                }
            }
        }
        
        return surroundingPositions;
    }

    private void OnDestroy()
    {
        StopPattern();
    }
}