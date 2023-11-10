
using UnityEngine;
using Spine.Unity;

public class Character : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField]
    public CharacterConfig Config;
    
    [Header("Current State")]
    [SerializeField]
    public int currentHealth = 1;
    [SerializeField]
    public int currentStagger = 0;
    public bool isDead = false;
    public bool IsCurrentCombatant = false;

    public void RestoreStagger() {
        currentStagger = Config.BaseSP;
    }

    // TODO: Give to SKIRMISH Resolver
    int CalculateFinalDamage(PowerType sourcePowerType, Character source, int rawDamage, int unmitigatedDamage) {
        if (Config.TeamType == TeamType.PLAYER) return unmitigatedDamage;

        bool CharacterIsCracked = currentStagger == 0;

        if (CharacterIsCracked) {
            return unmitigatedDamage;
        }

        bool SourceAffectsStagger = sourcePowerType != Config.PowerType;

        int DamageDealtToStagger = SourceAffectsStagger ? rawDamage : 0;

        currentStagger = Mathf.Clamp(
            currentStagger - DamageDealtToStagger,
            0,
            currentStagger
        );

        int DamageToHealth = (int) (unmitigatedDamage / 2f);

        return DamageToHealth;
    }

    public void InitializeMe() {
        isDead = false;
        currentHealth = Config.BaseHP;
        currentStagger = Config.BaseSP;
    }

    public void TurnStart() {
        IsCurrentCombatant = true;
    }

    public void TurnEnd() {
        IsCurrentCombatant = false;
    }

    public int GetRandomDamageRoll() {
        return Random.Range(Config.BaseAttackMin, Config.BaseAttackMax);
    }

    public int HandleIncomingAttack(PowerType sourcePowerType, Character source) {
        int rawDamage = source.GetRandomDamageRoll();

        bool resistantToPowerType = sourcePowerType == Config.PowerType;
        int PowerTypeResistance = resistantToPowerType ? 10 : 0;
        int mitigationPower = Config.BaseMitigation + PowerTypeResistance;
        int mitigatedDamage = (int) (rawDamage * (mitigationPower / 100f));

        int unmitigatedDamage = Mathf.Clamp(
            rawDamage - mitigatedDamage,
            0,
            rawDamage
        );

        int FinalDamage = CalculateFinalDamage(sourcePowerType, source, rawDamage, unmitigatedDamage);

        TakeDamage(FinalDamage);
        return FinalDamage;
    }


    void TakeDamage(int Damage) {
        currentHealth -= Damage;
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
    }

    void Die() {
        isDead = true;
    }
}
