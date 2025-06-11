using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지속 피격 판정을 위한 데이터 클래스
/// </summary>
public class PersistentDamageZone
{
    private List<Vector3Int> gridPositions;
    private int damage;
    private float remainingTime;
    private HashSet<PlayerController> hitPlayers;
    
    public List<Vector3Int> GridPositions { get => gridPositions; }
    public int Damage { get => damage; }
    public float RemainingTime { get => remainingTime; set => remainingTime = value; }
    public HashSet<PlayerController> HitPlayers { get => hitPlayers; }
    
    /// <summary>
    /// 지속 데미지 존 생성자
    /// </summary>
    /// <param name="positions">영향을 받는 격자 위치들</param>
    /// <param name="dmg">적용할 데미지</param>
    /// <param name="duration">지속 시간</param>
    public PersistentDamageZone(List<Vector3Int> positions, int dmg, float duration)
    {
        gridPositions = positions;
        damage = dmg;
        remainingTime = duration;
        hitPlayers = new HashSet<PlayerController>();
    }
}