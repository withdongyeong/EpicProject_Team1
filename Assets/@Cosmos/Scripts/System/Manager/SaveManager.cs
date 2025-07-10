using UnityEngine;

public static class SaveManager
{
    
    // 🔒 캐싱 변수들
    public static int UnlockLevel { get; private set; }
    public static int IsTutorialCompleted { get; private set; }
    public static bool IsFullScreen { get; private set; }
    public static string Resolution { get; private set; }
    public static float MasterVolume { get; private set; } 
    public static float BgmVolume { get; private set; }
    public static float SfxVolume { get; private set; }

    // ✅ 처음 로드시 호출
    public static void LoadAll()
    {
        UnlockLevel = PlayerPrefs.GetInt(SaveKeys.UnlockLevel, 0);
        IsTutorialCompleted = PlayerPrefs.GetInt(SaveKeys.IsTutorialCompleted, 0);
        IsFullScreen = PlayerPrefs.GetInt(SaveKeys.IsFullScreen, 1) == 1; // 1이면 true, 0이면 false
        Resolution = PlayerPrefs.GetString(SaveKeys.Resolution, "1920x1080");
        MasterVolume = PlayerPrefs.GetFloat(SaveKeys.MasterVolume, 1.0f);
        BgmVolume = PlayerPrefs.GetFloat(SaveKeys.BgmVolume, 1.0f);
        SfxVolume = PlayerPrefs.GetFloat(SaveKeys.SfxVolume, 1.0f);
    }

    public static void SaveAll()
    {
        PlayerPrefs.SetInt(SaveKeys.UnlockLevel, UnlockLevel);
        PlayerPrefs.SetInt(SaveKeys.IsTutorialCompleted, IsTutorialCompleted);
        PlayerPrefs.SetInt(SaveKeys.IsFullScreen, IsFullScreen? 1 : 0);
        PlayerPrefs.SetString(SaveKeys.Resolution, Resolution);
        PlayerPrefs.SetFloat(SaveKeys.MasterVolume, MasterVolume);
        PlayerPrefs.SetFloat(SaveKeys.BgmVolume, BgmVolume);
        PlayerPrefs.SetFloat(SaveKeys.SfxVolume, SfxVolume);
        
        PlayerPrefs.Save(); // 변경 사항 저장
    }
    
    // ✅ 저장시엔 PlayerPrefs 와 변수 둘 다 갱신
    public static void SaveUnlockLevel(int level)
    {
        //해금 레벨은 낮아지지 않습니다.
        if(level > UnlockLevel)
        {
            UnlockLevel = level;
            PlayerPrefs.SetInt(SaveKeys.UnlockLevel, level);
            PlayerPrefs.Save();
        }
        
    }

    public static void SaveIsTutorialCompleted(int isCompleted)
    {
        IsTutorialCompleted = isCompleted;
        PlayerPrefs.SetInt(SaveKeys.IsTutorialCompleted, isCompleted);
        PlayerPrefs.Save();
    }

    public static void SaveIsFullScreen(bool isFullScreen)
    {
        IsFullScreen = isFullScreen;
        PlayerPrefs.SetInt(SaveKeys.IsFullScreen, isFullScreen? 1:0);
        PlayerPrefs.Save();
    }

    public static void SaveResolution(string resolution)
    {
        Resolution = resolution;
        PlayerPrefs.SetString(SaveKeys.Resolution, resolution);
        PlayerPrefs.Save();
    }

    public static void SaveMasterVolume(float volume)
    {
        MasterVolume = volume;
        PlayerPrefs.SetFloat(SaveKeys.MasterVolume, volume);
        PlayerPrefs.Save();
    }
    
    public static void SaveBgmVolume(float volume)
    {
        BgmVolume = volume;
        PlayerPrefs.SetFloat(SaveKeys.BgmVolume, volume);
        PlayerPrefs.Save();
    }

    public static void SaveSfxVolume(float volume)
    {
        SfxVolume = volume;
        PlayerPrefs.SetFloat(SaveKeys.SfxVolume, volume);
        PlayerPrefs.Save();
    }
    public static void DeleteAllSaves()
    {
        PlayerPrefs.DeleteKey(SaveKeys.UnlockLevel);
        PlayerPrefs.DeleteKey(SaveKeys.IsTutorialCompleted);
        PlayerPrefs.DeleteKey(SaveKeys.IsFullScreen);
        PlayerPrefs.DeleteKey(SaveKeys.Resolution);
        PlayerPrefs.DeleteKey(SaveKeys.MasterVolume);
        PlayerPrefs.DeleteKey(SaveKeys.BgmVolume);
        PlayerPrefs.DeleteKey(SaveKeys.SfxVolume);
        
        PlayerPrefs.Save(); // 변경 사항 저장
    }
}


public static class SaveKeys
{
    //UnlockLevel은 게임에서 잠금 해제된 레벨을 저장하는 키입니다.
    public const string UnlockLevel = "Unlock_Level";
    
    //IsTutorialCompleted는 튜토리얼 완료 여부를 저장하는 키입니다.
    public const string IsTutorialCompleted = "Is_Tutorial_Completed";
    
    //설정 관련 키
    public const string IsFullScreen = "Is_FullScreen";
    public const string Resolution = "Resolution";
    public const string MasterVolume = "Master_Volume";
    public const string BgmVolume = "Bgm_Volume";
    public const string SfxVolume = "Sfx_Volume";
    
}