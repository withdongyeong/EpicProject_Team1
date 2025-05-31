using UnityEngine;

public class ShieldTile : BaseTile
{
    [SerializeField] private int _shieldAmount = 5;
    [SerializeField] private GameObject _shieldEffectPrefab;

    private PlayerHealth _playerHealth;
    private GameObject _activeShieldEffect;

    public int ShieldDuration { get => _shieldAmount; set => _shieldAmount = value; }

    private void Start()
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
    }

    /// <summary>
    /// 타일 발동 - 플레이어 방어 효과 부여
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && _playerHealth != null)
        {
            GrantShield();
        }
    }

    /// <summary>
    /// 플레이어에게 방어 상태 부여
    /// </summary>
    private void GrantShield()
    {
        _playerHealth.SetShield(true, _shieldAmount);

        // 방어 이펙트 생성
        CreateShieldEffect();

        // 일정 시간 후 이펙트 제거
        Invoke(nameof(RemoveShieldEffect), _shieldAmount);
    }

    /// <summary>
    /// 방어막 이펙트 생성
    /// </summary>
    private void CreateShieldEffect()
    {
        if (_shieldEffectPrefab != null && _playerHealth != null)
        {
            // 기존 이펙트가 있다면 제거
            if (_activeShieldEffect != null)
            {
                Destroy(_activeShieldEffect);
            }

            // 플레이어 위치에 실드 이펙트 생성
            _activeShieldEffect = Instantiate(
                _shieldEffectPrefab,
                _playerHealth.transform.position,
                Quaternion.identity,
                _playerHealth.transform
            );
        }
    }

    /// <summary>
    /// 방어막 이펙트 제거
    /// </summary>
    private void RemoveShieldEffect()
    {
        if (_activeShieldEffect != null)
        {
            Destroy(_activeShieldEffect);
            _activeShieldEffect = null;
        }
    }

    private void OnDestroy()
    {
        // 실행 중인 Invoke 취소
        CancelInvoke(nameof(RemoveShieldEffect));

        // 이펙트 제거
        if (_activeShieldEffect != null)
        {
            Destroy(_activeShieldEffect);
        }
    }

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _shieldAmount = itemData.ShieldAmount;
    }
}