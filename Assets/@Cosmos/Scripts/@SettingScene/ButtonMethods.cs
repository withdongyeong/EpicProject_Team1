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
        GameManager.Instance.GameQuit();
    }

    private void ButtonClickSound()
    {
        // 버튼 클릭 사운드 재생
        SoundManager.Instance.UISoundClip("ButtonActivate");
    }
}
