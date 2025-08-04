using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class DamageMeterManager : Singleton<DamageMeterManager>
{
    private Dictionary<string, int> _damageRecords = new Dictionary<string, int>();
    public Dictionary<string, int> DamageRecords => _damageRecords;
    private DamageMeterHandler _damageMeterHandler;

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

    public void SetDamageMeterPanel()
    {
        _damageMeterHandler = FindAnyObjectByType<DamageMeterHandler>();
        if (_damageMeterHandler == null)
        {
            Debug.LogError("DamageMeterHandler not found in the scene.");
            return;
        }

        _damageMeterHandler.ClearDamageText(); // 초기화 시 모든 자식 오브젝트 삭제

        // value 값이 높은 순으로 정렬
        foreach (var entry in SortDamageRecordsByValueDesc())
        {
            TextMeshProUGUI nameText = new TextMeshProUGUI();

            LocalizedString localizedString_Name = new LocalizedString("EpicProject_Table", "Tile_TileName_" + entry.Key);
            localizedString_Name.StringChanged += (text) => { nameText.text = text; };
            _damageMeterHandler.AddDamageText($"{nameText.text}: {entry.Value}");
        }
    }

    private IEnumerable<KeyValuePair<string, int>> SortDamageRecordsByValueDesc()
    {
        // value 기준 내림차순 정렬
        foreach (var entry in _damageRecords.OrderByDescending(x => x.Value))
        {
            yield return entry;
        }
    }


    //디버그용
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
