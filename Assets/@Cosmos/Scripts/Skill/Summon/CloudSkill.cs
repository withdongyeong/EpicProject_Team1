using UnityEngine;

public class CloudSkill : SkillBase
{
    private CloudHandler _cloudHandler;
    [SerializeField] private int _cloudLevel = 1;

    protected override void Activate()
    {
        base.Activate();
        _cloudHandler = FindAnyObjectByType<CloudHandler>();
        // 구름 이미지 변경
        if (_cloudHandler != null)
        {
            if (_cloudLevel > _cloudHandler.CloudLevel)
            {
                _cloudHandler.CloudLevel = _cloudLevel;
            }
            // 구름이 생성되어 있다면 스킬 발동
            else
            {
                ActivateSkill();
            }
        }
    }

    /// <summary>
    /// 구름 스킬을 활성화합니다.
    /// </summary>
    protected virtual void ActivateSkill()
    {
        if (_cloudHandler != null)
        {
            Cloud currentCloud = _cloudHandler.CurrentCloud;
            if (currentCloud != null)
            {
                //먼저 타일 오브젝트의 이름을 가져옵니다
                string coreName = GetComponent<CombineCell>().GetTileObject().name;
                //Cloud라는 문자를 찾고 그 앞부터 클라우드까지만 잘라옵니다
                int index = coreName.IndexOf("Cloud");
                string result = index >= 0
                    ? coreName.Substring(0, index + "Cloud".Length)
                    : coreName; // "Cloud"가 없으면 전체 반환
                currentCloud.Activate(result);
            }
            else
            {
                Debug.LogWarning("Current cloud is null. Cannot activate cloud skill.");
            }
        }
    }
}