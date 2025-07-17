using System.Collections.Generic;
using UnityEngine;

public class BeetleSummonSkill : SkillBase
{
    private GameObject _beetlePrefab;

    private List<string> _adjacentSummonNameList = new();

    private PlayerProtection _playerProtection;

    protected override void Awake()
    {
        base.Awake();
        _beetlePrefab = Resources.Load<GameObject>("Prefabs/Summons/Beetle/Cat");
        EventBus.SubscribeGameStart(SpawnBeetle);
    }


    private void SpawnBeetle()
    {
        if (tileObject.IsPlaced)
        {
            Vector3 spawnPos = transform.TransformPoint(new Vector3(0, 1f));
            Quaternion rotate = transform.parent.rotation;
            Instantiate(_beetlePrefab, spawnPos, rotate);
            _playerProtection = FindAnyObjectByType<PlayerProtection>();
        }

    }

    protected override void Activate()
    {
        base.Activate();
        _playerProtection.SetProtection(true, 5 + _adjacentSummonNameList.Count * 5);
        // 인접 소환수 7개 이상이면 업적
        if (_adjacentSummonNameList.Count >= 7)
        {
            SteamAchievement.Achieve("ACH_CON_BEETLE");
        }
        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(SpawnBeetle);
    }

    public void AddSummonList(string summonTileName)
    {
        if(!_adjacentSummonNameList.Contains(summonTileName))
        {
            _adjacentSummonNameList.Add(summonTileName);
        }
    }
}
