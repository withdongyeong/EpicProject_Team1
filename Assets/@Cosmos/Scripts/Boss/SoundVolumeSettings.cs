using UnityEngine;

public class SoundVolumeSettings : MonoBehaviour
{
    // ---------------- Player ----------------
    [Header("🎮 Player")]
    public float StickRoll = 1f;
    public float AriaActive = 1f;
    public float PlayerMove = 0.03f;
    public float PlayerDamage = 1f;
    public float PlayerDead = 1f;

    // ---------------- Tile Skills ----------------
    [Header("🧱 Tile Skills")]
    public float HealSkillActivate = 0.2f;
    public float FireBallSkillActivate = 0.5f;
    public float FireBoltSkillActivate = 0.2f;
    public float ShieldSkillActivate = 0.3f;
    public float IcicleSkillActivate = 0.2f;
    public float FlamingSwordSkillActivate = 0.5f;
    public float FrostStaffSkillActivate = 0.2f;
    public float TotemSummonSkillActivate = 0.5f;
    public float ManaTurretSkillActivate = 0.3f;
    public float ProjectileSkillActivate = 0.3f;
    public float ArchmageStaffSkillActivate = 0.7f;
    public float RainbowSkillActivate = 0.2f;
    public float StaffSkillActivate = 0.3f;
    public float ProtectionSkillActivate = 1f;
    public float ShieldSkillRemove = 0.3f;
    public float SwordSkillActivate = 1f;
    public float WarFlagSkillActivate = 1f;
    public float FrostHammerSkillActivate = 1f;
    public float HauntedDollSkillActivate = 0.3f;
    public float CloudSkillActivate = 0.5f;
    public float PhantomSkillActivate = 0.5f;
    public float NecronomiconSkillActivate = 1f;
    public float WraithSkillActivate = 0.8f;
    public float 비구름 = 0.3f;

    // ---------------- Arachne ----------------
    [Header("🕷 Arachne")]
    public float PoisionExplotionActivate = 0.7f;
    public float PoisonBallActivate = 0.7f;
    public float SpiderLegActivate = 0.3f;
    public float SpiderSilkActivate = 0.8f;
    public float ArachneDamageActivate = 0.3f;
    public float ArachneDeadActivate = 1f;

    // ---------------- Orc Mage ----------------
    [Header("🧙 Orc Mage")]
    public float OrcMage_DamageActivate = 0.6f;
    public float OrcMage_DieActivate = 0.6f;
    public float OrcMage_FrogActivate = 1f;
    public float OrcMage_RunActivate = 0.3f;
    public float OrcMage_ScreamActivate = 0.6f;
    public float OrcMage_SpikeActivate = 0.2f;

    // ---------------- Slime ----------------
    [Header("🟢 Slime")]
    public float SlimeTentacleActivate = 0.5f;
    public float SlimeDamageActivate = 1f;
    public float SlimeDeadActivate = 0.3f;

    // ---------------- Bomber ----------------
    [Header("💣 Bomber")]
    public float BomberAttackActivate = 0.05f;
    public float BomberDamageActivate = 0.3f;
    public float BomberDeadActivate = 0.7f;

    // ---------------- Golem ----------------
    [Header("🪨 Golem")]
    public float GolemAttackActivate = 0.2f;
    public float GolemDamageActivate = 1f;
    public float GolemDeadActivate = 1f;

    // ---------------- Turtree ----------------
    [Header("🌳 Turtree")]
    public float TurtreeAttackActivate = 0.05f;
    public float TurtreeDamageActivate = 0.3f;
    public float TurtreeDeadActivate = 0.3f;

    // ---------------- Reaper ----------------
    [Header("☠️ Reaper")]
    public float ReaperAttackActivate = 0.3f;
    public float ReaperDamageActivate = 1f;
    public float ReaperDeadActivate = 1f;

    // ---------------- Knight ----------------
    [Header("🛡 Knight")]
    public float KnightAttackActivate = 0.1f;
    public float KnightDashActivate = 1f;
    public float KnightDamageActivate = 1f;
    public float KnightDeadActivate = 1f;

    // ---------------- Big Hand ----------------
    [Header("✋ Big Hand")]
    public float BigHandAttackActivate = 0.3f;
    public float BigHandAttackActivate_Small = 0.1f;
    public float BigHandFistActivate = 0.01f;
    public float BigHandFingerActivate = 0.7f;
    public float BigHandDamageActivate = 0.3f;
    public float BigHandDeadActivate = 0.3f;
    public float BigHandSlideActivate = 0.5f;


    // ---------------- Last Boss ----------------
    [Header("🔥❄️⚔️ Last Boss")]
    public float LastBossDamageActivate = 0.3f;
    public float LastBossDeadActivate = 1f;
    public float LastBossFlameAttackActivate = 0.1f;
    public float LastBossFrostAttackActivate = 0.1f;
    public float LastBossStaffAttackActivate = 0.1f;
    public float LastBossSwordAttackActivate = 0.1f;
    public float LastBossFlameModeActivate = 0.1f;
    public float LastBossFrostModeActivate = 0.1f;
    public float LastBossStaffModeActivate = 0.1f;
    public float LastBossSwordModeActivate = 1f;

    // ---------------- UI ----------------
    [Header("🖱 UI")]
    public float DeploymentActivate = 0.2f;
    public float RerollActivate = 0.5f;
    public float ButtonActivate = 0.1f;
    public float BlackHoleStartActivate = 0.8f;
    public float TileOpenActivate = 0.3f;
    public float TileLockActivate = 0.3f;
    public float TileSellActivate = 1f;
    public float GameClearActivate = 0.5f;
    public float LoseActivate = 0.5f;

    // ---------------- BGM ----------------
}