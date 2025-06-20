using UnityEngine;
using DamageNumbersPro;

public class Example : MonoBehaviour {

    //Assign prefab in inspector.
    public DamageNumber numberPrefab;
    public RectTransform rectParent;

    void Update()
    {
        //On leftclick.
        if(Input.GetMouseButtonDown(0))
        {
            //Spawn new popup with a random number between 0 and 100.
            DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, Vector2.zero, Random.Range(100,101));
        }
    }
}