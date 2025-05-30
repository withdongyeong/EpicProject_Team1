using UnityEngine;
using System;

/// <summary>
/// 모든 타일의 기본 클래스
/// </summary>
public class BaseTile : MonoBehaviour
{
    //타일이 준비되었는지 알려주는 상태
    public enum TileState
    {
        Charging,  // 충전/쿨다운
        Ready,     // 준비 완료
        Activated  // 발동 중
    }
    
    //타일이 준비되기 위해 필요한 시간
    protected float _chargeTime = 3f;
    //타일의 현재 상태
    private TileState _currentState = TileState.Charging; 
    private float _stateTimer;

    //타일의 종류를 모은 enum입니다
    public enum TileType
    {
        Weapon,
        MagicCircle,
        Summon,
        Armor,
        Potion,
        Accessory,
        Null,
        Special
    }
    //이 타일의 종류를 담은 변수입니다
    protected TileType _type;

    // Getters & Setters
    public float ChargeTime { get => _chargeTime; set => _chargeTime = value; }
    public TileType Type { get => _type; }
    
    // 이벤트 정의
    public event Action<BaseTile> OnReady;
    public event Action<BaseTile> OnActivated;

  
    
    protected virtual void Update()
    {
        UpdateState();
    }
    
    /// <summary>
    /// 타일 상태 업데이트 및 상태 전환 관리
    /// </summary>
    protected virtual void UpdateState()
    {
        switch (_currentState)
        {
            case TileState.Charging:
                _stateTimer += Time.deltaTime;
                if (_stateTimer >= _chargeTime)
                {
                    SetState(TileState.Ready);
                }
                break;
        
            case TileState.Ready:
                // 준비 상태 유지
                break;
        
            case TileState.Activated:
                // 발동 후 즉시 충전 상태로
                SetState(TileState.Charging);
                _stateTimer = 0f;
                break;
        }
    }
    
    /// <summary>
    /// 타일 발동 - 플레이어가 밟을 때 호출
    /// </summary>
    public virtual void Activate()
    {
        if (_currentState == TileState.Ready)
        {
            SetState(TileState.Activated);
            OnActivated?.Invoke(this);
        }
    }
    
    /// <summary>
    /// 타일 상태 변경 처리
    /// </summary>
    protected void SetState(TileState newState)
    {
        _currentState = newState;
        _stateTimer = 0f;
        
        if (newState == TileState.Ready)
        {
            OnReady?.Invoke(this);
        }
    }
    
    /// <summary>
    /// 현재 타일 상태 반환
    /// </summary>
    public TileState GetState()
    {
        return _currentState;
    }

    /// <summary>
    /// 충전상태로 설정
    /// </summary>
    public void SetToChargeState()
    {
        SetState(TileState.Charging);
        _stateTimer = 0f;
    }

    /// <summary>
    /// 현재 상태의 진행도(0-1) 반환
    /// </summary>
    public float GetStateProgress()
    {
        switch (_currentState)
        {
            case TileState.Charging:
                return _stateTimer / _chargeTime;
            default:
                return 0f;
        }
    }

    /// <summary>
    /// 넣어준 타입을 비교해서 true/false를 전달해주는 함수입니다
    /// </summary>
    /// <param name="type">비교할 타입입니다.</param>
    /// <returns>이 타일의 타입과 넣어준 타입이 같으면 true입니다</returns>
    public bool CompareTileType(TileType type)
    {
        if (_type == type)
        {
            return true;
        }
        else
            return false;
    }

    public virtual void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        _chargeTime = itemData.ChargeTime;
    }
}   