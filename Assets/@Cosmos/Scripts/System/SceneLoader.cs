using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string TitleScene = "TitleScene";
    public const string LogoScene = "LogoScene"; 
    public const string BuildingScene = "BuildingScene";
    public const string StageScene = "StageScene";
    public const string GuideBuildingScene = "GuideBuildingScene";
    public const string GuideStageScene = "GuideStageScene"; //튜토리얼 씬
    public const string CreditsScene = "CreditsScene";
    
    //셋팅 씬
    public const string SettingScene = "SettingScene";
    private static bool settingSceneLoaded = false;
    private static bool isPausedBySettings = false;  // 설정창으로 인한 일시정지 상태 추적

    public static void LoadTitle() => FadeManager.Instance.LoadSceneWithFade(TitleScene);
    public static void LoadLogo() => FadeManager.Instance.LoadSceneWithFade(LogoScene);
    public static void LoadBuilding() => FadeManager.Instance.LoadSceneWithFade(BuildingScene);
    public static void LoadStage() => FadeManager.Instance.LoadSceneWithFade(StageScene);
    
    public static void LoadGuideBuilding() => FadeManager.Instance.LoadSceneWithFade(GuideBuildingScene);
    public static void LoadGuideStage() => FadeManager.Instance.LoadSceneWithFade(GuideStageScene);

    public static void LoadCredits() => FadeManager.Instance.LoadSceneWithFade(CreditsScene);
    
    public static void ReloadCurrentScene() => FadeManager.Instance.LoadSceneWithFade(SceneManager.GetActiveScene().name);
    public static void LoadSceneWithName(string sceneName) => FadeManager.Instance.LoadSceneWithFade(sceneName);
    
    
    public static bool IsCurrentScene(string sceneName) => SceneManager.GetActiveScene().name == sceneName;
    public static string GetCurrentSceneName() => SceneManager.GetActiveScene().name;
    public static bool IsInTitle() => IsCurrentScene(TitleScene);

    public static bool IsInBuilding()
    {
        if (IsCurrentScene(BuildingScene) || IsCurrentScene(GuideBuildingScene))
        {
            return true;
        }
        return false;
    }

    public static bool IsInStage()
    {
        if(IsCurrentScene(StageScene) || IsCurrentScene(GuideStageScene))
        {
            return true;
        }
        return false;
    }
    
    public static bool IsInGuide()
    {
        if(IsCurrentScene(GuideBuildingScene) || IsCurrentScene(GuideStageScene)) // 가이드 중인가요 ?
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 설정창 토글 - 게임 상태에 따른 일시정지 처리 포함
    /// </summary>
    public static void ToggleSetting()
    {
        GameStateManager gameStateManager = GameStateManager.Instance;
    
        // Count 상태에서는 설정창 열기 차단
        if (!settingSceneLoaded && !gameStateManager.CanOpenSetting())
        {
            return;
        }

        if(!settingSceneLoaded)
        {
            settingSceneLoaded = true;
            // Playing 상태이면서 현재 일시정지되지 않은 상태에서만 일시정지하고 플래그 설정
            if (gameStateManager.CurrentState == GameState.Playing && 
                !TimeScaleManager.Instance.IsTimeScaleStopped)
            {
                TimeScaleManager.Instance.StopTimeScale();
                isPausedBySettings = true;  // 설정창이 일시정지를 시킨 경우만 true
            }
            else
            {
                isPausedBySettings = false;  // 이미 일시정지 상태였거나 Playing이 아닌 경우
            }
        
            SceneManager.LoadSceneAsync(SettingScene, LoadSceneMode.Additive);

        }
        else
        {
            //만약 세팅씬이 로드되어있지 않을 가능성이 있으므로 예외처리 합니다.
            bool sceneExist = false;
            int sceneCount = SceneManager.sceneCount;
            for(int i = 0; i<sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                Debug.Log(scene.name);
                if(SettingScene == scene.name)
                {
                    sceneExist = true;
                }
            }

            if(sceneExist)
            {
                settingSceneLoaded = false;
                // 설정창으로 인한 일시정지였다면 해제
                if (isPausedBySettings)
                {
                    TimeScaleManager.Instance.ResetTimeScale();
                    isPausedBySettings = false;
                }

                SceneManager.UnloadSceneAsync(SettingScene);
            }
           
        }
    }

    public static void CloseSetting()
    {
        settingSceneLoaded = false;
        // 설정창으로 인한 일시정지였다면 해제
        if (isPausedBySettings)
        {
            TimeScaleManager.Instance.ResetTimeScale();
            isPausedBySettings = false;
        }

        SceneManager.UnloadSceneAsync(SettingScene);
    }
}