using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string TitleScene = "TitleScene";
    public const string LogoScene = "LogoScene"; 
    public const string BuildingScene = "BuildingScene";
    public const string StageScene = "StageScene";
    
    //셋팅 씬
    public const string SettingScene = "SettingScene";
    private static bool settingSceneLoaded = false;

    public static void LoadTitle() => FadeManager.Instance.LoadSceneWithFade(TitleScene);
    public static void LoadLogo() => FadeManager.Instance.LoadSceneWithFade(LogoScene);
    public static void LoadBuilding() => FadeManager.Instance.LoadSceneWithFade(BuildingScene);
    public static void LoadStage() => FadeManager.Instance.LoadSceneWithFade(StageScene);
    public static void ReloadCurrentScene() => FadeManager.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().name);
    public static void LoadSceneWithName(string sceneName) => FadeManager.Instance.LoadSceneWithFade(sceneName);
    
    
    public static bool IsCurrentScene(string sceneName) => SceneManager.GetActiveScene().name == sceneName;
    
    public static bool IsInTitle() => IsCurrentScene(TitleScene);

    public static bool IsInBuilding()
    {
        if (IsCurrentScene(BuildingScene) || 
            IsCurrentScene("BuildingScene_KYH") || // 추가적인 설정 씬 이름을 여기에 추가
            IsCurrentScene("BuildingScene_KMJ") ||
            IsCurrentScene("BuildingScene_KDY") ||
            IsCurrentScene("BuildingScene_YDH") ||
            IsCurrentScene("BuildingScene_PGW")) // 테스트용) 설정 씬
        {
            return true;
        }
        return false;
    }

    public static bool IsInStage()
    {
        if(IsCurrentScene(StageScene) || 
           IsCurrentScene("StageScene_KYH") || // 추가적인 스테이지 씬 이름을 여기에 추가
           IsCurrentScene("StageScene_KMJ") ||
           IsCurrentScene("StageScene_KDY") ||
           IsCurrentScene("StageScene_PGW") ||
           IsCurrentScene("StageScene_YDH")) // 테스트용) 스테이지 씬
        {
            return true;
        }
        return false;
    }

    public static void ToggleSetting()
    {
        if(!settingSceneLoaded)
        {
            SceneManager.LoadSceneAsync(SettingScene, LoadSceneMode.Additive);
            settingSceneLoaded = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(SettingScene);
            settingSceneLoaded = false;
        }
    }
}