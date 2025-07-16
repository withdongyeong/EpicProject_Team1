using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class SkillBase : MonoBehaviour
{

    //private SkillUseManager sm;
    [Header("Skill Info")]
    protected float cooldown = 5f;

    [Header("Animation Settings")]
    private float pulseScale = 1.5f; // 펄스 시 확대 배율
    private float pulseDuration = 0.2f; // 펄스 애니메이션 지속시간
    private Vector3 originalScale;

    [Header("발동 이펙트")]
    private GameObject activateEffectPrefab;
    private float effectDuration = 1.3f;


    //쿨다운 계수입니다.
    private float cooldownFactor;
    //계수를 적용한 최종 쿨다운 입니다.
    private float finalCooldown;

    private float lastUsedTime = -Mathf.Infinity;

    //타일 오브젝트입니다.
    protected TileObject tileObject;
    
    private CombineCell combineCell;

    /// <summary>
    /// 스킬의 타일 오브젝트입니다.
    /// </summary>
    public TileObject TileObject => tileObject;

    /// <summary>
    /// 적용받고 있는 인접효과 리스트입니다.
    /// </summary>
    protected List<StarBase> starList;


    /// <summary>
    /// 게임 시작시 발동시킬 인접효과 함수들의 액션입니다.
    /// </summary>
    protected Action<SkillBase> onGameStartAction;

    /// <summary>
    /// Activate때 발동시킬 인접효과 함수들의 액션입니다.
    /// </summary>
    protected Action<SkillBase> onActivateAction;

    
    protected List<GameObject> _lightList = new();
    protected SpriteRenderer _sr;


    protected virtual void Awake()
    {
        // '전투씬이 시작될 때' 타이밍입니다.(보스x, 플레이어x)
        EventBus.SubscribeSceneLoaded(InitClearStarList);
        //'전투가 시작될때' 타이밍입니다.
        EventBus.SubscribeGameStart(InitPassiveStarList);
        //'상점 진입시' 타이밍입니다.
        EventBus.SubscribeSceneLoaded(ResetCoolDown);

        combineCell = GetComponent<CombineCell>();
        _sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        for(int i =0; i<_sr.transform.childCount; i++)
        {
            _lightList.Add(_sr.transform.GetChild(i).gameObject);
        }
    }

    protected virtual void Start()
    {
        //sm = SkillUseManager.Instance;
        if (TryGetComponent<CombineCell>(out CombineCell combineCell))
        {
            tileObject = combineCell.GetTileObject();
            cooldown = tileObject.GetTileData().TileCoolTime;
            originalScale = combineCell.GetSprite().transform.localScale;
        }
        
        // 발동 프리팹 할당
        activateEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ActivateEffect");

    }

    protected virtual void LateUpdate()
    {     
        ApplyCoolDownToImage();
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

        combineCell.CoolDownEffectActivate(finalCooldown);
        
        // 펄스 애니메이션 실행
        StartCoroutine(PulseAnimation());

        // 발동 이펙트 0.1초 지연 소환
        StartCoroutine(SpawnEffectDelayed());
        
        // 타일 발동시 발동시킬 인접 효과의 액션 리스트를 발동시킵니다.
        onActivateAction?.Invoke(this);
    }

    /// <summary>
    /// 타일 오브젝트의 스케일을 펄스 애니메이션으로 변경
    /// </summary>
    private IEnumerator PulseAnimation()
    {
        if (tileObject == null) yield break;

        Transform tileTransform = combineCell.GetSprite().transform;
        Vector3 targetScale = originalScale * pulseScale;

        float elapsedTime = 0f;
        float halfDuration = pulseDuration * 0.5f;

        // 확대 애니메이션 (0.5배 시간)
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            
            // 부드러운 곡선을 위한 Ease Out
            t = 1f - Mathf.Pow(1f - t, 2f);
            
            tileTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsedTime = 0f;

        // 축소 애니메이션 (0.5배 시간)
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            
            // 부드러운 곡선을 위한 Ease In
            t = Mathf.Pow(t, 2f);
            
            tileTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // 정확히 원래 스케일로 복원
        tileTransform.localScale = originalScale;
    }

    /// <summary>
    /// 발동 이펙트
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEffectDelayed()
    {
        yield return new WaitForSeconds(0.1f);

        if (activateEffectPrefab != null)
        {
            var playerPosition = FindAnyObjectByType<PlayerMarker>();
            if (playerPosition != null)
            {
                GameObject effect = Instantiate(activateEffectPrefab, playerPosition.transform.position, Quaternion.identity);
                Destroy(effect, effectDuration);
            }
        }
    }

    
    /// <summary>
    /// 남은 쿨타임 반환
    /// </summary>
    public virtual float GetCooldownRemaining()
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
    /// 씬이 로드될 때, 인접 효과를 초기화하는 메서드입니다. override하신다음에 개조하시면 됩니다.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    protected virtual void InitClearStarList(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage())
        {
            // 전투씬 시작시 인접 효과를 초기화합니다.
            ClearStarBuff();
        }
        
        
    }

    /// <summary>
    /// 현재 적용받고 있는 인접효과중에서, 게임이 시작되었을때 적용받을 효과를 적용받는 메서드입니다. override하신다음에 개조하시면 됩니다.
    /// </summary>
    protected virtual void InitPassiveStarList()
    {
        if(starList != null)
        {
            foreach (StarBase star in starList)
            {
                StarBuff starBuff = star.StarBuff;
                onGameStartAction += starBuff.Action_OnGameStart;
                onActivateAction += starBuff.Action_OnActivate;
            }
        }
        onGameStartAction?.Invoke(this);
    }

    /// <summary>
    /// 주어진 스탯 종류와 버프 양에 따라 자기 자신에게 버프를 적용하는 로직입니다.
    /// </summary>
    /// <param name="buffData">주어진 버프 데이터입니다. .TileStat은 버프할 스탯의 종류를, .Value는 버프할 양입니다.</param>
    public virtual void ApplyStatBuff(TileBuffData buffData)
    {
        if (buffData.TileStat == BuffableTileStat.CoolTime)
            finalCooldown *= (1-buffData.Value);
        
    }

    /// <summary>
    /// 현재 적용된 인접 효과들을 전부 제거합니다.
    /// </summary>
    protected virtual void ClearStarBuff()
    {
        finalCooldown = cooldown;
        onGameStartAction = null;
        onActivateAction = null;
    }

    protected virtual void ResetCoolDown(Scene scene, LoadSceneMode mode)
    {
        if(SceneLoader.IsInBuilding())
        {
            lastUsedTime = -Mathf.Infinity;
        }   
    }

    protected virtual void ApplyCoolDownToImage()
    {
        if(IsOnCooldown)
        {
            _sr.color = Color.gray;
            foreach (GameObject light in _lightList)
            {
                light.SetActive(false);
            }
        }
        else
        {
            _sr.color = Color.white;
            foreach (GameObject light in _lightList)
            {
                light.SetActive(true);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(InitClearStarList);
        EventBus.UnsubscribeGameStart(InitPassiveStarList);
        EventBus.UnsubscribeSceneLoaded(ResetCoolDown);
    }

}