using UnityEngine;

public class TurtleBase : MonoBehaviour
{
    /// <summary>
    /// 마지막으로 공격한 시간입니다. 시작하자마자 쿨이 돕니다.
    /// </summary>
    private float _lastUsedTime;

    private float _chargeInterval = 1.0f;

    /// <summary>
    /// 차지할때마다 사용하는 보호막 양입니다.
    /// </summary>
    private int _consumeProtection = 3;

    /// <summary>
    /// 차지된 횟수입니다.
    /// </summary>
    private int _chargedProtection = 0;

    /// <summary>
    /// 보호막 스크립트입니다
    /// </summary>
    private PlayerProtection _protectionScript;

    /// <summary>
    /// 보호막 효과 스크립트입니다. 보호막이 차지되면 보호막 효과를 보여줍니다.
    /// </summary>
    private ProtectionEffect _protectionEffect;

    /// <summary>
    /// 보호막 효과 Id입니다.
    /// </summary>
    private int _effectId = -1;

    /// <summary>
    /// 발사체 프리팹입니다. 여기에 회전하는 거북이 넣으면 회전하면서 날아갑니다.
    /// </summary>
    private GameObject _projectilePrefab;

    /// <summary>
    /// 색깔 바꾸기용입니다.
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// 무지개빛으로 빛나는 주기입니다.
    /// </summary>
    private float _cycleDuration = 1f;

    private void Awake()
    {
        _lastUsedTime = Time.time;
        _protectionScript = FindAnyObjectByType<PlayerProtection>();
        _protectionEffect = FindAnyObjectByType<ProtectionEffect>();
        _projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/TurtleProjectile");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        EventBus.SubscribeProtectionConsume(OnProtectionConsume);
    }


    private void Update()
    {
        if(Time.time - _lastUsedTime > _chargeInterval)
        {
            _lastUsedTime = Time.time;
            // 보호막이 차지 중이고 보호막이 있다면 이펙트를 재생합니다.
            if (_protectionScript.IsProtected && _effectId == -1)
            {
                _effectId = _protectionEffect.StartProtectionEffect(_protectionScript.gameObject, transform.position);
            }
            // 보호막이 없으면 이펙트를 중지합니다.
            else if (!_protectionScript.IsProtected && _effectId != -1)
            {
                _protectionEffect.StopProtectionEffect(_effectId);
                _effectId = -1;
            }
        }
        if (_chargedProtection >= 15)
        {
            float hue = Mathf.Repeat(Time.time / _cycleDuration, 1f);
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
            _spriteRenderer.color = rainbowColor;
            // 보호막이 차지 완료되면 이펙트를 중지합니다.
            if (_effectId != -1)
            {
                _protectionEffect.StopProtectionEffect(_effectId);
                _effectId = -1;
            }
        }
        else
        {
            _spriteRenderer.color = Color.white;
        }
    }

    private void Charge()
    {
        if(_chargedProtection < 15)
        {
            _protectionScript.TryProtectionBlock(_consumeProtection, true);
        }
    }

    public void Fire()
    {
        if(_projectilePrefab != null)
        {
            Vector3 dir = (FindAnyObjectByType<BaseBoss>().transform.position - transform.position);
            GameObject projectileObj = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, dir);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            bool isRainbow;
            if (_chargedProtection < 15)
            {
                SoundManager.Instance.PlayTileSoundClip("TurtleAttack");
                isRainbow = false;
            }
            else
            {
                SoundManager.Instance.PlayTileSoundClip("TurtleRainbowAttack");
                isRainbow = true;
            }

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            if(_chargedProtection == 0)
            {
                projectile.Initialize(dir, Projectile.ProjectileTeam.Player, 1, isRainbow);
            }
            else
            {
                projectile.Initialize(dir, Projectile.ProjectileTeam.Player, _chargedProtection * 3, isRainbow);
            }
            projectile.SetTileName("TurtleTile");
               
            _chargedProtection = 0;

        }
    }

    private void OnProtectionConsume(int num)
    {
        _chargedProtection = Mathf.Min(15, _chargedProtection + num);
    }

    private void OnDestroy()
    {
        EventBus.UnSubscribeProtectionConsume(OnProtectionConsume);
    }


}
