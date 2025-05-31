using UnityEngine;

public abstract class BaseTotem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 토템의 능력치를 정해주는 메서드입니다
    /// </summary>
    /// <param name="itemData">아이템 데이터를 바탕으로 자기가 해석해서 적용하게 합니다</param>
    public abstract void InitializeTotem(InventoryItemData itemData);

    /// <summary>
    /// 토템의 능력을 발동시키는 메서드입니다
    /// </summary>
    public virtual void ActivateTotem()
    {

    }

    /// <summary>
    /// 더 좋은 능력을 발동시키는 메서드입니다. 안건드리면 두번 발동합니다
    /// </summary>
    public virtual void ActivateTotemBetter()
    {
        ActivateTotem();
        ActivateTotem();
    }
}
