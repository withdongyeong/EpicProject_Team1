using UnityEngine;

public class ProtectionTile : BaseTile
{
    [SerializeField] private int _protectionAmount = 5;
    [SerializeField] private GameObject _protectionEffectPrefab;
    
    private PlayerHealth _playerHealth;
    private GameObject _activeProtectionEffect;
    
    public int ProtectionAmount { get => _protectionAmount; set => _protectionAmount = value; }
    
    private void Start()
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
    }
    
    /// <summary>
    /// 타일 발동 - 플레이어 보호 효과 부여
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && _playerHealth != null)
        {
            GrantProtection();
        }
    }
    
    /// <summary>
    /// 플레이어에게 보호 상태 부여
    /// </summary>
    private void GrantProtection()
    {
        _playerHealth.SetProtection(true, _protectionAmount);
        
        // 보호 이펙트 생성
        CreateProtectionEffect();
        
        // 일정 시간 후 이펙트 제거
        Invoke(nameof(RemoveProtectionEffect), _protectionAmount);
    }
    
    /// <summary>
    /// 보호막 이펙트 생성
    /// </summary>
    private void CreateProtectionEffect()
    {
        if (_protectionEffectPrefab != null && _playerHealth != null)
        {
            // 기존 이펙트가 있다면 제거
            if (_activeProtectionEffect != null)
            {
                Destroy(_activeProtectionEffect);
            }
            
            // 플레이어 위치에 실드 이펙트 생성
            _activeProtectionEffect = Instantiate(
                _protectionEffectPrefab, 
                _playerHealth.transform.position, 
                Quaternion.identity, 
                _playerHealth.transform
            );
        }
    }
    
    /// <summary>
    /// 보호막 이펙트 제거
    /// </summary>
    private void RemoveProtectionEffect()
    {
        if (_activeProtectionEffect != null)
        {
            Destroy(_activeProtectionEffect);
            _activeProtectionEffect = null;
        }
    }
    
    private void OnDestroy()
    {
        // 실행 중인 Invoke 취소
        CancelInvoke(nameof(RemoveProtectionEffect));
        
        // 이펙트 제거
        if (_activeProtectionEffect != null)
        {
            Destroy(_activeProtectionEffect);
        }
    }

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _protectionAmount = itemData.ProtectionAmount;
    }
}