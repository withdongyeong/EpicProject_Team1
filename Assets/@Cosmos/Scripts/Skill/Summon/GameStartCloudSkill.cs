using UnityEngine;

public class GameStartCloudSkill : NonActivateSkill
{
    private CloudHandler _cloudHandler;
    [SerializeField] private int _cloudLevel = 1;

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(InitializeCloud);
    }

    /// <summary>
    /// 구름 핸들러를 초기화합니다. 구름 레벨이 현재 핸들러의 레벨보다 높으면 업데이트합니다.
    /// </summary>
    private void InitializeCloud()
    {
        _cloudHandler = FindAnyObjectByType<CloudHandler>();
        if (_cloudHandler != null)
        {
            // 구름 레벨이 현재 핸들러의 레벨보다 높으면 업데이트
            if (_cloudLevel > _cloudHandler.CloudLevel)
            {
                _cloudHandler.CloudLevel = _cloudLevel;
            }
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
                currentCloud.Init(result);
            }
        }
    }

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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 구독 해제
        EventBus.UnsubscribeGameStart(InitializeCloud);
    }
}