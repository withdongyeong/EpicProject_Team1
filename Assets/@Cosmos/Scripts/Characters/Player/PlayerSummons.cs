using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환된 애들중 플레이어를 따라다니는 애들의 위치를 관리하는 용도입니다.
/// </summary>
public class PlayerSummons : MonoBehaviour
{
    //플레이어를 따라다니는 소환물들의 리스트입니다.
    private List<ISummon> _summonList = new();

    [SerializeField] private GameObject _totemManager;

    private bool isGameStarted;


    private void Awake()
    {
        EventBus.SubscribeGameStateChanged(GameStart);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var totemManager = Instantiate(_totemManager, transform);
        AddToList(totemManager.GetComponent<ISummon>());
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            UpdateSummonPosition();
        }        
    }

    public void AddToList(ISummon summon)
    {
        if(!_summonList.Contains(summon))
        {
            _summonList.Add(summon);
        }
        else
        {
            Debug.Log("이미 존재하는 소환물을 리스트에 다시 등록하려 했습니다");
        }
    }

    private void UpdateSummonPosition()
    {
        if (_summonList != null && _summonList.Count > 0)
        {
            int i = 1;
            foreach(ISummon summon in _summonList)
            {
                summon.SetPosition(transform.parent,i);
                i++;
            }    
        }
    }

    public void RemoveFromList(ISummon summon)
    {
        if(!_summonList.Contains(summon))
        {
            Debug.Log("소환물 리스트에 등록 안된 소환물을 리스트에서 제거하려고 했습니다.");
        }
        else
        {
            _summonList.Remove(summon);
        }
    }

    private void GameStart(GameState state)
    {
        if(state == GameState.Playing)
        {
            isGameStarted = true;
        }
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStateChanged(GameStart);
    }



}
