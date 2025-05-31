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
