using System.Collections.Generic;
using System;
using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    /// <summary>
    /// 모든 검 컨트롤러 리스트
    /// </summary>
    private List<SwordController> swords = new List<SwordController>();
    private bool isBurning = false;

    /// <summary>
    /// 불타는 상태 여부
    /// </summary>
    public bool IsBurning
    {
        get { return isBurning; }
        set
        {
            isBurning = value;
            // 불타는 상태가 변경될 때 모든 검에 적용
            foreach (SwordController sword in swords)
            {
                if (sword != null)
                {
                    sword.IsBurning = value;
                }
            }
        }
    }

    #region 칼날폭풍 관련
    ///// <summary>
    ///// 칼날폭풍 스킬 발동
    ///// </summary>
    //public void ActivateBladeStorm()
    //{
    //    // 쿨타임 시작
    //    bladeStormCurrentCooldown = bladeStormCooldown;
    //    OnBladeStormCooldownUpdate?.Invoke(bladeStormCurrentCooldown, bladeStormCooldown);

    //    // 플레이어 현재 위치 저장
    //    GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //    if (playerObj == null) return;

    //    Vector3 playerStartPosition = playerObj.transform.position;
    //    Character.Player.PlayerController playerController = playerObj.GetComponent<Character.Player.PlayerController>();

    //    // 플레이어 이동 방향 계산
    //    Vector2 moveDirection = Vector2.zero;
    //    if (Input.GetKey(KeyCode.W)) moveDirection.y += 1;
    //    if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1;
    //    if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1;
    //    if (Input.GetKey(KeyCode.D)) moveDirection.x += 1;

    //    if (moveDirection.magnitude < 0.1f)
    //    {
    //        moveDirection = Vector2.up; // 기본 방향
    //    }

    //    // 플레이어 대시
    //    if (playerController != null)
    //    {
    //        playerController.StartDash(moveDirection);
    //    }

    //    // 검들을 시작 위치로 모으기
    //    RefreshSwordList();
    //    foreach (SwordController sword in swords)
    //    {
    //        if (sword != null && sword.CurrentState == SwordController.SwordState.Flying)
    //        {
    //            sword.ActivateBladeStorm(playerStartPosition);
    //        }
    //    }

    //    // 쿨타임 시작
    //    bladeStormCurrentCooldown = bladeStormCooldown;

    //    Debug.Log("Blade Storm activated!");
    //}
    #endregion

    /// <summary>
    /// 스킬 발동
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    public void ActivateSkill(Vector3 targetPosition)
    {
        // 검 리스트 새로고침 (혹시 검이 추가/제거된 경우)
        RefreshSwordList();

        // 모든 검에게 스킬 명령
        foreach (SwordController sword in swords)
        {
            if (sword != null && sword.CurrentState == SwordController.SwordState.Flying)
            {
                sword.ActivateSkill(targetPosition);
            }
        }
    }

    /// <summary>
    /// 검 리스트 새로고침
    /// </summary>
    public void RefreshSwordList()
    {
        swords.Clear();
        SwordController foundSword = FindAnyObjectByType<SwordController>();
        Debug.Log($"Found sword: {foundSword!= null}");
        SwordController[] foundSwords = GameObject.FindObjectsByType<SwordController>(0);
        Debug.Log($"Found {foundSwords.Length} swords in the scene");
        swords.AddRange(foundSwords);

        Debug.Log($"Found {swords.Count} swords");
    }

    /// <summary>
    /// 특정 검을 리스트에 추가
    /// </summary>
    /// <param name="sword">추가할 검</param>
    public void AddSword(SwordController sword)
    {
        if (sword != null && !swords.Contains(sword))
        {
            swords.Add(sword);
        }
    }

    /// <summary>
    /// 특정 검을 리스트에서 제거
    /// </summary>
    /// <param name="sword">제거할 검</param>
    public void RemoveSword(SwordController sword)
    {
        if (swords.Contains(sword))
        {
            swords.Remove(sword);
        }
    }
}