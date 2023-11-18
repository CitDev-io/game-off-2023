
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
    public List<Buff> Buffs = new List<Buff>();

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

    public int GetBasicAttackRoll() {
        return UnityEngine.Random.Range(Config.BaseAttackMin, Config.BaseAttackMax);
    }

    public int GetSpecialAttackRoll() {
        return UnityEngine.Random.Range(Config.BaseSpecialMin, Config.BaseSpecialMax);
    }

    public void TakeDamage(int Damage) {
        bool startedDead = currentHealth == 0;

        currentHealth = Math.Clamp(
            currentHealth - Damage,
            0,
            Config.BaseHP
        );

        if (currentHealth == 0 && !startedDead) {
            currentHealth = 0;
            Die();
        }
    }

    public void TakeStagger(int Damage) {
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
        AddBuff(new BuffStunned(this, this, 1));
    }
    [ContextMenu("add multistrike buff")]
    void GetMultistrike() {
        AddBuff(new BuffMultistrike(this, this, 1));
    }
    [ContextMenu("add charmed buff")]
    void GetCharmed() {
        AddBuff(new BuffCharmed(this, this, 1));
    }
}
