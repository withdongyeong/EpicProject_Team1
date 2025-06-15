using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{

    private SkillUseManager sm;
    [Header("Skill Info")] 
    public string skillName;
    public float cooldown = 5f;
    private float defaultCooldown;
    private float lastUsedTime = -Mathf.Infinity;

    private Material _coolTimeMaterial;
    
    private void Start()
    {
        defaultCooldown = cooldown;
        sm = SkillUseManager.Instance;
        if(TryGetComponent<CombineCell>(out CombineCell combineCell))
        {
            _coolTimeMaterial = combineCell.GetSprite().material;
            _coolTimeMaterial.SetFloat("_WorldSpaceHeight", combineCell.GetSprite().bounds.size.y);
            _coolTimeMaterial.SetFloat("_WorldSpaceBottomY", combineCell.GetSprite().bounds.min.y);
        }
    }

    private void LateUpdate()
    {
        _coolTimeMaterial.SetFloat("_FillAmount", 1 - (GetCooldownRemaining() / cooldown));
    }

    /// <summary>
    /// 쿨타임 중인지 여부 확인
    /// </summary>
    public bool IsOnCooldown => Time.time < lastUsedTime + cooldown;

    /// <summary>
    /// 스킬 발동 시도. 쿨타임을 체크하고 성공 시 Activate 호출.
    /// </summary>
    public bool TryActivate(GameObject user)
    {
        if (IsOnCooldown)
        {
            return false;
        }

        cooldown = defaultCooldown * sm.CooldownFactor;
        for(int i =0; i < sm.SkillActivationCount; i++)
        {
            Debug.Log("sm.SkillActivationCount: " + sm.SkillActivationCount);
            Activate(user);
        }
        lastUsedTime = Time.time;
        return true;
    }

    /// <summary>
    /// 자식 클래스에서 반드시 구현해야 하는 스킬 효과
    /// </summary>
    protected virtual void Activate(GameObject user)
    {
        SoundManager.Instance.PlayTileSoundClip(GetType().Name + "Activate");
    }

    /// <summary>
    /// 남은 쿨타임 반환
    /// </summary>
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, (lastUsedTime + cooldown) - Time.time);
    }
}