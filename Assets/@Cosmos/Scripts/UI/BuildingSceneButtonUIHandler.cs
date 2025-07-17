using UnityEngine;
using UnityEngine.UI;

public class BuildingSceneButtonUIHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.GetChild(0)?.GetComponent<Button>().onClick.AddListener(() => SceneLoader.ToggleSetting());
        transform.GetChild(1)?.GetComponent<Button>().onClick.AddListener(()=> JournalSlotManager.Instance.ToggleJournal());
    }
}
