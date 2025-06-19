using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string TitleScene = "TitleScene";
    public const string BuildingScene = "BuildingScene";
    public const string StageScene = "StageScene";
    

    public static void LoadTitle() => SceneManager.LoadScene(TitleScene);
    public static void LoadBuilding() => SceneManager.LoadScene(BuildingScene);
    public static void LoadStage() => SceneManager.LoadScene(StageScene);
    public static void ReloadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public static void LoadSceneWithName(string sceneName) => SceneManager.LoadScene(sceneName);
    
    
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
    
}