using UnityEngine;

public class Kabuto_RE : MonoBehaviour
{
    private BaseBoss _target;
    private int _currentConsume = 0;
    public bool IsHyper = false;
    private SpriteRenderer _sr;
    private float _cycleDuration = 2.0f;
    private PlayerProtection _protection;
    private ProtectionEffect _protectionEffect;
    private GameObject _projectile;
    private KabutoSummonSkill _summoner;
    private int _effectId = -1;

    private KabutoProjectile _kabutoProjectile;
    private Sprite _imago;


    private void Update()
    {
        if (IsHyper)
        {
            float hue = Mathf.Repeat(Time.time / _cycleDuration, 1f);
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
            _sr.color = rainbowColor;
        }
        if(_currentConsume > 0 && _protection.ProtectionAmount == 0)
        {
            Fire();
        }

        if(_protection.IsProtected)
        {
            if (_kabutoProjectile == null)
            {
                SpawnProjectile();
            }
            if(_effectId == -1)
            {
                _effectId = _protectionEffect.StartProtectionEffect(_protection.gameObject, transform.position);
            }
        }
        else if (!_protection.IsProtected && _effectId != -1) // 보호막이 없으면 이펙트를 중지합니다.
        {
            _protectionEffect.StopProtectionEffect(_effectId);
            _effectId = -1;
        }

    }

    public void Init(KabutoSummonSkill summoner,bool hype)
    {
        _target = FindAnyObjectByType<BaseBoss>();
        _sr = GetComponent<SpriteRenderer>();
        EventBus.SubscribeProtectionConsume(OnProtectionConsume);
        _protection = FindAnyObjectByType<PlayerProtection>();
        _protectionEffect = FindAnyObjectByType<ProtectionEffect>();
        _projectile = Resources.Load<GameObject>("Prefabs/Projectiles/Kabuto");

        _imago = Resources.Load<Sprite>("Arts/Objects/plus/장수풍뎅이 강림");
        _sr.sprite = _imago;

        _summoner = summoner;
        IsHyper = hype;
    }

    private void OnProtectionConsume(int protection)
    {
        SoundManager.Instance.PlayTileSoundClip("KabutoConsume");
        _currentConsume += protection;
    }

    private void SpawnProjectile()
    {
        Vector3 dir = _target.transform.position - transform.position;
        GameObject projectileObj = Instantiate(_projectile, transform.TransformPoint(new Vector2(1.18f,1.156f)), Quaternion.identity);
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, dir);
        Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
        projectileObj.transform.rotation = lookRotation * clockwise90;
        _kabutoProjectile = projectileObj.GetComponent<KabutoProjectile>();
    }

    public void Fire()
    {

        // 보호막 100 이상이면 업적 달성
        if (_currentConsume >= 120)
        {
            SteamAchievement.Achieve("ACH_CON_KABUTO");
        }
        SoundManager.Instance.PlayTileSoundClip("KabutoFire");
        Vector3 dir = _target.transform.position - transform.TransformPoint(new Vector2(1.18f, 1.156f));
        if(_kabutoProjectile != null)
        {
            if (IsHyper)
            {
                _kabutoProjectile.Initialize(dir, Projectile.ProjectileTeam.Player, _currentConsume * 4, true);
            }
            else
            {
                _kabutoProjectile.Initialize(dir, Projectile.ProjectileTeam.Player, _currentConsume * 3, true);
            }
        }    
        _currentConsume = 0;
        _kabutoProjectile = null;

    }


}
