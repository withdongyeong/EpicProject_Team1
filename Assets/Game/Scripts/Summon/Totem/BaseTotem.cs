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
    /// Start() 역할이자 토템의 능력치를 정해주는 메서드입니다. base에 적혀있는 내용은 토템 메니저에 등록하는 겁니다. 보통 능력치 다 전달 하고 마지막에 base를 실행시켜주세요
    /// </summary>
    /// <param name="itemData">아이템 데이터를 바탕으로 자기가 해석해서 적용하게 합니다 부모의 아이템 데이터를 전해주세요</param>
    public virtual void InitializeTotem(InventoryItemData itemData)
    {
        FindAnyObjectByType<TotemManager>().AddToTotemList(this);
    }

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

    public virtual void DestroyTotem()
    {
        Destroy(gameObject);
    }
}
