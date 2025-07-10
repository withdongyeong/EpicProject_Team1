using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    public void OnTitleButton()
    {
        SceneLoader.ToggleSetting();
        GameManager.Instance.LoadTitle();
    }
    
    public void OnConfirmButton()
    {
        SceneLoader.ToggleSetting();
    }
    
    public void OnExitButton()
    {
        SaveManager.SaveAll(); // 게임 저장
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }
}
