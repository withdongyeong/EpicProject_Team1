using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 디버프 관리 시스템
/// </summary>
public class PlayerDebuff : MonoBehaviour
{
    private PlayerController _playerController;

    private bool _isBind = false;

    public bool IsBind { get => _isBind; set => _isBind = value; }

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    /// <summary>
    /// 일정 시간 동안 속박 디버프 적용
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator Bind(float time)
    {
        _isBind = true;
        yield return new WaitForSeconds(time);
        _isBind = false;
    }
}
