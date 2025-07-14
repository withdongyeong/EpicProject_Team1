using UnityEngine;

public class ButtonMethods : MonoBehaviour
{
    public void OnTitleButton()
    {
        ButtonClickSound();
        SceneLoader.ToggleSetting();
        GameManager.Instance.LoadTitle();
    }
    
    public void OnConfirmButton()
    {
        ButtonClickSound();
        SceneLoader.ToggleSetting();
    }
    
    public void OnExitButton()
    {
        ButtonClickSound();
        SaveManager.SaveAll(); // 게임 저장
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }

    private void ButtonClickSound()
    {
        // 버튼 클릭 사운드 재생
        SoundManager.Instance.UISoundClip("ButtonActivate");
    }
}
