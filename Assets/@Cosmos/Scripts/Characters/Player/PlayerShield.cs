using System;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [SerializeField] private GameObject _shieldEffectPrefab;

    // 방어 상태 변수
    private bool _isShielded = false;
    private int _shieldAmount = 0;
    private GameObject _activeShieldEffect;

    /// <summary>
    /// 현재 방어막 수치입니다.
    /// </summary>
    public int ShieldAmount => _shieldAmount;

    public event Action<bool> OnShieldChanged;

    /// <summary>
    /// 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    public void SetShield(bool isShielded)
    {
        _isShielded = isShielded;

        // 방어 상태 변경 이벤트 발생
        OnShieldChanged?.Invoke(_isShielded);

        if (_isShielded)
        {
            // 방어 이펙트 생성
            CreateShieldEffect();
        }
        else
        {
            // 방어 이펙트 제거
            RemoveShieldEffect();
        }
    }

    /// <summary>
    /// 방어도가 있는 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    /// <param name="amount">방어막량</param>
    public void SetShield(bool isShielded, int amount)
    {
        // 방어 상태 설정
        SetShield(isShielded);
        _shieldAmount = amount > 0 ? amount : 0; // 방어막량 설정
    }

    /// <summary>
    /// 방어막 이펙트 생성
    /// </summary>
    private void CreateShieldEffect()
    {
        if (_shieldEffectPrefab != null)
        {
            // 기존 이펙트가 있다면 제거
            if (_activeShieldEffect != null)
            {
                Destroy(_activeShieldEffect);
            }

            // 플레이어 위치에 실드 이펙트 생성
            _activeShieldEffect = Instantiate(
                _shieldEffectPrefab,
                transform.position,
                Quaternion.identity,
                transform
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

    /// <summary>
    /// 방어 절차 수행
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <returns></returns>
    public bool TryShieldBlock(int damage)
    {
        if (!_isShielded) return false;

        _shieldAmount -= damage;
        if (_shieldAmount <= 0)
        {
            SetShield(false);
            _shieldAmount = 0;
        }
        return true;
    }

}
