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
    public Vector2 totem_ActivatePos; //토템이 발동하는 위치입니다.
    /// <summary>
    /// 토템의 머리는 몇번째부터인지 정하는 변수입니다.
    /// </summary>
    public int totem_HeadInt;

    [Header("상점 등급 확률표")]
    public float shop_NormalChance; //노말 등급의 확률입니다
    public float shop_RareChance; //레어 등급의 확률입니다
    public float shop_EpicChance; //에픽 등급의 확률입니다
    public float shop_LegendaryChance; //레전더리 등급의 확률입니다

    [Header("언어")]
    public SynergyTranslationSO language_SynergyTranslationSO; // 시너지(#화염)의 언어를 결정하는 SO입니다
}
