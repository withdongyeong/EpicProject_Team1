using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField]
    private CombineCell combinedCell;
    private void Awake()
    {
        combinedCell = GetComponentInParent<CombineCell>();
        if (combinedCell == null)
        {
            Debug.LogError("CombineCell component not found in children of Cell.");
        }
    }


    public CombineCell GetObjectData()
    {
        return combinedCell;
    }
}
