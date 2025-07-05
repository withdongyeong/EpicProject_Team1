using UnityEngine;

public class Kabuto : MonoBehaviour
{
    private BaseBoss _target;
    private int _currentConsume = 0;
    private bool _isCastOff = false;
    private SpriteRenderer _sr;
    private float _cycleDuration = 2.0f;
    private float _chargeStartedTime;
    private bool _isCharging = false;
    private PlayerProtection _protection;
    private GameObject _projectile;
    private KabutoSummonSkill _summoner;

    private Sprite _pupa;
    private Sprite _imago;


    private void Update()
    {
        if(_isCastOff)
        {
            float hue = Mathf.Repeat(Time.time / _cycleDuration, 1f);
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
            _sr.color = rainbowColor;
        }
        if(_isCharging && Time.time - _chargeStartedTime > 4.0f)
        {
            Fire();
        }
    }

    public void Init(KabutoSummonSkill summoner)
    {
        _target = FindAnyObjectByType<BaseBoss>();
        _sr = GetComponent<SpriteRenderer>();
        EventBus.SubscribeProtectionConsume(OnProtectionConsume);
        _protection = FindAnyObjectByType<PlayerProtection>();
        _projectile = Resources.Load<GameObject>("Prefabs/Projectiles/Kabuto");
        _pupa = Resources.Load<Sprite>("Arts/Objects/plus/장수풍뎅이 번데기");
        _imago = Resources.Load<Sprite>("Arts/Objects/plus/장수풍뎅이 강림");
        _summoner = summoner;
    }

    private void OnProtectionConsume(int protection)
    {
        _currentConsume += protection;
        if(_currentConsume >= 20 && !_isCastOff)
        {
            CastOff();
        }
    }

    private void CastOff()
    {
        _isCastOff = true;
        _sr.sprite = _imago;
    }

    private void PutOn()
    {
        _currentConsume = 0;
        _isCastOff = false;
        _sr.sprite = _pupa;
        _sr.color = Color.white;

    }

    public void TryCharge()
    {
        if(_isCastOff && !_isCharging)
        {
            _chargeStartedTime = Time.time;
            _currentConsume = 20;
            _isCharging = true;
            _protection.SetChargeBool(true);
        }
        
    }

    private void Fire()
    {
        Vector3 dir = _target.transform.position - transform.position;
        GameObject projectileObj = Instantiate(_projectile, transform.position, Quaternion.identity);
        Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, dir);
        Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
        projectileObj.transform.rotation = lookRotation * clockwise90;
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Initialize(dir, Projectile.ProjectileTeam.Player, _currentConsume * 3);
        _protection.SetChargeBool(false);
        _isCharging = false;
        PutOn();
    }

    
}
