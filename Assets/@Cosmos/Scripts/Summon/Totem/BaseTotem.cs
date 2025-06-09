using UnityEngine;

public abstract class BaseTotem : MonoBehaviour
{
    /// <summary>
    /// 토템의 성능을 정하는 기준입니다. 힐 토템이든 딜 토템이든 이걸 올리면 기본적으로 더 많은 딜, 더 많은 힐을 제공합니다.
    /// </summary>
    protected int _totemPower;

    /// <summary>
    /// 토템의 애니메이션을 재생해주는 애니메이터입니다
    /// </summary>
    protected Animator _anim;

    /// <summary>
    /// 이 토템이 몇번째로 발동한건지 같은 정보를 담은 변수입니다.
    /// </summary>
    protected TotemContext _context;

    /// <summary>
    /// Start() 역할이자 토템의 능력치를 정해주는 메서드입니다. base에 적혀있는 내용은 토템 메니저에 등록하는 겁니다. 보통 능력치 다 전달 하고 마지막에 base를 실행시켜주세요
    /// </summary>
    /// <param name="totemPower">부모의 토템파워를 전해주면 적용합니다.</param>
    public virtual void InitializeTotem(int totemPower)
    {
        _totemPower = totemPower;
        _anim = GetComponent<Animator>();
        FindAnyObjectByType<TotemManager>().AddToTotemList(this);
    }

    public virtual void ReadyToActive(TotemContext context)
    {
        _context = context;
        GotoActivePos(_context.order);
        _anim.SetTrigger("Active");
    }

    protected virtual void GotoActivePos(int order)
    {
        transform.parent = null;
        transform.position = GlobalSetting.Totem_ActivatePos + GlobalSetting.Totem_Offset * order;
    }

    protected virtual void AnimationEnd()
    {
        //3번째부터가 머리다! 면 순서는 0부터 시작이라 1 빼줍니다.
        if(_context.order < GlobalSetting.Totem_HeadInt - 1)
        {
            ActivateTotem(_context);
        }
        else
        {
            ActivateTotemBetter(_context);
        }
        DestroyTotem();
    }

    /// <summary>
    /// 토템의 능력을 발동시키는 메서드입니다
    /// </summary>
    protected virtual void ActivateTotem(TotemContext context)
    {
        
        
    }

    /// <summary>
    /// 더 좋은 능력을 발동시키는 메서드입니다. 안건드리면 두번 발동합니다
    /// </summary>
    protected virtual void ActivateTotemBetter(TotemContext context)
    {
        ActivateTotem(context);
        ActivateTotem(context);
    }

    protected virtual void DestroyTotem()
    {
        Destroy(gameObject);
    }

    

    
}
