using UnityEngine;

[CreateAssetMenu(fileName = "New Setting", menuName = "GlobalSetting/Setting")]
public class GlobalSettingSO : ScriptableObject
{
    [Header("타일 기본 속성")]
    public float basicDamage;
    [Header("소환 기믹 데이터")]
    public Vector2 summon_Offset;
}
