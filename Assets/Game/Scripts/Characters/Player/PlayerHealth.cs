using UnityEngine;
using System;
using System.Collections;
    
/// <summary>
/// 플레이어 체력 관리 시스템
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private int _maxHealth = 100;
    private int _currentHealth;
    // 보호 상태 변수
    private bool _isProtected = false;
    private int _protectionAmount = 0;
    private Coroutine _protectionCoroutine;
    // 방어 상태 변수
    private bool _isShielded = false;
    private int _shieldAmount = 0;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    
    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDeath;
    public event Action<bool> OnProtectionChanged;
    public event Action<bool> OnShieldChanged;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        _currentHealth = _maxHealth;
        
        // 초기 체력 상태 이벤트 발생
        OnHealthChanged?.Invoke(_currentHealth);
    }
    
    /// <summary>
    /// 데미지 처리
    /// </summary>
    public void TakeDamage(int damage)
    {
        // 보호 상태면 보호막량 감소
        if (_isProtected)
        {
            _protectionAmount -= damage;
            if (_protectionAmount <= 0)
            {
                SetProtection(false);
                _protectionAmount = 0;

                // 이미 실행 중인 코루틴이 있다면 중지
                if (_protectionCoroutine != null)
                {
                    StopCoroutine(_protectionCoroutine);
                    _protectionCoroutine = null;
                }
            }
            return;
        }
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 회복 처리
    /// </summary>
    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth);
        
        OnHealthChanged?.Invoke(_currentHealth);
    }

    #region 보호 상태 관련
    /// <summary>
    /// 보호 상태 설정
    /// </summary>
    /// <param name="isShielded">보호 상태 여부</param>
    public void SetProtection(bool isShielded)
    {
        _isProtected = isShielded;
        
        // 보호 상태 변경 이벤트 발생
        OnProtectionChanged?.Invoke(_isProtected);
        
        Debug.Log($"플레이어보호 상태 변경: {_isProtected}");
    }
    
    /// <summary>
    /// 지속시간이 있는 보호  상태 설정
    /// </summary>
    /// <param name="protected">보호 상태 여부</param>
    /// <param name="duration">지속 시간(초)</param>
    public void SetProtection(bool @protected, int duration)
    {
        // 이미 실행 중인 코루틴이 있다면 중지
        if (_protectionCoroutine != null)
        {
            StopCoroutine(_protectionCoroutine);
            _protectionCoroutine = null;
        }
        
        // 보호 상태 설정
        SetProtection(@protected);
        _protectionAmount = duration > 0 ? duration : 0; // 보호막량 설정

        // 지속 시간 설정
        if (@protected && duration > 0)
        {
            _protectionCoroutine = StartCoroutine(protectionTimer(duration));
        }
    }
    
    /// <summary>
    /// 보호 상태 타이머
    /// </summary>
    private IEnumerator protectionTimer(int duration)
    {
        Debug.Log($"보호 상태 시작: {duration}초 동안 지속");

        // 매 초마다 보호막량 감소
        for (int i = 0; i < duration; i++)
        {
            yield return new WaitForSeconds(1f);

            _protectionAmount--;

            if (_protectionAmount <= 0)
            {
                SetProtection(false);
                _protectionAmount = 0;
                _protectionCoroutine = null;
                Debug.Log("보호막 소멸로 보호 상태 종료");
                yield break;
            }
        }
    }
    #endregion

    #region 방어 상태 관련
    /// <summary>
    /// 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    public void SetShield(bool isShielded)
    {
        _isShielded = isShielded;

        // 방어 상태 변경 이벤트 발생
        OnShieldChanged?.Invoke(_isShielded);

        Debug.Log($"플레이어보호 상태 변경: {_isShielded}");
    }

    /// <summary>
    /// 방어도가 있는 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    /// <param name="Amount">지속 시간(초)</param>
    public void SetShield(bool isShielded, int Amount)
    {
        // 방어 상태 설정
        SetShield(isShielded);
        _shieldAmount = Amount > 0 ? Amount : 0; // 방어막량 설정
    }
    #endregion

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어 사망");
        OnPlayerDeath?.Invoke();
    }
    
    private void OnDestroy()
    {
        // 실행 중인 코루틴이 있다면 중지
        if (_protectionCoroutine != null)
        {
            StopCoroutine(_protectionCoroutine);
            _protectionCoroutine = null;
        }
    }
}