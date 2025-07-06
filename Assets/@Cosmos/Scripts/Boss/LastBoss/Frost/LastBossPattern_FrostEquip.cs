using System.Collections;
using UnityEngine;

/// <summary>
/// 최종보스 - FrostEquip: 얼음 무기만 띄움 (공격 없음)
/// </summary>
public class LastBossPattern_FrostEquip : IBossAttackPattern
{
    private GameObject _weaponVisualPrefab;
    public string PatternName => "FrostEquip";

    public LastBossPattern_FrostEquip(GameObject weaponVisualPrefab)
    {
        _weaponVisualPrefab = weaponVisualPrefab;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _weaponVisualPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        if (boss is LastBoss lastBoss)
        {
            lastBoss.SetWeaponPrefab(_weaponVisualPrefab, 4f, false);
            if (lastBoss.CurrentWeapon != null)
            {
                boss.StartCoroutine(SoundPlay());

                var rotator = lastBoss.CurrentWeapon.GetComponent<RotateOverHead>();
                if (rotator == null)
                    rotator = lastBoss.CurrentWeapon.AddComponent<RotateOverHead>();
                rotator.rotationSpeed = 40f;
            }
        }

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(0f);
        SoundManager.Instance.LastBossSoundClip("LastBossFrostModeActivate");
    }
}