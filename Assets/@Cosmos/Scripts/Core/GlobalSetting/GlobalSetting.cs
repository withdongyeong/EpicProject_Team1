using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 글로벌 세팅 클래스입니다 하나만 존재해야합니다
/// </summary>
public class GlobalSetting : Singleton<GlobalSetting>
{

    private static GlobalSettingSO _instance;

    
    protected override void Awake()
    {
        base.Awake();
        _instance = Resources.Load<GlobalSettingSO>("TestSetting");
    }
    public static Vector2 Summon_Offset => _instance.summon_Offset;

    public static Vector2 Totem_Offset => _instance.totem_Offset;

    public static Vector2 Totem_ActivatePos => _instance.totem_ActivatePos;

    public static int Totem_HeadInt => _instance.totem_HeadInt;

    /// <summary>
    /// 확률 리스트에 접근합니다.
    /// </summary>
    public static List<ShopChanceClass> Shop_ChanceList => _instance.shop_ChanceList;

    ///// <summary>
    ///// 노말 등급이 뜰 확률입니다
    ///// </summary>
    //public static float Shop_NormalChance => _instance.shop_NormalChance;

    ///// <summary>
    ///// 레어 등급이 뜰 확률입니다
    ///// </summary>
    //public static float Shop_RareChance => _instance.shop_RareChance;

    ///// <summary>
    ///// 에픽 등급이 뜰 확률입니다
    ///// </summary>
    //public static float Shop_EpicChance => _instance.shop_EpicChance;

    ///// <summary>
    ///// 레전더리 등급이 뜰 확률입니다
    ///// </summary>
    //public static float Shop_LegendaryChance => _instance.shop_LegendaryChance;

    /// <summary>
    /// 시너지(#화염)의 언어를 결정하는 SO입니다
    /// </summary>
    public static SynergyTranslationSO Language_SynergyTranslationSO => _instance.language_SynergyTranslationSO;
}
