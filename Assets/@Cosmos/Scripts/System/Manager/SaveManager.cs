using UnityEngine;

public static class SaveManager
{
    
    // 🔒 캐싱 변수들
    public static int FirstStart { get; private set; } //게임이 처음 시작되었는지 여부입니다. (0: 처음 시작, 1: 다시 시작)
    public static int UnlockLevel { get; private set; } //타일이 해금 된 정도입니다
    public static int IsTutorialCompleted { get; private set; }
    public static bool IsFullScreen { get; private set; }
    public static string Resolution { get; private set; }
    public static float MasterVolume { get; private set; } 
    public static float BgmVolume { get; private set; }
    public static float SfxVolume { get; private set; }
    public static int GameModeLevel { get; private set; } //게임모드가 해금 된 정도입니다 (ex:하드모드, 베리 하드모드)
    public static int ShownUnlockLevel { get; private set; } //해금된 타일이 타이틀에서 보여진 정도입니다
    
    public static int LanguageIndex { get; private set; } //언어 인덱스 (0: 한국어, 3: 영어 등)

    // ✅ 처음 로드시 호출
    public static void LoadAll()
    {
        FirstStart = PlayerPrefs.GetInt(SaveKeys.FirstStart, 0); // 0이면 처음 시작, 1이면 다시 시작
        UnlockLevel = PlayerPrefs.GetInt(SaveKeys.UnlockLevel, 0);
        IsTutorialCompleted = PlayerPrefs.GetInt(SaveKeys.IsTutorialCompleted, 0);
        IsFullScreen = PlayerPrefs.GetInt(SaveKeys.IsFullScreen, 1) == 1; // 1이면 true, 0이면 false
        Resolution = PlayerPrefs.GetString(SaveKeys.Resolution, "1920x1080");
        MasterVolume = PlayerPrefs.GetFloat(SaveKeys.MasterVolume, 1.0f);
        BgmVolume = PlayerPrefs.GetFloat(SaveKeys.BgmVolume, 1.0f);
        SfxVolume = PlayerPrefs.GetFloat(SaveKeys.SfxVolume, 1.0f);
        GameModeLevel = PlayerPrefs.GetInt(SaveKeys.GameModeLevel, 1);
        ShownUnlockLevel = PlayerPrefs.GetInt(SaveKeys.ShownUnlockLevel, 0);
        LanguageIndex = PlayerPrefs.GetInt(SaveKeys.LanguageIndex, 3); // 기본값은 3 (영어)
    }

    public static void SaveAll()
    {
        PlayerPrefs.SetInt(SaveKeys.FirstStart, 1);
        PlayerPrefs.SetInt(SaveKeys.UnlockLevel, UnlockLevel);
        PlayerPrefs.SetInt(SaveKeys.IsTutorialCompleted, IsTutorialCompleted);
        PlayerPrefs.SetInt(SaveKeys.IsFullScreen, IsFullScreen? 1 : 0);
        PlayerPrefs.SetString(SaveKeys.Resolution, Resolution);
        PlayerPrefs.SetFloat(SaveKeys.MasterVolume, MasterVolume);
        PlayerPrefs.SetFloat(SaveKeys.BgmVolume, BgmVolume);
        PlayerPrefs.SetFloat(SaveKeys.SfxVolume, SfxVolume);
        PlayerPrefs.SetInt(SaveKeys.GameModeLevel, GameModeLevel);
        PlayerPrefs.SetInt(SaveKeys.ShownUnlockLevel, ShownUnlockLevel);
        PlayerPrefs.SetInt(SaveKeys.LanguageIndex, LanguageIndex); // 언어 인덱스 저장
        
        PlayerPrefs.Save(); // 변경 사항 저장
    }
    
    // ✅ 저장시엔 PlayerPrefs 와 변수 둘 다 갱신
    public static void SaveFirstStart()
    {
        FirstStart = 1;
        PlayerPrefs.SetInt(SaveKeys.FirstStart, 1);
        PlayerPrefs.Save();
    }
    
    
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

    public static void SaveGameModeLevel(int level)
    {
        GameModeLevel = level;
        PlayerPrefs.SetInt(SaveKeys.GameModeLevel, level);
        PlayerPrefs.Save();
    }

    public static void SaveShownUnlockLevel(int level)
    {
        ShownUnlockLevel = level;
        PlayerPrefs.SetInt(SaveKeys.ShownUnlockLevel, level);
        PlayerPrefs.Save();
    }
    
    public static void SaveLanguageIndex(int index)
    {
        LanguageIndex = index;
        PlayerPrefs.SetInt("LanguageIndex", index);
        PlayerPrefs.Save();
    }
    
    public static void DeleteAllSaves()
    {
        PlayerPrefs.DeleteKey(SaveKeys.FirstStart);
        PlayerPrefs.DeleteKey(SaveKeys.UnlockLevel);
        PlayerPrefs.DeleteKey(SaveKeys.IsTutorialCompleted);
        PlayerPrefs.DeleteKey(SaveKeys.IsFullScreen);
        PlayerPrefs.DeleteKey(SaveKeys.Resolution);
        PlayerPrefs.DeleteKey(SaveKeys.MasterVolume);
        PlayerPrefs.DeleteKey(SaveKeys.BgmVolume);
        PlayerPrefs.DeleteKey(SaveKeys.SfxVolume);
        PlayerPrefs.DeleteKey(SaveKeys.GameModeLevel);
        PlayerPrefs.DeleteKey(SaveKeys.ShownUnlockLevel);
        PlayerPrefs.DeleteKey(SaveKeys.LanguageIndex);
        LoadAll();
        PlayerPrefs.Save(); // 변경 사항 저장
        
    }
}


public static class SaveKeys
{
    public const string FirstStart = "First_Start"; // 게임이 처음 시작되었는지 여부를 저장하는 키입니다.
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

    //게임 모드가 해금된 정도를 저장하는 키입니다. (ex: 2이면 하드 모드만 해금되어있다)
    public const string GameModeLevel = "Game_Mode_Level";

    public const string ShownUnlockLevel = "Shown_Unlock_Level";
    
    public const string LanguageIndex = "Language_Index"; // 언어 인덱스 저장 키
    
}