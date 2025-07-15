using System;
using UnityEngine;

/// <summary>
/// 게임 진행 중 로그로 남길 모든 데이터들을 여기서 측정 합니다.
/// </summary>
public class LogHandler : MonoBehaviour
{
    public float totalPlayTimer = 0f; // 총 플레이 시간
    public float stageClearTimer = 0f;

    private void Awake()
    {
        EventBus.SubscribeGameStart(SetStageTimer);
        SetTotalPlayTimer();
    }

    private void Update()
    {
        UpdateStageTimer();
        UpdateTotalPlayTimer();
    }
    
    
    private void SetStageTimer()
    {
        stageClearTimer = 0f; // 타이머 초기화
    }
    private void UpdateStageTimer()
    {
        if (!GameManager.Instance.IsInStage) return; // 스테이지가 진행 중이 아닐 때는 타이머 업데이트 안 함
        if (stageClearTimer >= 0f)
        {
            stageClearTimer += Time.deltaTime; // 타이머 업데이트
        }
    }
    
    public float GetStageClearTimer()
    {
        return stageClearTimer; // 현재 스테이지 클리어 타이머 반환
    }
    
    
    
    public void SetTotalPlayTimer()
    {
        totalPlayTimer = 0; // 총 플레이 시간 설정
    }

    
    private void UpdateTotalPlayTimer()
    {
        totalPlayTimer += Time.deltaTime;
    }

    public float GetTotalPlayTimer()
    {
        return totalPlayTimer; // 현재 총 플레이 시간 반환
    }

    

    public void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(SetStageTimer);
    }
}
