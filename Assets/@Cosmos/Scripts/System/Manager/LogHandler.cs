using UnityEngine;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic; // 🔑 JSON 직렬화를 위해 필요합니다.

/// <summary>
/// 게임 진행 중 로그로 남길 모든 데이터들을 여기서 측정 합니다.
/// </summary>
public class LogHandler : MonoBehaviour
{
    public float totalPlayTimer = 0f; // 총 플레이 시간
    public float sessionPlayTimer = 0f; // 세션 플레이 시간
    public float stageTimer = 0f;

    private List<string> _purchasedTiles = new(); //이번 라운드동안 구매한 타일.
    private List<string> _selledTiles = new(); //이번 라운드동안 판매한 타일.
    public int EnforcedTileNum = 0; // 강화된 마법진의 개수.
    public int totalStarNum = 0; //배치된 별들의 개수.


    private void Awake()
    {
        EventBus.SubscribeGameStart(SetStageTimer);
        EventBus.SubscribeTileSell(AddSelledTile);
        SetTotalPlayTimer();
    }

    private void Update()
    {
        UpdateTimers();
    }


    public int GetStageIndex()
    {
        int idx = StageSelectManager.Instance.StageNum;
        if (SceneLoader.IsInBuilding())
        {
            idx = idx * -1;
        }
        return idx;
    }

    public string GetPlacedTileCountJson()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager is not initialized.");
            return "{}"; // 빈 JSON 객체 반환
        }
        
        var placedTileCount = GridManager.Instance.GetPlacedTileCount();
        string json = JsonConvert.SerializeObject(placedTileCount);
        return json; // 아이템 사용 딕셔너리를 JSON 문자열로 변환
    }
    
    public int GetHealedAmount()
    {
        PlayerHp playerHp = FindAnyObjectByType<PlayerHp>();
        int healedAmount = 0; // 초기화
        if (playerHp != null)
        {
            healedAmount = playerHp.HealedAmount; // 플레이어가 힐한 양을 가져옵니다.
        }
        else
        {
            Debug.LogWarning("PlayerHp component not found.");
        }
        return healedAmount; // 힐한 양 반환
    }

    public int GetProtectionAmount()
    {
        PlayerProtection playerProtection = FindAnyObjectByType<PlayerProtection>();
        int protectionAmount = 0; // 초기화
        if (playerProtection != null)
        {
            protectionAmount = playerProtection.AllProtectionAmount; // 플레이어가 보호한 양을 가져옵니다.
        }
        else
        {
            Debug.LogWarning("PlayerProtection component not found.");
        }
        return protectionAmount; // 보호한 양 반환
    }


    private void UpdateTimers()
    {
        // 총 플레이 시간 업데이트
        totalPlayTimer += Time.deltaTime; 
        
        // 스테이지 클리어 타이머 업데이트
        if (GameManager.Instance.IsInStage  && stageTimer >= 0f)
        {
            stageTimer += Time.deltaTime; 
        }
        
        // 세션 플레이 시간 업데이트
        if ((GameManager.Instance.IsInBuilding || GameManager.Instance.IsInStage) && sessionPlayTimer >= 0f)
        {
            sessionPlayTimer += Time.deltaTime;
        }
    }
    
    private void SetStageTimer()
    {
        stageTimer = 0f; // 타이머 초기화
    }
    public float GetStageTimer()
    {
        return stageTimer; // 현재 스테이지 클리어 타이머 반환
    }
    
    public void SetSessionPlayTimer()
    {
        sessionPlayTimer = 0f; // 세션 플레이 시간 초기화
    }

    public float GetSessionPlayTimer()
    {
        return sessionPlayTimer;
    }
    
    public void SetTotalPlayTimer()
    {
        totalPlayTimer = 0; // 총 플레이 시간 설정
    }

    public float GetTotalPlayTimer()
    {
        return totalPlayTimer; // 현재 총 플레이 시간 반환
    }

    public void AddPurchasedTile(string tileName)
    {
        _purchasedTiles.Add(tileName);
    }

    /// <summary>
    /// 이번 라운드동안 산 타일들 리스트를 받습니다. 그 뒤 산 타일들 리스트를 초기화합니다.
    /// </summary>
    /// <returns>산 타일들을 화염살, 지팡이, 거북이 이런식으로 받습니다.</returns>
    public string GetPurchasedTile()
    {
        string result = string.Join(", ", _purchasedTiles);
        _purchasedTiles.Clear();
        return result;
    }

    public void AddSelledTile(TileObject selledTile)
    {
        _selledTiles.Add(selledTile.GetTileData().TileName);
    }

    public string GetSelledTile()
    {
        string result = string.Join(", ", _selledTiles);
        _selledTiles.Clear();
        return result;
    }


    public void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(SetStageTimer);
        EventBus.UnSubscribeTileSell(AddSelledTile);
    }
}
