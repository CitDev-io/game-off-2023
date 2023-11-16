
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField]
    List<Buff> Buffs = new List<Buff>();

    public bool HasBuff<T>() where T : Buff {
        return Buffs.Any(buff => buff is T);
    }

    public void AddBuff(Buff newBuff) {
        Type newBuffType = newBuff.GetType();
        var existingBuff = Buffs.FirstOrDefault(buff => buff.GetType() == newBuffType);
        if (existingBuff != null)
        {
            Buffs.Remove(existingBuff);
        }

        Buffs.Add(newBuff);
    }

    public void AgeBuffsForPhase(CombatPhase phase) {
        var buffsToAge = Buffs.Where(buff => buff.AgingPhase == phase);
        Debug.Log("Age for " + phase.ToString() + " : " + buffsToAge.Count());
        foreach (var buff in buffsToAge)
        {
            buff.Tick();
        }
        RemoveAgedBuffs();
    }

    void RemoveAgedBuffs() {
        if (Buffs.Count == 0) return;
        Buffs.RemoveAll(buff => buff.TurnsRemaining < 1);
    }

    public void RestoreStagger() {
        currentStagger = Config.BaseSP;
    }

    // TODO: Give to SKIRMISH Resolver
    CalculatedDamage CalculateFinalDamage(PowerType sourcePowerType, Character source, int rawDamage, int unmitigatedDamage) {
        if (Config.BaseSP == 0) {
            return new CalculatedDamage(
                this,
                unmitigatedDamage,
                0,
                false
            );
        }

        bool SourceAffectsStagger = sourcePowerType != Config.PowerType;

        int DamageDealtToStagger = SourceAffectsStagger ? rawDamage : 0;

        bool CharacterBeganCracked = currentStagger == 0;
        bool CharacterIsCracked = currentStagger <= DamageDealtToStagger;

        int FinalDamageToHealth = CharacterIsCracked ?
            unmitigatedDamage :
            (int) (unmitigatedDamage / 2f);
        
        CalculatedDamage result = new CalculatedDamage(
            this,
            FinalDamageToHealth,
            DamageDealtToStagger,
            !CharacterBeganCracked && CharacterIsCracked
        );

        return result;
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
        return UnityEngine.Random.Range(Config.BaseAttackMin, Config.BaseAttackMax);
    }

    public ExecutedAbility HandleIncomingAttack(PowerType sourcePowerType, Character source) {
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

        CalculatedDamage FinalDamage = CalculateFinalDamage(sourcePowerType, source, rawDamage, unmitigatedDamage);

        TakeDamage(FinalDamage.DamageToHealth);
        TakeStagger(FinalDamage.DamageToStagger);

        ExecutedAbility e = new ExecutedAbility(
            source,
            this,
            AttackType.BASICATTACK,
            new List<AppliedBuff>(),
            new List<CalculatedDamage>{FinalDamage}
        );
        return e;
    }


    void TakeDamage(int Damage) {
        currentHealth -= Damage;
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
    }

    void TakeStagger(int Damage) {
        currentStagger -= Damage;
        if (currentStagger < 0) {
            currentStagger = 0;
        }
    }

    void Die() {
        isDead = true;
    }

    // dev
    [ContextMenu("add stun buff")]
    void GetStunned() {
        AddBuff(new BuffStunned(this, 1));
    }
    [ContextMenu("add multistrike buff")]
    void GetMultistrike() {
        AddBuff(new BuffMultistrike(this, 1));
    }
    [ContextMenu("add charmed buff")]
    void GetCharmed() {
        AddBuff(new BuffCharmed(this, 1));
    }
}
