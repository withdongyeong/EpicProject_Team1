using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 손가락 패턴 - 플레이어 추적 손가락 공격
/// </summary>
public class BigHandFingerPattern : IBossAttackPattern
{
    private GameObject _fingerBottomPrefab;
    private GameObject _fingerTopPrefab;
    private GameObject _fingerLeftPrefab;
    private GameObject _fingerRightPrefab;
    private GameObject _attackEffectPrefab;
    private GameObject _fingerObject;
    private List<Vector3Int> _blockedPositions;
    private Vector3Int _currentTargetPos;
    private int _damage;

    public string PatternName => "손가락_추적공격";
    
    public BigHandFingerPattern(GameObject fingerBottomPrefab, GameObject fingerTopPrefab, 
                              GameObject fingerLeftPrefab, GameObject fingerRightPrefab, 
                              GameObject attackEffectPrefab, int damage)
    {
        _fingerBottomPrefab = fingerBottomPrefab;
        _fingerTopPrefab = fingerTopPrefab;
        _fingerLeftPrefab = fingerLeftPrefab;
        _fingerRightPrefab = fingerRightPrefab;
        _attackEffectPrefab = attackEffectPrefab;
        _blockedPositions = new List<Vector3Int>();
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _fingerBottomPrefab != null && 
               _fingerTopPrefab != null &&
               _fingerLeftPrefab != null &&
               _fingerRightPrefab != null &&
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null;
    }

    /// <summary>
    /// 손가락 패턴 실행 - 추적, 공격, 십자 폭발
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        
        Debug.Log("손가락 패턴 시작");
        
        // 손가락 날아오기 + 십자 공격
        yield return boss.StartCoroutine(LaunchFingerAndCrossAttack(boss));
        
        // 보스에 손가락 끝 위치 전달 (BigHand에 프로퍼티 추가 필요)
        var bigHand = boss as BigHand;
        if (bigHand != null)
        {
            // LaunchFingerAndCrossAttack에서 이미 설정됨
        }
        
        Debug.Log("손가락 패턴 완료 - 확산은 별도 패턴에서");
    }

    /// <summary>
    /// 손가락 날아오기 + 십자 공격
    /// </summary>
    private IEnumerator LaunchFingerAndCrossAttack(BaseBoss boss)
    {
        Vector3Int playerPos = new Vector3Int(boss.BombHandler.PlayerController.CurrentX, boss.BombHandler.PlayerController.CurrentY, 0);
        Vector3Int targetPos = GetFingerTargetPosition(playerPos);
        _currentTargetPos = targetPos;
        
        (Vector3 startPos, Vector3 targetWorldPos, GameObject selectedPrefab) = CalculateFingerTrajectory(targetPos);
        List<Vector3Int> fingerPositions = GetFingerPositions(targetPos, playerPos);
        
        Vector3 fingerTipWorld = GridManager.Instance.GridToWorldPosition(fingerPositions[fingerPositions.Count - 1]);
        Vector3 startTipPos = CalculateStartPosition(fingerTipWorld, targetPos);
        
        _fingerObject = Object.Instantiate(selectedPrefab, startTipPos, Quaternion.identity);

        boss.StartCoroutine(PlayAttackSound("BigHandFingerActivate", 1f));

        // 손가락 본체 전조
        foreach (Vector3Int pos in fingerPositions)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { new Vector3Int(0, 0, 0) }, 
                pos, 
                _attackEffectPrefab,
                warningDuration: 1f, 
                explosionDuration: 5f,
                damage: 0, 
                warningType: WarningType.Type2
            );
        }
        
        // 손가락 이동
        float duration = 1f;
        float elapsedTime = 0f;
        
        boss.StartCoroutine(ExecuteCrossAttack(boss, fingerPositions, duration));
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (_fingerObject != null)
                _fingerObject.transform.position = Vector3.Lerp(startTipPos, fingerTipWorld, progress);
            
            yield return null;
        }
        
        if (_fingerObject != null)
            _fingerObject.transform.position = fingerTipWorld;
        
        // 차단 설정
        foreach (Vector3Int pos in fingerPositions)
        {
            GridManager.Instance.AddUnmovableGridPosition(pos);
            _blockedPositions.Add(pos);
        }
        
        // 보스에 손가락 오브젝트 저장 (기존 방식과 동일)
        var bigHand = boss as BigHand;
        if (bigHand != null)
        {
            bigHand.LastFingerTipPosition = fingerPositions[fingerPositions.Count - 1];
            bigHand.FingerBlockedPositions = new List<Vector3Int>(_blockedPositions);
            bigHand.FingerObject = _fingerObject; // 오브젝트 직접 저장
        }
    }

    /// <summary>
    /// 십자 공격
    /// </summary>
    private IEnumerator ExecuteCrossAttack(BaseBoss boss, List<Vector3Int> fingerPositions, float fingerMoveTime)
    {
        if (fingerPositions.Count == 0) yield break;
        
        Vector3Int fingerTip = fingerPositions[fingerPositions.Count - 1];
        
        List<Vector3Int> crossPositions = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0)
        };
        
        float warningDuration = 1f;
        float explosionDelay = fingerMoveTime - warningDuration;
        
        if (explosionDelay > 0)
        {
            yield return new WaitForSeconds(explosionDelay);
        }
        
        boss.StartCoroutine(PlayAttackSound("BigHandAttackActivate", 1f));

        boss.BombHandler.ExecuteFixedBomb(
            crossPositions, 
            fingerTip, 
            _attackEffectPrefab,
            warningDuration: warningDuration, 
            explosionDuration: 0.7f, 
            damage: _damage, 
            warningType: WarningType.Type1
        );
    }

    private IEnumerator ReturnFingerCoroutine()
    {
        if (_fingerObject == null) yield break;
        
        Vector3 currentPos = _fingerObject.transform.position;
        Vector3 exitPos = CalculateExitPosition(currentPos, _currentTargetPos);
        
        foreach (Vector3Int pos in _blockedPositions)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(pos);
        }
        _blockedPositions.Clear();
        
        float duration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            
            if (_fingerObject != null)
                _fingerObject.transform.position = Vector3.Lerp(currentPos, exitPos, progress);
            
            yield return null;
        }
        
        if (_fingerObject != null)
        {
            Object.Destroy(_fingerObject);
            _fingerObject = null;
        }
    }
    
    private Vector3Int GetFingerTargetPosition(Vector3Int playerPos)
    {
        if (playerPos.x == 4 && playerPos.y == 4)
        {
            int randomDirection = Random.Range(0, 4);
            return randomDirection switch
            {
                0 => new Vector3Int(4, 0, 0),
                1 => new Vector3Int(4, 8, 0),
                2 => new Vector3Int(0, 4, 0),
                _ => new Vector3Int(8, 4, 0)
            };
        }
        else if (playerPos.x == 4)
        {
            if (playerPos.y < 4)
                return new Vector3Int(4, 0, 0);
            else
                return new Vector3Int(4, 8, 0);
        }
        else if (playerPos.y == 4)
        {
            if (playerPos.x < 4)
                return new Vector3Int(0, 4, 0);
            else
                return new Vector3Int(8, 4, 0);
        }
        
        if (playerPos.y < 4)
            return new Vector3Int(playerPos.x, 0, 0);
        else if (playerPos.y > 4)
            return new Vector3Int(playerPos.x, 8, 0);
        else if (playerPos.x < 4)
            return new Vector3Int(0, playerPos.y, 0);
        else
            return new Vector3Int(8, playerPos.y, 0);
    }

    private (Vector3, Vector3, GameObject) CalculateFingerTrajectory(Vector3Int targetPos)
    {
        Vector3 targetWorldPos = GridManager.Instance.GridToWorldPosition(targetPos);
        Vector3 startPos;
        GameObject selectedPrefab;
        
        if (targetPos.y <= 2)
        {
            startPos = targetWorldPos + Vector3.down * 15f;
            selectedPrefab = _fingerBottomPrefab;
        }
        else if (targetPos.y >= 6)
        {
            startPos = targetWorldPos + Vector3.up * 15f;
            selectedPrefab = _fingerTopPrefab;
        }
        else if (targetPos.x <= 2)
        {
            startPos = targetWorldPos + Vector3.left * 15f;
            selectedPrefab = _fingerLeftPrefab;
        }
        else
        {
            startPos = targetWorldPos + Vector3.right * 15f;
            selectedPrefab = _fingerRightPrefab;
        }
        
        return (startPos, targetWorldPos, selectedPrefab);
    }

    private List<Vector3Int> GetFingerPositions(Vector3Int targetPos, Vector3Int playerPos)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        Vector3Int direction = Vector3Int.zero;
        if (targetPos.y <= 2)
            direction = Vector3Int.up;
        else if (targetPos.y >= 6)
            direction = Vector3Int.down;
        else if (targetPos.x <= 2)
            direction = Vector3Int.right;
        else
            direction = Vector3Int.left;
        
        int fingerLength = CalculateFingerLength(targetPos, playerPos, direction);
        
        for (int i = 0; i < fingerLength; i++)
        {
            Vector3Int pos = targetPos + direction * i;
            if (IsWithinGrid(pos))
                positions.Add(pos);
        }
        
        return positions;
    }

    private int CalculateFingerLength(Vector3Int targetPos, Vector3Int playerPos, Vector3Int direction)
    {
        int distance = 0;
        
        if (direction == Vector3Int.up || direction == Vector3Int.down)
        {
            distance = Mathf.Abs(playerPos.y - targetPos.y);
        }
        else if (direction == Vector3Int.left || direction == Vector3Int.right)
        {
            distance = Mathf.Abs(playerPos.x - targetPos.x);
        }
        
        return Mathf.Clamp(distance + 1, 1, 4);
    }

    private Vector3 CalculateStartPosition(Vector3 centerWorldPos, Vector3Int targetPos)
    {
        if (targetPos.y <= 2)
            return centerWorldPos + Vector3.down * 15f;
        else if (targetPos.y >= 6)
            return centerWorldPos + Vector3.up * 15f;
        else if (targetPos.x <= 2)
            return centerWorldPos + Vector3.left * 15f;
        else
            return centerWorldPos + Vector3.right * 15f;
    }

    private Vector3 CalculateExitPosition(Vector3 currentPos, Vector3Int targetPos)
    {
        if (targetPos.y <= 2)
            return currentPos + Vector3.down * 15f;
        else if (targetPos.y >= 6)
            return currentPos + Vector3.up * 15f;
        else if (targetPos.x <= 2)
            return currentPos + Vector3.left * 15f;
        else
            return currentPos + Vector3.right * 15f;
    }

    private bool IsWithinGrid(Vector3Int position)
    {
        return position.x >= 0 && position.x < 9 && 
               position.y >= 0 && position.y < 9;
    }

    public void Cleanup()
    {
        foreach (Vector3Int pos in _blockedPositions)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(pos);
        }
        _blockedPositions.Clear();
        
        if (_fingerObject != null)
        {
            Object.Destroy(_fingerObject);
            _fingerObject = null;
        }
    }
    public IEnumerator PlayAttackSound(string SoundName, float BombTime)
    {
        yield return new WaitForSeconds(BombTime); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip(SoundName);
    }

}