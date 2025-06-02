using UnityEngine;

/// <summary>
/// 소환물,그중에서도 특히 플레이어 옆에 따라다니는 소환물들이 위치를 고정하기 위해 가지는 인터페이스입니다
/// </summary>
public interface ISummon
{

    public void SetPosition(Transform playerTransform, int i);
}
