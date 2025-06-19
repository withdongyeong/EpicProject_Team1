using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SkillBase : MonoBehaviour
{

    //private SkillUseManager sm;
    [Header("Skill Info")] 
    public string skillName;
    public float cooldown = 5f;

    //쿨다운 계수입니다.
    private float cooldownFactor;
    //계수를 적용한 최종 쿨다운 입니다.
    private float finalCooldown;

    private float lastUsedTime = -Mathf.Infinity;

    private Material _coolTimeMaterial;

    //타일 오브젝트입니다.
    protected TileObject tileObject;

    /// <summary>
    /// 적용받고 있는 인접효과 리스트입니다.
    /// </summary>
    protected List<StarBase> starList;

    /// <summary>
    /// Activate때 발동시킬 인접효과 함수들의 액션입니다.
    /// </summary>
    protected Action<TileObject> onActivateAction;




    protected virtual void Awake()
    {
        //'전투가 시작될때' 타이밍입니다.
        EventBus.SubscribeGameStart(InitPassiveStarList);
    }

    protected virtual void Start()
    {
        //sm = SkillUseManager.Instance;
        if (TryGetComponent<CombineCell>(out CombineCell combineCell))
        {
            _coolTimeMaterial = combineCell.GetSprite().material;
            _coolTimeMaterial.SetFloat("_WorldSpaceHeight", combineCell.GetSprite().bounds.size.y);
            _coolTimeMaterial.SetFloat("_WorldSpaceBottomY", combineCell.GetSprite().localBounds.min.y);
            tileObject = combineCell.GetTileObject();
        }

    }

    private void LateUpdate()
    {
        _coolTimeMaterial.SetFloat("_FillAmount", 1 - (GetCooldownRemaining() / finalCooldown));
    }

    /// <summary>
    /// 쿨타임 중인지 여부 확인
    /// </summary>
    public bool IsOnCooldown => Time.time < lastUsedTime + finalCooldown;

    /// <summary>
    /// 스킬 발동 시도. 쿨타임을 체크하고 성공 시 Activate 호출.
    /// </summary>
    public bool TryActivate()
    {
        if (IsOnCooldown)
        {
            return false;
        }

        Activate();

        //cooldown = defaultCooldown * sm.CooldownFactor;
        //for(int i =0; i < sm.SkillActivationCount; i++)
        //{
        //    Debug.Log("sm.SkillActivationCount: " + sm.SkillActivationCount);
        //    Activate(user);
        //}
        lastUsedTime = Time.time;
        return true;
    }

    /// <summary>
    /// 자식 클래스에서 반드시 구현해야 하는 스킬 효과
    /// </summary>
    protected virtual void Activate()
    {
        SoundManager.Instance.PlayTileSoundClip(GetType().Name + "Activate");

        if(onActivateAction == null)
        {

        }
        // 타일 발동시 발동시킬 인접 효과의 액션 리스트를 발동시킵니다.
        onActivateAction?.Invoke(tileObject);
        
        
    }

    /// <summary>
    /// 남은 쿨타임 반환
    /// </summary>
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, (lastUsedTime + finalCooldown) - Time.time);
    }


    /// <summary>
    /// 현재 적용되는 인접 효과를 업데이트하는 함수
    /// </summary>
    /// <param name="starBases"></param>
    public void UpdateStarList(List<StarBase> starBases)
    {
        starList = starBases;
    }

    /// <summary>
    /// 현재 적용받고 있는 인접효과중에서, 게임이 시작되었을때 적용받을 효과를 적용받는 메서드입니다. override하신다음에 개조하시면 됩니다.
    /// </summary>
    protected virtual void InitPassiveStarList()
    {
        //먼저 저번 게임의 인접 효과가 적용되지 않게 하도록, 초기화합니다.
        ClearStarBuff();
        if(starList != null)
        {
            foreach (StarBase star in starList)
            {
                StarBuff starBuff = star.StarBuff;
                finalCooldown *= (1-star.CooldownFactor);
                starBuff.Action_OnActivate.Invoke(tileObject);
                onActivateAction += starBuff.Action_OnActivate;
            }
        }
        
    }

    /// <summary>
    /// 현재 적용된 인접 효과들을 전부 제거합니다.
    /// </summary>
    protected virtual void ClearStarBuff()
    {
        finalCooldown = cooldown;
        onActivateAction = null;
    }

    protected virtual void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(InitPassiveStarList);
    }
}