using UnityEngine;
using UnityEngine.EventSystems;

public class HoverDifficultyText : MonoBehaviour, IPointerEnterHandler
{
    private DifficultyText _difficultyText;
    [SerializeField] private int _difficulty = 1;

    private void Awake()
    {
        _difficultyText = transform.parent.GetComponentInChildren<DifficultyText>();
        if (_difficultyText == null)
        {
            Debug.LogError("DifficultyText 컴포넌트가 없습니다.");
        }
    }

    private void Start()
    {
        if (_difficultyText != null)
        {
            _difficultyText.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _difficultyText?.gameObject.SetActive(true);
        _difficultyText?.SetDifficulty(_difficulty);
    }

}
