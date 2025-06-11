﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 보스의 기본 클래스
/// </summary>
public abstract class BaseBoss : MonoBehaviour
{
    [Header("기본 스탯")]
    private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead = false;
    
    [Header("공격 설정")]
    private float _patternCooldown = 0.6f;

    [Header("상태 이상 클래스")]
    private BossDebuffs _bossDebuff;
    private bool _isStopped = false; // 공격 중지 여부

    // 컴포넌트 참조
    private GridManager _gridSystem;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    
    // 공격 패턴 시스템
    private List<IBossAttackPattern> _attackPatterns = new List<IBossAttackPattern>();

    private Animator _animator;

    // Properties
    /// <summary>
    /// 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    
    /// <summary>
    /// 현재 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }
    
    /// <summary>
    /// 사망 여부 프로퍼티
    /// </summary>
    public bool IsDead { get => _isDead; private set => _isDead = value; }
    
    /// <summary>
    /// 패턴 쿨다운 프로퍼티
    /// </summary>
    public float PatternCooldown { get => _patternCooldown; protected set => _patternCooldown = value; }

    /// <summary>
    /// 공격 중지 여부 프로퍼티
    /// </summary>
    public bool IsStopped { get => _isStopped; set => _isStopped = value; }

    /// <summary>
    /// 그리드 시스템 프로퍼티
    /// </summary>
    public GridManager GridSystem { get => _gridSystem; }
    
    /// <summary>
    /// 플레이어 컨트롤러 프로퍼티
    /// </summary>
    public PlayerController Player { get => _player; }
    
    /// <summary>
    /// 플레이어 체력 프로퍼티
    /// </summary>
    public PlayerHealth PlayerHealth { get => _playerHealth; }

    // Events
    /// <summary>
    /// 보스 사망 이벤트
    /// </summary>
    public event Action OnBossDeath;

    public Animator Animator { get => _animator; }

    /// <summary>
    /// 보스 초기화
    /// </summary>
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        
        // 컴포넌트 참조 설정
        _gridSystem = GridManager.Instance;
        _player = FindAnyObjectByType<PlayerController>();
        _playerHealth = _player != null ? _player.GetComponent<PlayerHealth>() : null;
        _bossDebuff = GetComponent<BossDebuffs>();
        _animator = GetComponent<Animator>();
        // 공격 패턴 초기화
        InitializeAttackPatterns();
        
        // 공격 루틴 시작
        StartCoroutine(AttackRoutine());

        // 상태이상 적용 루틴 시작
        StartCoroutine(ApplyDebuffsRoutine());

        Debug.Log($"{GetType().Name} spawned with {_maxHealth} HP!");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 상속받는 클래스에서 반드시 구현
    /// </summary>
    protected abstract void InitializeAttackPatterns();

    /// <summary>
    /// 공격 패턴 추가
    /// </summary>
    /// <param name="pattern">추가할 공격 패턴</param>
    protected void AddAttackPattern(IBossAttackPattern pattern)
    {
        _attackPatterns.Add(pattern);
    }

    /// <summary>
    /// 공격 패턴 실행 루틴
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f); // 초반 딜레이
        
        while (!_isDead && !_isStopped)
        {
            ExecuteRandomPattern();
            yield return new WaitForSeconds(_patternCooldown);
        }
    }

    /// <summary>
    /// 랜덤 패턴 실행
    /// </summary>
    private void ExecuteRandomPattern()
    {
        if (_attackPatterns.Count == 0)
        {
            Debug.LogWarning($"{GetType().Name}: No attack patterns registered!");
            return;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, _attackPatterns.Count);
        IBossAttackPattern selectedPattern = _attackPatterns[randomIndex];
        
        if (selectedPattern.CanExecute(this))
        {
            selectedPattern.Execute(this);
        }
    }

    /// <summary>
    /// 데미지 처리
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    public virtual void TakeDamage(int damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        Debug.Log(damage);
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        else
        {
            _animator.SetTrigger("DamageTrigger");
        }
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log($"{GetType().Name} DEFEATED!");
        
        OnBossDeath?.Invoke();
        _animator.SetTrigger("DeadTrigger");

        // GameManager에 보스 사망 알림
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            StartCoroutine(BossDeath(gameManager));
        }
    }

    private IEnumerator BossDeath(GameManager gameManager)
    {
        yield return new WaitForSeconds(0.1f);
        gameManager.HandleBossDeath();
    }

    /// <summary>
    /// 데미지 효과 생성
    /// </summary>
    /// <param name="position">효과 생성 위치</param>
    /// <param name="effectPrefab">효과 프리팹</param>
    public void CreateDamageEffect(Vector3 position, GameObject effectPrefab, float second)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
            Destroy(effect, second);
        }
    }

    public void CreateDamageEffect_Inversion(Vector3 position, GameObject effectPrefab, float second)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            effect.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;
            Destroy(effect, second);
        }
    }

    /// <summary>
    /// 플레이어에게 데미지 적용
    /// </summary>
    /// <param name="damage">데미지 양</param>
    public void ApplyDamageToPlayer(int damage)
    {
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(damage);
        }
    }

    /// <summary>
    /// 상태이상 적용 루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator ApplyDebuffsRoutine()
    {
        yield return new WaitForSeconds(1f); // 초반 딜레이

        while (!_isDead && !_isStopped)
        {
            _bossDebuff.ApplyAllDebuffs();
            yield return new WaitForSeconds(1f);
        }

    }

    /// <summary>
    /// 공격 중지 메소드
    /// </summary>
    /// <param name="time"></param>
    public void StopAttack(float time)
    {
        StartCoroutine(StopAttackRoutine(time));
    }

    /// <summary>
    /// 공격 중지 루틴
    /// </summary>
    /// <param name="time">중지 시간</param>
    public IEnumerator StopAttackRoutine(float time)
    {
        _isStopped = true;
        StopCoroutine(AttackRoutine());
        yield return new WaitForSeconds(time);
        _isStopped = false;
        StartCoroutine(AttackRoutine());
        StartCoroutine(ApplyDebuffsRoutine());
    }

    /// <summary>
    /// 상태이상 추가
    /// </summary>
    /// <param name="debuff">추가할 상태이상</param>
    public void AddDebuff(BossDebuff debuff)
    {
        _bossDebuff.AddDebuff(debuff);
    }

    /// <summary>
    /// 상태이상 개수 조회
    /// </summary>
    /// <param name="debuff"></param>
    /// <returns></returns>
    public int GetDebuffCount(BossDebuff debuff)
    {
        return _bossDebuff.Debuffs[(int)debuff];
    }

    /// <summary>
    /// 상태이상 제거
    /// </summary>
    /// <param name="debuff">제거할 디버프</param>
    public void RemoveDebuff(BossDebuff debuff)
    {
        _bossDebuff.RemoveDebuff(debuff);
    }

    public void AttackAnimation()
    {
        _animator.SetTrigger("AttackTrigger");
    }
}