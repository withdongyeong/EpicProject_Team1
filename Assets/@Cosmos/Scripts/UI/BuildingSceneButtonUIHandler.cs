using UnityEngine;
using UnityEngine.UI;

public class BuildingSceneButtonUIHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SceneLoader.ToggleSetting());
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(ToggleJournal);
    }

    private void ToggleJournal()
    {
        if(JournalSlotManager.Instance.gameObject.activeSelf)
        {
            JournalSlotManager.Instance.gameObject.SetActive(false);
        }
        else
        {
            JournalSlotManager.Instance.gameObject.SetActive(true);
        }
    }


}
