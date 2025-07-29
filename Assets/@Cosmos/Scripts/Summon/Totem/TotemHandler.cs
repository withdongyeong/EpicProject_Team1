using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 토템을 발동시켰을때 넘겨줄 정보입니다
/// </summary>
public class TotemContext
{
    /// <summary>
    /// 몇번째로 발동하는지 입니다 시작은 0입니다.
    /// </summary>
    public int order = 0;

    public TotemContext Clone()
    {
        return new TotemContext
        {
            order = this.order
        };
    }
}

public class TotemHandler : MonoBehaviour
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
        //새로운 토템 상황을 생성합니다.
        TotemContext totemContext = new();

        //토템 리스트를 전부 돌면서 토템 상황을 넣어주고 발동준비를 시킵니다.
        for(int i=0; i<_currentTotemList.Count; i++)
        {
            _currentTotemList[i].ReadyToActive(totemContext);
            UpdateTotemContext(totemContext);
        }
        //이제 이 리스트는 안쓰는 리스트입니다.
        _currentTotemList.Clear();
    }

    /// <summary>
    /// 현재 발동된 토템 리스트에 있는 토템들을 싹 발동시킵니다 머리(가장 나중에 추가된 토템)는 더 좋게 발동시킵니다
    /// </summary>
    /// <returns></returns>
    private IEnumerator ActivateTotems()
    {
        //새로운 토템 상황을 생성합니다.
        TotemContext totemContext = new();

        //토템 리스트를 전부 돌면서 토템 상황을 넣어주고 발동준비를 시킵니다.
        for (int i = 0; i < _currentTotemList.Count; i++)
        {
            _currentTotemList[i].ReadyToActive(totemContext);
            UpdateTotemContext(totemContext);
            yield return new WaitForSeconds(0.15f);
        }
        //이제 이 리스트는 안쓰는 리스트입니다.
        _currentTotemList.Clear();
    }

    /// <summary>
    /// 발동된 토템 리스트에 토템 넣어주는 스크립트입니다 토템 한도에 다다르면 발동합니다
    /// </summary>
    /// <param name="totem">넣어줄 토템입니다</param>
    public void AddToTotemList(BaseTotem totem)
    {
        _currentTotemList.Add(totem);
        totem.transform.localPosition = GlobalSetting.Totem_Offset * (_currentTotemList.Count - 1);
        if(_currentTotemList.Count >= 3)
        {
            StartCoroutine(ActivateTotems());
            Debug.Log("토템발사!");
        }
    }

    private void UpdateTotemContext(TotemContext context)
    {
        context.order++;
    }

}
