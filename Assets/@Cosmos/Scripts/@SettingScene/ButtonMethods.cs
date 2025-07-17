using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMethods : MonoBehaviour
{
    private void Awake()
    {
        // 버튼 클릭 이벤트 등록
        if (SceneLoader.IsInTitle())
        {
            transform.GetChild(1).GetComponent<Button>().interactable = false;
            transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }
        else
        {
            transform.GetChild(1).GetComponent<Button>().interactable = true;
            transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
    }

    public void OnAbandonButton()
    {
        ButtonClickSound();
        SceneLoader.ToggleSetting();
        GameManager.Instance.LoadTitle();
    }

    public void OnContinueButton(GameObject gameObject)
    {
        ButtonClickSound();
        gameObject.SetActive(false);
    }

    public void OnTitleButton(GameObject gameObject)
    {
        ButtonClickSound();
        gameObject.SetActive(true);
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
