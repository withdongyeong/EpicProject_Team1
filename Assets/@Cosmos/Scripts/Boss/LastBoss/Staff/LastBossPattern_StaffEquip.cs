using System.Collections;
using UnityEngine;

/// <summary>
/// 최종보스 - 스태프 장착 전용 패턴 (공격 없음, 회전 포함)
/// </summary>
public class LastBossPattern_StaffEquip : IBossAttackPattern
{
    private GameObject _weaponVisualPrefab;

    public string PatternName => "StaffEquip";

    public LastBossPattern_StaffEquip(GameObject weaponVisualPrefab)
    {
        _weaponVisualPrefab = weaponVisualPrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _weaponVisualPrefab != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        if (boss is LastBoss lastBoss)
        {
            boss.StartCoroutine(SoundPlay());

            lastBoss.SetWeaponPrefab(_weaponVisualPrefab, 4f, true);

            if (lastBoss.CurrentWeapon != null)
            {
                // 회전 컴포넌트 부여
                var rotator = lastBoss.CurrentWeapon.GetComponent<RotateOverHead>();
                if (rotator == null)
                    rotator = lastBoss.CurrentWeapon.AddComponent<RotateOverHead>();

                rotator.rotationSpeed = 40f;
            }
        }

        yield return new WaitForSeconds(boss.Beat);
    }

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(0f);
        SoundManager.Instance.LastBossSoundClip("LastBossStaffModeActivate");
    }
}