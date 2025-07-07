using System;
using System.Collections;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    private GameObject _shieldEffectPrefab;
    private float _shieldOffsetDistance = 0.5f; // 플레이어 머리 위로의 오프셋 거리
    private float _updateInterval = 0.02f; // 코루틴 위치 갱신 간격 (초)

    // 방어 상태 변수
    private bool _isShielded = false;
    private int _shieldAmount = 0;
    private GameObject _activeShieldEffect;

    /// <summary>
    /// 현재 방어막 수치입니다.
    /// </summary>
    public int ShieldAmount => _shieldAmount;

    public event Action<bool> OnShieldChanged;

    private void Awake()
    {
        // 방어막 이펙트 프리팹 로드
        _shieldEffectPrefab = Resources.Load<GameObject>("Effect/ShieldEffect");
        if (_shieldEffectPrefab == null)
        {
            Debug.LogError("ShieldEffectPrefab not found in Resources folder.");
        }
    }

    /// <summary>
    /// 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    public void SetShield(bool isShielded)
    {
        _isShielded = isShielded;

        // 방어 상태 변경 이벤트 발생
        OnShieldChanged?.Invoke(_isShielded);

        if (_isShielded)
        {
            // 방어 이펙트 생성
            CreateShieldEffect();
        }
        else
        {
            // 방어 이펙트 제거
            RemoveShieldEffect();
        }
    }

    /// <summary>
    /// 방어도가 있는 방어 상태 설정
    /// </summary>
    /// <param name="isShielded">방어 상태 여부</param>
    /// <param name="amount">방어막량</param>
    public void SetShield(bool isShielded, int amount)
    {
        // 방어 상태 설정
        SetShield(isShielded);
        _shieldAmount = amount > 0 ? amount : 0; // 방어막량 설정
    }

    /// <summary>
    /// 방어막 이펙트 생성
    /// </summary>
    private void CreateShieldEffect()
    {
        if (_shieldEffectPrefab != null)
        {
            // 기존 이펙트가 있다면 제거
            if (_activeShieldEffect != null)
            {
                Destroy(_activeShieldEffect);
            }

            // 월드 좌표계의 up 방향으로 오프셋 설정
            Vector3 shieldOffset = Vector3.up * _shieldOffsetDistance;

            // 플레이어 위치에 실드 이펙트 생성 (부모 없음)
            _activeShieldEffect = Instantiate(
                _shieldEffectPrefab,
                transform.position + shieldOffset, // 월드 up 방향 오프셋 적용
                Quaternion.identity // 회전 고정
            );

            // 코루틴 시작
            StartCoroutine(FollowPlayerCoroutine());
        }
    }

    /// <summary>
    /// 방어막 이펙트 제거
    /// </summary>
    private void RemoveShieldEffect()
    {
        if (_activeShieldEffect != null)
        {
            StopAllCoroutines(); // 코루틴 중지
            Destroy(_activeShieldEffect);
            _activeShieldEffect = null;
        }
    }

    /// <summary>
    /// 방어 절차 수행
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <returns></returns>
    public bool TryShieldBlock(int damage)
    {
        if (!_isShielded) return false;

        _shieldAmount -= damage;
        if (_shieldAmount <= 0)
        {
            SetShield(false);
            _shieldAmount = 0;
        }
        return true;
    }

    /// <summary>
    /// 실드 이펙트가 플레이어를 따라가도록 위치 갱신
    /// </summary>
    private IEnumerator FollowPlayerCoroutine()
    {
        while (_activeShieldEffect != null)
        {
            _activeShieldEffect.transform.position = transform.position + Vector3.up * _shieldOffsetDistance;
            yield return new WaitForSeconds(_updateInterval); // 주기적으로 갱신
        }
    }
}