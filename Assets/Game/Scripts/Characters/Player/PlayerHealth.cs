using UnityEngine;
using System;
    
/// <summary>
/// 플레이어 체력 관리 시스템
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private PlayerShield _playerShield;
    private PlayerProtection _playerProtection;

    private int _maxHealth = 100;
    private int _currentHealth;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }

    public event Action<int> OnHealthChanged;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        _playerShield = GetComponent<PlayerShield>();
        _playerProtection = GetComponent<PlayerProtection>();
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
        if (_playerProtection.TryProtectionBlock(damage))
            return;

        // 방어 상태면 방어막량 감소
        if (_playerShield.TryShieldBlock(damage)) 
            return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            // 죽었으면 죽음 애니메이션 재생
            Die();

            SoundManager.Instance.PlayerDamageSound();
        }
        else
        {
            // 살아있으면 피격 애니메이션 재생
            FindAnyObjectByType<GameManager>().Player.Animator.SetTrigger("Damaged");

            SoundManager.Instance.PlayerDeadSound();
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
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어 사망");
        FindAnyObjectByType<GameManager>().Player.Animator.SetTrigger("Death");
        OnPlayerDeath?.Invoke();
    }
}