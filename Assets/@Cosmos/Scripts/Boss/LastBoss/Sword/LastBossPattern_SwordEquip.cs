using System.Collections;
using UnityEngine;

/// <summary>
/// 최종보스 - SwordEquip: 검 무기만 띄움 (공격 없음)
/// </summary>
public class LastBossPattern_SwordEquip : IBossAttackPattern
{
    private GameObject _weaponVisualPrefab;
    public string PatternName => "SwordEquip";

    public LastBossPattern_SwordEquip(GameObject weaponVisualPrefab)
    {
        _weaponVisualPrefab = weaponVisualPrefab;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _weaponVisualPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        if (boss is LastBoss lastBoss)
        {
            lastBoss.SetWeaponPrefab(_weaponVisualPrefab, 3f, true);
            if (lastBoss.CurrentWeapon != null)
            {
                boss.StartCoroutine(SoundPlay());

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
        SoundManager.Instance.LastBossSoundClip("LastBossSwordModeActivate");
    }
}