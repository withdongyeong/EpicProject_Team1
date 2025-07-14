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
        }
        else
        {
            transform.GetChild(3).GetComponent<Button>().interactable = false;
        }

        //베리 하드모드
        if (SaveManager.GameModeLevel >= 3)
        {
            transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => DifficultySelectButton(3));
        }
        else
        {
            transform.GetChild(4).GetComponent<Button>().interactable = false;
        }

        transform.GetChild(5).GetComponent<Button>().onClick.AddListener(ClosePannel);
    }

    //타이틀씬에서 쓰는 빌딩 씬으로 이동하는 버튼
    private void GOBUILDSCENE()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        SceneLoader.LoadBuilding();
    }

    private void DifficultySelectButton(int num)
    {
        if(SaveManager.GameModeLevel >= num)
        {
            GameManager.Instance.SetDifficultyLevel(num);
            GOBUILDSCENE();
        }
    }

    private void ClosePannel()
    {
        gameObject.SetActive(false);
    }


}
