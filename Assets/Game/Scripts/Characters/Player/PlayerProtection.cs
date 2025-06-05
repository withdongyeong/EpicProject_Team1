using System;
using UnityEngine;
using System.Collections;

public class PlayerProtection : MonoBehaviour
{
    [SerializeField] private GameObject _protectionEffectPrefab;

    // 보호 상태 변수
    private bool _isProtected = false;
    private int _protectionAmount = 0;
    public int ProtectionAmount => _protectionAmount;
    private Coroutine _protectionCoroutine;
    private GameObject _activeProtectionEffect;



    //보호 상태 이벤트
    public event Action<bool> OnProtectionChanged;

    /// <summary>
    /// 보호 상태 설정
    /// </summary>
    /// <param name="isProtected">보호 상태 여부</param>
    public void SetProtection(bool isProtected)
    {
        _isProtected = isProtected;

        if(_isProtected)
        {
            // 보호 이펙트 생성
            CreateProtectionEffect();
        }
        else
        {
            // 보호 이펙트 제거
            RemoveProtectionEffect();
        }

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
    /// 보호막 이펙트 생성
    /// </summary>
    private void CreateProtectionEffect()
    {
        if (_protectionEffectPrefab != null)
        {
            // 기존 이펙트가 있다면 제거
            if (_activeProtectionEffect != null)
            {
                Destroy(_activeProtectionEffect);
            }

            // 플레이어 위치에 실드 이펙트 생성
            _activeProtectionEffect = Instantiate(
                _protectionEffectPrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
        }
    }

    /// <summary>
    /// 보호막 이펙트 제거
    /// </summary>
    private void RemoveProtectionEffect()
    {
        if (_activeProtectionEffect != null)
        {
            Destroy(_activeProtectionEffect);
            _activeProtectionEffect = null;
        }
    }

    public bool TryProtectionBlock(int damage)
    {
        if (_isProtected && _protectionAmount > 0)
        {
            _protectionAmount -= damage;
            Debug.Log($"보호막으로 {damage} 데미지 차단, 남은 보호막량: {_protectionAmount}");
            if (_protectionAmount <= 0)
            { 
                SetProtection(false);
                _protectionAmount = 0;
                Debug.Log("보호막 소멸로 보호 상태 종료");
            }
            return true; // 보호막으로 데미지 차단 성공
        }
        return false; // 보호막으로 데미지 차단 실패
    }
}
