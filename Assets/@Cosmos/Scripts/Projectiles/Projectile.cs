using UnityEngine;

/// <summary>
/// 기본 투사체 클래스
/// </summary>
public class Projectile : MonoBehaviour
{
    private int damage = 10;
    private float speed = 18f;
    private Vector3 direction;
    private BossDebuff bossDebuff = BossDebuff.None; // 상태 이상
    private ProjectileTeam _team; // 투사체 소속 진영
    private bool isFrostHammer = false; // FrostHammer 투사체 여부
    private bool _isRainbow = false; //무지개로 빛나는지 여부
    private SpriteRenderer _sr;

    private GameObject hitEffect;
    private string PatternName = "Projectile";
   
    public enum ProjectileTeam
    {
        Player,
        Enemy
    }

    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public ProjectileTeam Team { get => _team; set => _team = value; }
    public BossDebuff BossDebuff { get => bossDebuff; set => bossDebuff = value; }
    public bool IsFrostHammer { get => isFrostHammer; set => isFrostHammer = value; }

    /// <summary>
    /// 투사체 초기화
    /// </summary>
    public void Initialize(Vector3 dir, ProjectileTeam projectileTeam, int givenDamage = 10, bool isRainbow = false)
    {
        direction = dir.normalized;
        _team = projectileTeam;
        damage = givenDamage;
        _isRainbow = isRainbow;
        if(_isRainbow)
        {
            _sr = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        hitEffect = Resources.Load("Prefabs/HitEffect/HitEffect_Frost") as GameObject;
    }

    void Update()
    {
        transform.position += Time.deltaTime * direction * speed;
    
        // 화면 밖으로 나가면 제거
        if (Mathf.Abs(transform.position.x) > 20 || Mathf.Abs(transform.position.y) > 20)
        {
            Destroy(gameObject);
        }
        if(_isRainbow)
        {
            float hue = Mathf.Repeat(Time.time, 1f);
            Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
            _sr.color = rainbowColor;
        }
    }

    /// <summary>
    /// 충돌 처리
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // 적 진영 투사체가 아군에게 충돌
        if (_team == ProjectileTeam.Enemy)
        {
            PlayerHp player = other.GetComponent<PlayerHp>();
            if (player != null)
            {
                player.TakeDamage(damage, patternName:PatternName);
                Destroy(gameObject);
            }
        }
        // 아군 진영 투사체가 적에게 충돌
        else if (_team == ProjectileTeam.Player)
        {
            BaseBoss enemy = other.GetComponent<BaseBoss>();
            if (enemy != null)
            {
                // FrostHammer 투사체이고 보스가 정지 상태인 경우
                if (isFrostHammer && (enemy.IsStopped || enemy.IsDamageIncreased))
                {
                    damage *= 10; // 피해량 10배 증가
                }

                if (bossDebuff != BossDebuff.None)
                {
                    if(bossDebuff == BossDebuff.Frostbite)
                    {
                        enemy.TakeDamage(damage, hitEffect);
                        // 피해량이 500 이상인 경우 업적
                        if (isFrostHammer)
                        {
                            if (damage >= 333)
                            {
                                SteamAchievement.Achieve("ACH_CON_HAMMER"); // FrostHammer 업적 달성
                            }

                            if (enemy.IsStopped || enemy.IsDamageIncreased)
                            {
                                enemy.GetComponent<BossDebuffs>().InterruptFrostEffect(); // 동결 효과 중단
                                Destroy(gameObject); // 디버프 부여하지 않고 투사체 제거
                            }
                            
                        }
                    }
                    else enemy.TakeDamage(damage, null);

                    enemy.AddDebuff(bossDebuff); // 상태 이상 추가
                }
                else enemy.TakeDamage(damage, null);
                
                Destroy(gameObject);
            }
        }
    }
}