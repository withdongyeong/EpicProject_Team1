using UnityEngine;

/// <summary>
/// 플레이어를 따라다니는 소환물들이 상속해야하는 스크립트입니다 
/// </summary>
public abstract class SummonBase : MonoBehaviour, ISummon
{
    /// <summary>
    /// 소환물이 플레이어 기준 어디에 위치할지 정하는 함수입니다 매 Update마다 호출됩니다
    /// </summary>
    /// <param name="playerTransform">플레이어의 Transform입니다</param>
    /// <param name="i">얘가 몇번째인지 받습니다. 시작은 1부터입니다.</param>
    public virtual void SetPosition(Transform playerTransform, int i)
    {
        transform.position = (Vector2)playerTransform.position + new Vector2(GlobalSetting.Summon_Offset.x * i, GlobalSetting.Summon_Offset.y);

    }

    /// <summary>
    /// 소환되었을때 Start에서 실행시키고 싶은걸 넣어놓는 함수입니다
    /// </summary>

    protected virtual void WhenStart()
    {
        FindAnyObjectByType<PlayerSummons>().AddToList(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WhenStart();
    }

    private void OnDestroy()
    {
        FindAnyObjectByType<PlayerSummons>().RemoveFromList(this);
    }

}
