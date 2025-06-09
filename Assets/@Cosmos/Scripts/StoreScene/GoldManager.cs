using System;
using UnityEngine;

/// <summary>
/// 플레이어의 골드를 관리하는 매니저입니다
/// </summary>
public class GoldManager : Singleton<GoldManager>
{
    //현재 소유하고 있는 골드입니다
    private int _currentGold = 1000;

    /// <summary>
    /// 현재 소유하고 있는 골드입니다.
    /// </summary>
    public int CurrentGold => _currentGold;

    /// <summary>
    /// 소유 골드가 바뀔때를 받는 액션입니다. 들어오는 인자는 바뀐 후 보유한 골드 양입니다.
    /// </summary>
    public Action<int> OnCurrentGoldChange;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.D))
        {
            ModifyCurrentGold(1);
        }
    }

    /// <summary>
    /// 현재 소유한 골드를 넣어준 값'으로' 변경하는 메서드입니다
    /// </summary>
    /// <param name="gold">이값으로 현재 돈을 변경합니다</param>
    public void SetCurrentGold(int gold)
    {
        if(gold >= 0)
        {
            _currentGold = gold;
        }
        CallGoldChangeAction();
        
    }

    /// <summary>
    /// 현재 소유한 골드를 넣어준 값만큼 변경하는 메서드입니다.
    /// </summary>
    /// <param name="gold">이 값만큼 현재 골드가 변경됩니다. 1이면 +1입니다.</param>
    public void ModifyCurrentGold(int gold)
    {
        _currentGold = Mathf.Max(_currentGold + gold, 0);
        CallGoldChangeAction();
    }

    /// <summary>
    /// 현재 돈을 사용하는 메서드입니다. 무언가를 구매할때 호출하고, True면 돈을 쓴것, False면 돈을 못쓴것입니다. 인자는 1넣으면 1만큼 씁니다.
    /// </summary>
    /// <param name="gold">이만큼 사용합니다. 가격이 1이면 1을 넣으면 됩니다.</param>
    /// <returns>돈의 사용 성공 여부를 반환합니다.</returns>
    public bool UseCurrentGold(int gold)
    {
        //실수로 -1을 넣더라도 1로 바꿔서 계산하게 해줍니다.
        if(gold < 0)
        {
            gold *= -1;
        }

        if(_currentGold < gold)
        {
            Debug.Log("돈이 적어요");
            return false;
        }
        else
        {
            _currentGold -= gold;
            CallGoldChangeAction();
            return true;
        }

    }

    private void CallGoldChangeAction()
    {
        OnCurrentGoldChange?.Invoke(_currentGold);
    }
}
