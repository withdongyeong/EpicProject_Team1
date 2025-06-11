using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackPreviewManager : MonoBehaviour
{
    public static AttackPreviewManager Instance { get; private set; }
    
    [Header("Settings")]
    private const float DEFAULT_PREVIEW_DURATION = 1f;
    private const float DEFAULT_PERSISTENT_DURATION = 0.2f;
    
    public GameObject attackPreviewPrefab;
    private GameObject damageEffectPrefab;
    private PlayerController playerController;
    private AttackEffectHandler effectHandler;
    private List<PersistentDamageZone> activeDamageZones = new List<PersistentDamageZone>();

    public PlayerController PlayerController { get => playerController; }
    public GameObject AttackPreviewPrefab { get => attackPreviewPrefab; set => attackPreviewPrefab = value; }
    public GameObject DamageEffectPrefab { get => damageEffectPrefab; set => damageEffectPrefab = value; }
    
    /// <summary>
    /// 싱글톤 인스턴스 초기화
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 초기 설정 및 플레이어 컨트롤러 검증
    /// </summary>
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        effectHandler = new AttackEffectHandler(attackPreviewPrefab);
        ValidatePlayerController();
    }
    
    /// <summary>
    /// 지속 데미지 존 업데이트 처리
    /// </summary>
    private void Update()
    {
        UpdatePersistentDamageZones();
    }
    
    #region 공격 생성 메서드들 (기본값 포함)
    
    /// <summary>
    /// 특정 위치들에 전조 생성 후 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreateSpecificPositionAttack(List<Vector3Int> gridPositions, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreateSpecificPositionAttack(gridPositions, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 특정 위치들에 전조 생성 후 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreateSpecificPositionAttack(List<Vector3Int> gridPositions, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        StartCoroutine(ExecuteAttackSequence(gridPositions, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete));
    }
    
    /// <summary>
    /// 랜덤 위치들에 전조 생성 후 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreateRandomPositionAttack(int attackCount, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreateRandomPositionAttack(attackCount, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 랜덤 위치들에 전조 생성 후 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreateRandomPositionAttack(int attackCount, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        List<Vector3Int> randomPositions = AttackPositionCalculator.GetRandomGridPositions(attackCount);
        CreateSpecificPositionAttack(randomPositions, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 플레이어 타게팅 + 주변 범위 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreatePlayerTargetingAttack(int range, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreatePlayerTargetingAttack(range, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 플레이어 타게팅 + 주변 범위 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreatePlayerTargetingAttack(int range, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(playerController.transform.position);
        List<Vector3Int> targetPositions = AttackPositionCalculator.GetAreaPositions(playerGridPos, range);
        CreateSpecificPositionAttack(targetPositions, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 커스텀 패턴 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreateCustomPatternAttack(List<Vector3Int> pattern, Vector3Int centerPosition, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreateCustomPatternAttack(pattern, centerPosition, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 커스텀 패턴 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreateCustomPatternAttack(List<Vector3Int> pattern, Vector3Int centerPosition, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        List<Vector3Int> targetPositions = AttackPositionCalculator.ApplyPatternToPosition(pattern, centerPosition);
        CreateSpecificPositionAttack(targetPositions, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 플레이어 타게팅 커스텀 패턴 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreatePlayerTargetingCustomPattern(List<Vector3Int> pattern, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreatePlayerTargetingCustomPattern(pattern, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 플레이어 타게팅 커스텀 패턴 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreatePlayerTargetingCustomPattern(List<Vector3Int> pattern, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(playerController.transform.position);
        CreateCustomPatternAttack(pattern, playerGridPos, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 랜덤 위치에 커스텀 패턴 지속 공격 (기본 지속시간 사용)
    /// </summary>
    public void CreateRandomCustomPatternAttack(List<Vector3Int> pattern, int patternCount, GameObject attackPrefab, 
        float previewDuration = DEFAULT_PREVIEW_DURATION, int damage = 10, Action onAttackComplete = null)
    {
        CreateRandomCustomPatternAttack(pattern, patternCount, attackPrefab, previewDuration, DEFAULT_PERSISTENT_DURATION, damage, onAttackComplete);
    }
    
    /// <summary>
    /// 랜덤 위치에 커스텀 패턴 지속 공격 (커스텀 지속시간)
    /// </summary>
    public void CreateRandomCustomPatternAttack(List<Vector3Int> pattern, int patternCount, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage = 10, Action onAttackComplete = null)
    {
        List<Vector3Int> allTargetPositions = new List<Vector3Int>();
        List<Vector3Int> randomCenters = AttackPositionCalculator.GetRandomGridPositions(patternCount);
        
        foreach (Vector3Int center in randomCenters)
        {
            List<Vector3Int> patternPositions = AttackPositionCalculator.ApplyPatternToPosition(pattern, center);
            allTargetPositions.AddRange(patternPositions);
        }
        
        CreateSpecificPositionAttack(allTargetPositions, attackPrefab, previewDuration, persistentDuration, damage, onAttackComplete);
    }
    
    #endregion
    
    #region 내부 실행 로직
    
    /// <summary>
    /// 공격 시퀀스 실행 (전조 -> 대기 -> 공격 -> 지속 피격 체크)
    /// </summary>
    private IEnumerator ExecuteAttackSequence(List<Vector3Int> gridPositions, GameObject attackPrefab, 
        float previewDuration, float persistentDuration, int damage, Action onAttackComplete)
    {
        // 1. 전조 생성
        List<GameObject> previewObjects = effectHandler.CreatePreviewObjects(gridPositions);
        
        // 2. 전조 대기
        yield return new WaitForSeconds(previewDuration);
        
        // 3. 전조 제거
        effectHandler.DestroyPreviewObjects(previewObjects);
        
        // 4. 공격 이펙트 생성
        effectHandler.CreateAttackEffects(gridPositions, attackPrefab);
        
        // 5. 지속 데미지 존 생성
        PersistentDamageZone damageZone = new PersistentDamageZone(gridPositions, damage, persistentDuration);
        activeDamageZones.Add(damageZone);
        
        // 6. 완료 콜백 실행
        onAttackComplete?.Invoke();
    }
    
    /// <summary>
    /// 지속 데미지 존 업데이트
    /// </summary>
    private void UpdatePersistentDamageZones()
    {
        if (playerController == null) return;
        
        Vector3Int currentPlayerPos = GridManager.Instance.WorldToGridPosition(playerController.transform.position);
        
        for (int i = activeDamageZones.Count - 1; i >= 0; i--)
        {
            PersistentDamageZone zone = activeDamageZones[i];
            
            zone.RemainingTime -= Time.deltaTime;
            
            bool playerInZone = zone.GridPositions.Contains(currentPlayerPos);
            bool hasDamage = zone.Damage > 0;
            bool notHitYet = !zone.HitPlayers.Contains(playerController);
            
            if (playerInZone && hasDamage && notHitYet)
            {
                ApplyDamageToPlayer(zone.Damage);
                zone.HitPlayers.Add(playerController);
            }
            
            if (zone.RemainingTime <= 0)
            {
                activeDamageZones.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 플레이어에게 데미지 적용
    /// </summary>
    private void ApplyDamageToPlayer(int damage)
    {
        if (playerController != null)
        {
            PlayerHealth playerHealth = playerController.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Player hit! Damage: {damage}");
            }
        }
    }
    
    #endregion
    
    #region 플레이어 관리 및 유틸리티
    
    /// <summary>
    /// 플레이어 컨트롤러 검증
    /// </summary>
    private void ValidatePlayerController()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found!");
            return;
        }
        
        PlayerHealth playerHealth = playerController.GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found!");
        }
    }
    
    /// <summary>
    /// 외부에서 플레이어 참조 설정
    /// </summary>
    public void SetPlayerController(PlayerController newPlayerController)
    {
        playerController = newPlayerController;
    }
    
    /// <summary>
    /// 현재 플레이어 위치 반환
    /// </summary>
    public Vector3Int GetCurrentPlayerPosition()
    {
        return playerController != null ? GridManager.Instance.WorldToGridPosition(playerController.transform.position) : Vector3Int.zero;
    }
    
    /// <summary>
    /// 모든 활성 전조 오브젝트 제거
    /// </summary>
    public void ClearAllPreviews()
    {
        effectHandler?.ClearAllPreviews();
    }
    
    /// <summary>
    /// 모든 활성 데미지 존 제거
    /// </summary>
    public void ClearAllDamageZones()
    {
        activeDamageZones.Clear();
    }
    
    #endregion
}