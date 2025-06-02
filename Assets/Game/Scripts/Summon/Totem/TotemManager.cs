using System.Collections.Generic;
using UnityEngine;

public class TotemManager : SummonBase
{
    //현재 활성화된 토템 리스트입니다
    private List<BaseTotem> _currentTotemList = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 현재 발동된 토템 리스트에 있는 토템들을 싹 발동시킵니다 머리(가장 나중에 추가된 토템)는 더 좋게 발동시킵니다
    /// </summary>
    private void ActivateTotemList()
    {
        for(int i=0; i<_currentTotemList.Count; i++)
        {
            if(i<_currentTotemList.Count - 1)
            {
                _currentTotemList[i].ActivateTotem();
            }
            else
            {
                Debug.Log("else 들어옴");
                _currentTotemList[i].ActivateTotemBetter();
            }
            _currentTotemList[i].DestroyTotem();
        }
        _currentTotemList.Clear();
    }

    /// <summary>
    /// 발동된 토템 리스트에 토템 넣어주는 스크립트입니다 토템 한도에 다다르면 발동합니다
    /// </summary>
    /// <param name="totem">넣어줄 토템입니다</param>
    public void AddToTotemList(BaseTotem totem)
    {
        _currentTotemList.Add(totem);
        if(_currentTotemList.Count >= 3)
        {
            ActivateTotemList();
            Debug.Log("토템발사!");
        }
    }

}
