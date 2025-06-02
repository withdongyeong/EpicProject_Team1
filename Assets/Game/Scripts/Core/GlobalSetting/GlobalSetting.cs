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

}
