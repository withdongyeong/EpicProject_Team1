using UnityEngine;

public class TotemSummonSkill : SkillBase
{
    [SerializeField] private GameObject _totem;

    [SerializeField] InventoryItemData _itemData;

    private TotemManager _totemManager;

    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        _totemManager = FindAnyObjectByType<TotemManager>();
        if (_totemManager != null)
        {
            var summonedTotem = Instantiate(_totem, _totemManager.transform);
            summonedTotem.transform.localPosition = Vector3.zero;
            summonedTotem.GetComponent<BaseTotem>().InitializeTotem(_itemData);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("시작함");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
