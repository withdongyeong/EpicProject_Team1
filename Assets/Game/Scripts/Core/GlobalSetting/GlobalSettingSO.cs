using UnityEngine;

[CreateAssetMenu(fileName = "New Setting", menuName = "GlobalSetting/Setting")]
public class GlobalSettingSO : ScriptableObject
{
    [Header("타일 기본 속성")]
    public float basicDamage; //기준이 되는 데미지입니다
    [Header("소환 기믹 데이터")]
    public Vector2 summon_Offset; //소환물들 간의 간격입니다
    [Header("토템")]
    public Vector2 totem_Offset; //토템들 간의 간격입니다
}
