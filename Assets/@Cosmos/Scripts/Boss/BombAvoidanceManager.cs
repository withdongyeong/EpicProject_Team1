using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 폭탄 피하기 공격 관리자 (다중 전조 타입 지원)
/// </summary>
public class BombAvoidanceManager : MonoBehaviour
{
    private PlayerController _playerController;
    private PlayerHealth _playerHealth;
    
    [Header("전조 타일 프리팹들")]
    public GameObject _warningPrefab;        // Type1 (기본)
    public GameObject _warningPrefabType2;   // Type2
    public GameObject _warningPrefabType3;   // Type3
    
    public PlayerController PlayerController 
    { 
        get => _playerController; 
        set => _playerController = value; 
    }
    
    public GameObject WarningPrefab 
    { 
        get => _warningPrefab; 
        set => _warningPrefab = value; 
    }
    
    public GameObject WarningPrefabType2 
    { 
        get => _warningPrefabType2; 
        set => _warningPrefabType2 = value; 
    }
    
    public GameObject WarningPrefabType3 
    { 
        get => _warningPrefabType3; 
        set => _warningPrefabType3 = value; 
    }
    
    private void Awake()
    {
        if (_playerController == null)
        {
            _playerController = FindAnyObjectByType<PlayerController>();
        }
        
        if (_playerHealth == null && _playerController != null)
        {
            _playerHealth = _playerController.GetComponent<PlayerHealth>();
        }
    }
    
    private void Start()
    {
        // Awake에서 못 찾았으면 Start에서 다시 시도
        if (_playerController == null)
        {
            _playerController = FindAnyObjectByType<PlayerController>();
            Debug.Log($"Start에서 PlayerController 재검색: {_playerController}");
        }
    
        if (_playerHealth == null && _playerController != null)
        {
            _playerHealth = _playerController.GetComponent<PlayerHealth>();
        }
    }
    
    /// <summary>
    /// 전조 타입에 따른 프리팹 반환
    /// </summary>
    /// <param name="warningType">전조 타입</param>
    /// <returns>해당 타입의 전조 프리팹</returns>
    private GameObject GetWarningPrefab(WarningType warningType)
    {
        switch (warningType)
        {
            case WarningType.Type1:
                return _warningPrefab;
            case WarningType.Type2:
                return _warningPrefabType2 ?? _warningPrefab; // null이면 기본값 사용
            case WarningType.Type3:
                return _warningPrefabType3 ?? _warningPrefab; // null이면 기본값 사용
            default:
                Debug.LogWarning($"Unknown WarningType: {warningType}, using default");
                return _warningPrefab;
        }
    }
    
    // ========== 전조만 표시하는 메소드들 ==========
    
    /// <summary>
    /// 특정 위치에 전조만 표시 (기본 타입)
    /// </summary>
    /// <param name="position">전조 표시 위치</param>
    /// <param name="duration">전조 지속 시간</param>
    public void ShowWarningOnly(Vector3Int position, float duration)
    {
        ShowWarningOnly(position, duration, WarningType.Type1);
    }
    
    /// <summary>
    /// 특정 위치에 전조만 표시 (타입 지정)
    /// </summary>
    /// <param name="position">전조 표시 위치</param>
    /// <param name="duration">전조 지속 시간</param>
    /// <param name="warningType">전조 타입</param>
    public void ShowWarningOnly(Vector3Int position, float duration, WarningType warningType)
    {
        StartCoroutine(ShowWarningOnlyCoroutine(position, duration, warningType));
    }
    
    /// <summary>
    /// 여러 위치에 전조만 표시 (기본 타입)
    /// </summary>
    /// <param name="positions">전조 표시 위치들</param>
    /// <param name="duration">전조 지속 시간</param>
    public void ShowWarningOnly(List<Vector3Int> positions, float duration)
    {
        ShowWarningOnly(positions, duration, WarningType.Type1);
    }
    
    /// <summary>
    /// 여러 위치에 전조만 표시 (타입 지정)
    /// </summary>
    /// <param name="positions">전조 표시 위치들</param>
    /// <param name="duration">전조 지속 시간</param>
    /// <param name="warningType">전조 타입</param>
    public void ShowWarningOnly(List<Vector3Int> positions, float duration, WarningType warningType)
    {
        StartCoroutine(ShowWarningOnlyCoroutine(positions, duration, warningType));
    }
    
    /// <summary>
    /// 전조 → 데미지만 (이펙트 없음) - 기본 타입
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="targetPosition">고정 중심 위치</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="damage">데미지</param>
    public void ExecuteWarningThenDamage(List<Vector3Int> shape, Vector3Int targetPosition, 
                                         float warningDuration, int damage)
    {
        ExecuteWarningThenDamage(shape, targetPosition, warningDuration, damage, WarningType.Type1);
    }
    
    /// <summary>
    /// 전조 → 데미지만 (이펙트 없음) - 타입 지정
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="targetPosition">고정 중심 위치</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="damage">데미지</param>
    /// <param name="warningType">전조 타입</param>
    public void ExecuteWarningThenDamage(List<Vector3Int> shape, Vector3Int targetPosition, 
                                         float warningDuration, int damage, WarningType warningType)
    {
        StartCoroutine(ExecuteWarningThenDamageCoroutine(shape, targetPosition, warningDuration, damage, warningType));
    }
    
    /// <summary>
    /// 단일 위치 전조 표시 코루틴
    /// </summary>
    private IEnumerator ShowWarningOnlyCoroutine(Vector3Int position, float duration, WarningType warningType)
    {
        List<Vector3Int> positions = new List<Vector3Int> { position };
        yield return StartCoroutine(ShowWarningOnlyCoroutine(positions, duration, warningType));
    }
    
    /// <summary>
    /// 다중 위치 전조 표시 코루틴
    /// </summary>
    private IEnumerator ShowWarningOnlyCoroutine(List<Vector3Int> positions, float duration, WarningType warningType)
    {
        // 전조 타일 생성
        List<GameObject> warningTiles = CreateWarningTiles(positions, warningType);
        
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(duration);
        
        // 전조 타일 제거
        DestroyWarningTiles(warningTiles);
    }
    
    /// <summary>
    /// 전조 → 데미지만 코루틴 (이펙트 없음)
    /// </summary>
    private IEnumerator ExecuteWarningThenDamageCoroutine(List<Vector3Int> shape, Vector3Int targetPosition, 
                                                          float warningDuration, int damage, WarningType warningType)
    {
        // 공격 위치 계산
        List<Vector3Int> attackPositions = CalculateAttackPositions(shape, targetPosition);
        
        // 지정된 타입의 경고 타일 생성
        List<GameObject> warningTiles = CreateWarningTiles(attackPositions, warningType);
        
        // 경고 대기
        yield return new WaitForSeconds(warningDuration);
        
        // 경고 타일 제거
        DestroyWarningTiles(warningTiles);
        
        // 플레이어 피격 판정만 수행 (이펙트 없음)
        CheckPlayerDamage(attackPositions, damage);
    }
    
    // ========== 기존 메소드들 (기본 타입 사용) ==========
    
    /// <summary>
    /// 플레이어 추적 폭탄 공격 (기본 전조 타입)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    public void ExecuteTargetingBomb(List<Vector3Int> shape, GameObject explosionPrefab, 
                                     float warningDuration, float explosionDuration, int damage)
    {
        ExecuteTargetingBomb(shape, explosionPrefab, warningDuration, explosionDuration, damage, WarningType.Type1);
    }
    
    /// <summary>
    /// 랜덤 위치 폭탄 공격 (기본 전조 타입)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="count">랜덤 중심점 개수</param>
    /// <param name="range">랜덤 생성 범위</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    public void ExecuteRandomBomb(List<Vector3Int> shape, int count, int range, GameObject explosionPrefab, 
                                  float warningDuration, float explosionDuration, int damage)
    {
        ExecuteRandomBomb(shape, count, range, explosionPrefab, warningDuration, explosionDuration, damage, WarningType.Type1);
    }
    
    /// <summary>
    /// 고정 위치 폭탄 공격 (기본 전조 타입)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="targetPosition">고정 중심 위치</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    public void ExecuteFixedBomb(List<Vector3Int> shape, Vector3Int targetPosition, GameObject explosionPrefab, 
                                 float warningDuration, float explosionDuration, int damage)
    {
        ExecuteFixedBomb(shape, targetPosition, explosionPrefab, warningDuration, explosionDuration, damage, WarningType.Type1);
    }
    
    // ========== 새로운 메소드들 (전조 타입 지정 가능) ==========
    
    /// <summary>
    /// 플레이어 추적 폭탄 공격 (전조 타입 지정)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    /// <param name="warningType">전조 타일 타입</param>
    public void ExecuteTargetingBomb(List<Vector3Int> shape, GameObject explosionPrefab, 
                                     float warningDuration, float explosionDuration, int damage, WarningType warningType)
    {
        Vector3Int playerPosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        StartCoroutine(ExecuteBombCoroutine(shape, explosionPrefab, warningDuration, explosionDuration, damage, playerPosition, warningType));
    }
    
    /// <summary>
    /// 랜덤 위치 폭탄 공격 (전조 타입 지정)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="count">랜덤 중심점 개수</param>
    /// <param name="range">랜덤 생성 범위</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    /// <param name="warningType">전조 타일 타입</param>
    public void ExecuteRandomBomb(List<Vector3Int> shape, int count, int range, GameObject explosionPrefab, 
                                  float warningDuration, float explosionDuration, int damage, WarningType warningType)
    {
        StartCoroutine(ExecuteRandomBombCoroutine(shape, count, range, explosionPrefab, warningDuration, explosionDuration, damage, warningType));
    }
    
    /// <summary>
    /// 고정 위치 폭탄 공격 (전조 타입 지정)
    /// </summary>
    /// <param name="shape">공격 모양 (상대 좌표)</param>
    /// <param name="targetPosition">고정 중심 위치</param>
    /// <param name="explosionPrefab">폭발 이펙트 프리팹</param>
    /// <param name="warningDuration">경고 지속 시간</param>
    /// <param name="explosionDuration">폭발 지속 시간</param>
    /// <param name="damage">데미지</param>
    /// <param name="warningType">전조 타일 타입</param>
    public void ExecuteFixedBomb(List<Vector3Int> shape, Vector3Int targetPosition, GameObject explosionPrefab, 
                                 float warningDuration, float explosionDuration, int damage, WarningType warningType)
    {
        StartCoroutine(ExecuteBombCoroutine(shape, explosionPrefab, warningDuration, explosionDuration, damage, targetPosition, warningType));
    }
    
    /// <summary>
    /// 단일 중심점 폭탄 공격 코루틴 (전조 타입 지원)
    /// </summary>
    private IEnumerator ExecuteBombCoroutine(List<Vector3Int> shape, GameObject explosionPrefab, 
                                             float warningDuration, float explosionDuration, int damage, Vector3Int centerPosition, WarningType warningType)
    {
        // 공격 위치 계산
        List<Vector3Int> attackPositions = CalculateAttackPositions(shape, centerPosition);
        
        // 지정된 타입의 경고 타일 생성
        List<GameObject> warningTiles = CreateWarningTiles(attackPositions, warningType);
        
        // 경고 대기
        yield return new WaitForSeconds(warningDuration);
        
        // 플레이어 피격 판정
        CheckPlayerDamage(attackPositions, damage);
        
        // 폭발 이펙트 생성
        CreateExplosionEffects(attackPositions, explosionPrefab, explosionDuration);
        
        // 경고 타일 제거
        DestroyWarningTiles(warningTiles);
    }
    
    /// <summary>
    /// 랜덤 폭탄 공격 코루틴 (전조 타입 지원)
    /// </summary>
    private IEnumerator ExecuteRandomBombCoroutine(List<Vector3Int> shape, int count, int range, GameObject explosionPrefab, 
                                                   float warningDuration, float explosionDuration, int damage, WarningType warningType)
    {
        Vector3Int basePosition = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        List<Vector3Int> allAttackPositions = new List<Vector3Int>();
        
        // 랜덤 중심점들 생성 및 공격 위치 계산
        for (int i = 0; i < count; i++)
        {
            int randomX = Random.Range(-range, range + 1);
            int randomY = Random.Range(-range, range + 1);
            Vector3Int randomCenter = new Vector3Int(basePosition.x + randomX, basePosition.y + randomY, 0);
            
            List<Vector3Int> positions = CalculateAttackPositions(shape, randomCenter);
            allAttackPositions.AddRange(positions);
        }
        
        // 지정된 타입의 경고 타일 생성
        List<GameObject> warningTiles = CreateWarningTiles(allAttackPositions, warningType);
        
        // 경고 대기
        yield return new WaitForSeconds(warningDuration);
        
        // 플레이어 피격 판정
        CheckPlayerDamage(allAttackPositions, damage);
        
        // 폭발 이펙트 생성
        CreateExplosionEffects(allAttackPositions, explosionPrefab, explosionDuration);
        
        // 경고 타일 제거
        DestroyWarningTiles(warningTiles);
    }
    
    /// <summary>
    /// 공격 위치 계산
    /// </summary>
    private List<Vector3Int> CalculateAttackPositions(List<Vector3Int> shape, Vector3Int centerPosition)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        foreach (Vector3Int offset in shape)
        {
            Vector3Int targetPos = centerPosition + offset;
            if (GridManager.Instance.IsWithinGrid(targetPos))
            {
                positions.Add(targetPos);
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// 경고 타일 생성 (기본 타입)
    /// </summary>
    private List<GameObject> CreateWarningTiles(List<Vector3Int> positions)
    {
        return CreateWarningTiles(positions, WarningType.Type1);
    }
    
    /// <summary>
    /// 경고 타일 생성 (전조 타입 지정)
    /// </summary>
    private List<GameObject> CreateWarningTiles(List<Vector3Int> positions, WarningType warningType)
    {
        List<GameObject> warningTiles = new List<GameObject>();
        GameObject warningPrefab = GetWarningPrefab(warningType);
        
        if (warningPrefab == null)
        {
            Debug.LogError($"Warning prefab for type {warningType} is null!");
            return warningTiles;
        }
        
        foreach (Vector3Int gridPos in positions)
        {
            Vector3 worldPos = GridManager.Instance.GridToWorldPosition(gridPos);
            GameObject warningTile = Instantiate(warningPrefab, worldPos, Quaternion.identity);
            warningTiles.Add(warningTile);
        }
        
        return warningTiles;
    }
    
    /// <summary>
    /// 플레이어 피격 판정
    /// </summary>
    private void CheckPlayerDamage(List<Vector3Int> attackPositions, int damage)
    {
        if (damage <= 0) return; // 데미지가 0 이하면 피격 판정 안함 (거미줄 같은 함정용)
        
        Vector3Int playerGridPos = GridManager.Instance.WorldToGridPosition(_playerController.transform.position);
        
        foreach (Vector3Int attackPos in attackPositions)
        {
            if (playerGridPos == attackPos)
            {
                // PlayerHealth를 통해 데미지 적용
                if (_playerHealth != null)
                {
                    _playerHealth.TakeDamage(damage);
                }
                break;
            }
        }
    }
    
    /// <summary>
    /// 폭발 이펙트 생성
    /// </summary>
    private void CreateExplosionEffects(List<Vector3Int> positions, GameObject explosionPrefab, float duration)
    {
        foreach (Vector3Int gridPos in positions)
        {
            Vector3 worldPos = GridManager.Instance.GridToWorldPosition(gridPos);
            GameObject explosion = Instantiate(explosionPrefab, worldPos, Quaternion.identity);
            
            // 지정된 시간 후 이펙트 제거
            Destroy(explosion, duration);
        }
    }
    
    /// <summary>
    /// 경고 타일 제거
    /// </summary>
    private void DestroyWarningTiles(List<GameObject> warningTiles)
    {
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
    }
}