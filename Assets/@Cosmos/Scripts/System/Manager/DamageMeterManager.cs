using System.Collections.Generic;
using UnityEngine;

public class DamageMeterManager : Singleton<DamageMeterManager>
{
    private Dictionary<string, int> _damageRecords = new Dictionary<string, int>();
    public Dictionary<string, int> DamageRecords => _damageRecords;


    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(ClearDictionary);
    }
    public void AddDamage(string tileName, int damage)
    {
        if(_damageRecords.ContainsKey(tileName))
        {
            _damageRecords[tileName] += damage;
        }
        else
        {
            _damageRecords.Add(tileName, damage);
        }
    }

    private void ClearDictionary()
    {
        _damageRecords.Clear();
    }

    private void ShowDictionary()
    {
        foreach (KeyValuePair<string, int> entry in _damageRecords)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            ShowDictionary();
        }
    }
    //TODO: 만약 테이크데미지 문제 생기면 에너미 프로젝타일 <- 이게 문제일듯

    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(ClearDictionary);
    }
}
