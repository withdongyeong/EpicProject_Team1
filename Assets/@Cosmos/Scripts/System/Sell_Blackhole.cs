using UnityEngine;

public class Sell_Blackhole : MonoBehaviour
{
    Collider2D _collider;

    private void Awake()
    {
        DragManager.Instance.AssignSell(this);
        _collider = GetComponent<Collider2D>();
    }


    public bool CheckSell(TileObject tile)
    {
        if(_collider.bounds.Contains(tile.transform.position))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
