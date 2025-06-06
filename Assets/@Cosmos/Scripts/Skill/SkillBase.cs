using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    
    [Header("Skill Info")] 
    private SkillState _currentState = SkillState.Charging;
    public string skillName;
    public float cooldown = 5f;
    private float lastUsedTime = -Mathf.Infinity;

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

        Activate(user);
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