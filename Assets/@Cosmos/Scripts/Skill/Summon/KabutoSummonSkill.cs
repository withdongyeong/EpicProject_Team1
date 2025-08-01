using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KabutoSummonSkill : SkillBase
{
    private GameObject _kabutoPrefab;
    private Kabuto_RE _currentKabuto;

    private List<string> _adjacentSummonNameList = new();

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(SpawnKabuto);
        _kabutoPrefab = Resources.Load<GameObject>("Prefabs/Summons/Kabuto/Kabuto");

    }

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void ClearStarBuff()
    {
        base.ClearStarBuff();
        _adjacentSummonNameList.Clear();
    }

    protected override void Activate()
    {
        base.Activate();
        _currentKabuto.Fire();
    }

    private void SpawnKabuto()
    {
        if(tileObject.IsPlaced)
        {
            Quaternion rotate = transform.parent.rotation;
            Vector3 spawnPos = transform.TransformPoint(Vector3.up + Vector3.right);
            _currentKabuto = Instantiate(_kabutoPrefab, spawnPos, rotate).GetComponent<Kabuto_RE>();
            if(_adjacentSummonNameList.Count >=3)
            {
                _currentKabuto.Init(this,true);
            }
            else
            {
                _currentKabuto.Init(this, false);
            }
            
        }
        
    }

    public void AddSummonList(string tileName)
    {
        if (!_adjacentSummonNameList.Contains(tileName))
        {
            _adjacentSummonNameList.Add(tileName);
        }
        if(_adjacentSummonNameList.Count >=3)
        {
            if(_currentKabuto != null)
            {
                _currentKabuto.IsHyper = true;
            }           
        }
    }

    public void DestoryKabuto()
    {
        _currentKabuto = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(SpawnKabuto);
    }


}
