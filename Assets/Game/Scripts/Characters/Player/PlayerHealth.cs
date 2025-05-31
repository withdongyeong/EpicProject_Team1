using UnityEngine;
using System;
using System.Collections;
    
/// <summary>
/// 플레이어 체력 관리 시스템
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private PlayerShield _playerShield;

    private int _maxHealth = 100;
    private int _currentHealth;
    // 보호 상태 변수
    private bool _isProtected = false;
    private int _protectionAmount = 0;
    private Coroutine _protectionCoroutine;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDeath;
    public event Action<bool> OnProtectionChanged;

    private void Awake()
    {
        _playerShield = GetComponent<PlayerShield>();
    }

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

        // 방어 상태면 방어막량 감소
        if (_playerShield.TryShieldBlock(damage)) 
            return;

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
    /// <param name="amount">보호막량</param>
    public void SetProtection(bool @protected, int amount)
    {
        // 이미 실행 중인 코루틴이 있다면 중지
        if (_protectionCoroutine != null)
        {
            StopCoroutine(_protectionCoroutine);
            _protectionCoroutine = null;
        }
        
        // 보호 상태 설정
        SetProtection(@protected);
        _protectionAmount = amount > 0 ? amount : 0; // 보호막량 설정

        // 지속 시간 설정
        if (@protected && amount > 0)
        {
            _protectionCoroutine = StartCoroutine(protectionTimer(amount));
        }
    }
    
    /// <summary>
    /// 보호 상태 타이머
    /// </summary>
    private IEnumerator protectionTimer(int amount)
    {
        Debug.Log($"보호 상태 시작: {amount}초 동안 지속");

        // 매 초마다 보호막량 감소
        for (int i = 0; i < amount; i++)
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