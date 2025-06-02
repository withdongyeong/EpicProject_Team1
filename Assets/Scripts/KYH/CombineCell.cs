using System;
using UnityEngine;

public class CombineCell : MonoBehaviour
{
    public GameObject coreCell;

    private void Awake()
    {
        if (coreCell == null)
        {
            coreCell = transform.GetChild(0).gameObject;
        }
    }
}
