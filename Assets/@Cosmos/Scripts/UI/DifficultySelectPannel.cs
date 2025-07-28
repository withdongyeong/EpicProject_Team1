using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelectPannel : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => DifficultySelectButton(0));
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DifficultySelectButton(1));

        //하드모드
        if(SaveManager.GameModeLevel>=2)
        {
            transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => DifficultySelectButton(2));
            transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => DifficultySelectButton(3));
            transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        else
        {
            transform.GetChild(3).GetComponent<Button>().interactable = false;
            transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
            transform.GetChild(4).GetComponent<Button>().interactable = false;
            transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
        }

        transform.GetChild(5).GetComponent<Button>().onClick.AddListener(ClosePannel);
    }

    //타이틀씬에서 쓰는 빌딩 씬으로 이동하는 버튼
    private void GOBUILDSCENE()
    {
        DragManager.Instance.GetComponentInChildren<PlacedHandler>().FirstPresent();
        SoundManager.Instance.UISoundClip("ButtonActivate");
        GameManager.Instance.LogHandler.SetSessionPlayTimer();
        SceneLoader.LoadBuilding();
    }

    private void DifficultySelectButton(int num)
    {
        GameManager.Instance.SetDifficultyLevel(num);
        GOBUILDSCENE();

    }

    private void ClosePannel()
    {
        gameObject.SetActive(false);
    }


}
