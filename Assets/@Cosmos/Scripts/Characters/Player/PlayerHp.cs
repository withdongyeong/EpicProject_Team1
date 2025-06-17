using UnityEngine;
using System;
    
/// <summary>
/// 플레이어 체력 관리 시스템
/// </summary>
public class PlayerHp : MonoBehaviour
{
    private PlayerShield _playerShield;
    private PlayerProtection _playerProtection;

    private int _maxHealth = 100;
    private int _currentHealth;

    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    
    private void Awake()
    {
        _playerShield = GetComponent<PlayerShield>();
        _playerProtection = GetComponent<PlayerProtection>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
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
        
        if (_currentHealth <= 0)
        {
            // 죽었으면 죽음 애니메이션 재생
            Die();

            SoundManager.Instance.PlayPlayerSound("PlayerDamage");
        }
        else
        {
            // 살아있으면 피격 애니메이션 재생
            FindAnyObjectByType<StageManager>().Player.Animator.SetTrigger("Damaged");

            SoundManager.Instance.PlayPlayerSound("PlayerDead");
        }

        EventBus.PublishPlayerHpChanged(_currentHealth);
    }
    
    /// <summary>
    /// 회복 처리
    /// </summary>
    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth);
        EventBus.PublishPlayerHpChanged(_currentHealth);
        
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("플레이어 사망");
        FindAnyObjectByType<StageManager>().Player.Animator.SetTrigger("Death");
        EventBus.PublishPlayerDeath();
        
    }
}