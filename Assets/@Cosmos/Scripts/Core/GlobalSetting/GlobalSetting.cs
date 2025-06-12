using UnityEngine;

/// <summary>
/// 글로벌 세팅 클래스입니다 하나만 존재해야합니다
/// </summary>
public class GlobalSetting : MonoBehaviour
{
    [SerializeField] private GlobalSettingSO loadThis;

    private static GlobalSettingSO _instance;

    private void Awake()
    {
        Load(loadThis);
    }

    public static void Load(GlobalSettingSO setting)
    {
        _instance = setting;
    }

    public static Vector2 Summon_Offset => _instance.summon_Offset;

    public static Vector2 Totem_Offset => _instance.totem_Offset;

    public static Vector2 Totem_ActivatePos => _instance.totem_ActivatePos;

    public static int Totem_HeadInt => _instance.totem_HeadInt;

    /// <summary>
    /// 노말 등급이 뜰 확률입니다
    /// </summary>
    public static float Shop_NormalChance => _instance.shop_NormalChance;

    /// <summary>
    /// 레어 등급이 뜰 확률입니다
    /// </summary>
    public static float Shop_RareChance => _instance.shop_RareChance;

    /// <summary>
    /// 에픽 등급이 뜰 확률입니다
    /// </summary>
    public static float Shop_EpicChance => _instance.shop_EpicChance;

    /// <summary>
    /// 레전더리 등급이 뜰 확률입니다
    /// </summary>
    public static float Shop_LegendaryChance => _instance.shop_LegendaryChance;

    /// <summary>
    /// 시너지(#화염)의 언어를 결정하는 SO입니다
    /// </summary>
    public static SynergyTranslationSO Language_SynergyTranslationSO => _instance.language_SynergyTranslationSO;
}
