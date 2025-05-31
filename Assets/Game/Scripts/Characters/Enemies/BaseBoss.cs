using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ������ �⺻ Ŭ����
/// </summary>
public abstract class BaseBoss : MonoBehaviour
{
    [Header("�⺻ ����")]
    private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead = false;
    
    [Header("���� ����")]
    private float _patternCooldown = 0.6f;

    [Header("���� �̻� Ŭ����")]
    private BossAbnormalConditions AbnormalConditions;

    // ������Ʈ ����
    private GridSystem _gridSystem;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    
    // ���� ���� �ý���
    private List<IBossAttackPattern> _attackPatterns = new List<IBossAttackPattern>();
    
    // Properties
    /// <summary>
    /// �ִ� ü�� ������Ƽ
    /// </summary>
    public int MaxHealth { get => _maxHealth; protected set => _maxHealth = value; }
    
    /// <summary>
    /// ���� ü�� ������Ƽ
    /// </summary>
    public int CurrentHealth { get => _currentHealth; private set => _currentHealth = value; }
    
    /// <summary>
    /// ��� ���� ������Ƽ
    /// </summary>
    public bool IsDead { get => _isDead; private set => _isDead = value; }
    
    /// <summary>
    /// ���� ��ٿ� ������Ƽ
    /// </summary>
    public float PatternCooldown { get => _patternCooldown; protected set => _patternCooldown = value; }
    
    /// <summary>
    /// �׸��� �ý��� ������Ƽ
    /// </summary>
    public GridSystem GridSystem { get => _gridSystem; }
    
    /// <summary>
    /// �÷��̾� ��Ʈ�ѷ� ������Ƽ
    /// </summary>
    public PlayerController Player { get => _player; }
    
    /// <summary>
    /// �÷��̾� ü�� ������Ƽ
    /// </summary>
    public PlayerHealth PlayerHealth { get => _playerHealth; }
    
    // Events
    /// <summary>
    /// ���� ��� �̺�Ʈ
    /// </summary>
    public event Action OnBossDeath;

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        
        // ������Ʈ ���� ����
        _gridSystem = FindAnyObjectByType<GridSystem>();
        _player = FindAnyObjectByType<PlayerController>();
        _playerHealth = _player != null ? _player.GetComponent<PlayerHealth>() : null;
        
        // ���� ���� �ʱ�ȭ
        InitializeAttackPatterns();
        
        // ���� ��ƾ ����
        StartCoroutine(AttackRoutine());
        
        Debug.Log($"{GetType().Name} spawned with {_maxHealth} HP!");
    }

    /// <summary>
    /// ���� ���� �ʱ�ȭ - ��ӹ޴� Ŭ�������� �ݵ�� ����
    /// </summary>
    protected abstract void InitializeAttackPatterns();

    /// <summary>
    /// ���� ���� �߰�
    /// </summary>
    /// <param name="pattern">�߰��� ���� ����</param>
    protected void AddAttackPattern(IBossAttackPattern pattern)
    {
        _attackPatterns.Add(pattern);
    }

    /// <summary>
    /// ���� ���� ���� ��ƾ
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f); // �ʹ� ������
        
        while (!_isDead)
        {
            ExecuteRandomPattern();
            yield return new WaitForSeconds(_patternCooldown);
        }
    }

    /// <summary>
    /// ���� ���� ����
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
    /// ������ ó��
    /// </summary>
    /// <param name="damage">���� ������</param>
    public virtual void TakeDamage(int damage)
    {
        if (_isDead) return;
        
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth);
        
        Debug.Log($"{GetType().Name} took {damage} damage. Current Health: {_currentHealth}/{_maxHealth}");
        
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ��� ó��
    /// </summary>
    protected virtual void Die()
    {
        _isDead = true;
        Debug.Log($"{GetType().Name} DEFEATED!");
        
        OnBossDeath?.Invoke();
        
        // GameManager�� ���� ��� �˸�
        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.HandleBossDeath();
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// ������ ȿ�� ����
    /// </summary>
    /// <param name="position">ȿ�� ���� ��ġ</param>
    /// <param name="effectPrefab">ȿ�� ������</param>
    public void CreateDamageEffect(Vector3 position, GameObject effectPrefab)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, 0.7f);
        }
    }

    /// <summary>
    /// �÷��̾�� ������ ����
    /// </summary>
    /// <param name="damage">������ ��</param>
    public void ApplyDamageToPlayer(int damage)
    {
        if (_playerHealth != null)
        {
            _playerHealth.TakeDamage(damage);
        }
    }
    /// <summary>
    /// �����̻� �Լ� 
    /// </summary>
    public void AddAbnormalCondition(AbnormalConditions abnormalConditions)
    {
        AbnormalConditions.bossAbnormalConditions.Add(abnormalConditions);
    }

    /// <summary>
    /// �����̻� ���� �о����
    /// </summary>
    /// <returns></returns>
    public List<AbnormalConditions> GetAbnormalCondition()
    {
        return AbnormalConditions.bossAbnormalConditions;
    }

    /// <summary>
    /// �����̻� �ѹ��� �Ҹ�
    /// </summary>
    /// <param name="abnormalConditions"></param>
    public void RemoveAbnormalCondition(AbnormalConditions abnormalConditions)
    {
        AbnormalConditions.AbnormalConditionDestruction(abnormalConditions);
    }
}