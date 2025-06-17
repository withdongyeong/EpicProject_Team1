using UnityEngine;

public class CombinedStarCell : MonoBehaviour
{
    private StarBase starSkill;

    private void Awake()
    {
        starSkill = GetComponent<StarBase>();
        if (starSkill == null)
        {
            Debug.LogError("스타스킬이 없는데요 ???? ");
        }
    }

    public StarBase GetStarSkill()
    {
        return starSkill;
    }
}
