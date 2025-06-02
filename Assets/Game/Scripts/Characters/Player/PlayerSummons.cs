using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 소환된 애들중 플레이어를 따라다니는 애들의 위치를 관리하는 용도입니다.
/// </summary>
public class PlayerSummons : MonoBehaviour
{
    //플레이어를 따라다니는 소환물들의 리스트입니다.
    private List<ISummon> _summonList = new();

    [SerializeField] private GameObject totemManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSummonPosition();
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
        int i = 1;
        foreach(ISummon summon in _summonList)
        {
            summon.SetPosition(transform,i);
            i++;
        }
    }



}
