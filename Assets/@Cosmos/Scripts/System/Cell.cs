using UnityEngine;

public class Cell : MonoBehaviour
{
    
    private CombineCell combinedCell;
    private void Awake()
    {
        combinedCell = GetComponentInParent<CombineCell>();
        if (combinedCell == null)
        {
            Debug.LogError("CombineCell component not found in children of Cell.");
        }
    }


    public CombineCell GetCombineCell()
    {
        return combinedCell;
    }
    
}
