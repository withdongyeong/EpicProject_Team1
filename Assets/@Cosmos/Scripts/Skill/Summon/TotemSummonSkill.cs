using UnityEngine;
using UnityEngine.InputSystem;

public class TotemSummonSkill : SkillBase
{
    private GameObject _totem;

    [SerializeField] int _totemPower;

    private TotemManager _totemManager;

    protected override void Start()
    {
        base.Start();
        //먼저 타일 오브젝트의 이름을 가져옵니다
        string coreName = GetComponent<CombineCell>().GetTileObject().name;

        //Totem이라는 문자를 찾고 그 앞부터 토템까지만 잘라옵니다
        int index = coreName.IndexOf("Totem");
        string result = index >= 0
            ? coreName.Substring(0, index + "Totem".Length)
            : coreName; // "here"가 없으면 전체 반환

        //잘라온 결과와 동일한 이름을 가진 토템을 로드해옵니다.
        string filePath = "Prefabs/Summons/Totem/" + result;
        _totem = Resources.Load<GameObject>(filePath);
    }

    protected override void Activate()
    {
        base.Activate();
        _totemManager = FindAnyObjectByType<TotemManager>();
        if (_totemManager != null)
        {
            var summonedTotem = Instantiate(_totem, _totemManager.transform);
            summonedTotem.transform.localPosition = Vector3.zero;
            summonedTotem.GetComponent<BaseTotem>().InitializeTotem(_totemPower);
        }
    }
}
