
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
    public int BattlefieldPosition = -10;
    [SerializeField]
    public List<Buff> Buffs = new List<Buff>();
    public int GenericWaveCounter = 0;

    public bool HasBuff<T>() where T : Buff {
        return Buffs.Any(buff => buff is T);
    }

    public void RemoveBuff<T>() where T : Buff {
        Buffs.RemoveAll(buff => buff is T);
    }

    public List<AbilityCategory> GetAvailableAbilities(int LightPoints, int ShadowPoints) {
        var availableAbilities = new List<AbilityCategory>(){ 
            AbilityCategory.BASICATTACK,
        };
        PowerType powerType = Config.PowerType;
        
        if (powerType == PowerType.LIGHT && LightPoints < 1) {
            return availableAbilities;
        }
        if (powerType == PowerType.SHADOW && ShadowPoints < 1) {
            return availableAbilities;
        }
        if (Config.SpecialAttack != UserAbilitySelection.NONE) {
            availableAbilities.Add(AbilityCategory.SPECIALATTACK);
        };
        if (Config.UltimateAbility != UserAbilitySelection.NONE && LightPoints > 1 && ShadowPoints > 1) {
            availableAbilities.Add(AbilityCategory.ULTIMATE);
        };

        if (HasBuff<BuffStunned>()) {
            return new List<AbilityCategory>();
        }

        if (HasBuff<BuffCharmed>() || HasBuff<BuffSilenced>() || HasBuff<BuffTaunted>()) {
            return new List<AbilityCategory>(){ 
                AbilityCategory.BASICATTACK,
            };
        }
        return availableAbilities;
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
        foreach (var buff in buffsToAge)
        {
            buff.Tick();
        }
        RemoveAgedBuffs();
    }

    public void RemoveRandomDebuff() {
        if (Buffs.Count == 0) return;

        Buff randomDebuff = Buffs.Where(buff => buff.isDebuff).FirstOrDefault();

        if (randomDebuff != null) {
            Buffs.Remove(randomDebuff);
        }
    }

    public void RemoveAllBuffs() {
        Buffs.Clear();
    }

    void RemoveAgedBuffs() {
        if (Buffs.Count == 0) return;
        Buffs.RemoveAll(buff => buff.TurnsRemaining < 1);
    }

    public void RestoreStagger() {
        currentStagger = Config.BaseSP;
    }

    public void InitializeMeAtPosition(int battlePosition) {
        isDead = false;
        currentHealth = Config.BaseHP;
        currentStagger = Config.BaseSP;
        Config.AttackTreeLevel = 0;
        Config.SupportTreeLevel = 0;
        BattlefieldPosition = battlePosition;
        if (Config.NativeBuff == NativeBuffOption.VOLCANICBOWEL) {
            AddBuff(new BuffVolcanicBowelSyndrome(this, this, 999));
        }
        if (Config.NativeBuff == NativeBuffOption.PYROPEAKABOO) {
            AddBuff(new BuffPyroPeakboo(this, this, 999));
        }
    }

    public void TurnStart() {
        IsCurrentCombatant = true;
    }

    public void TurnEnd() {
        IsCurrentCombatant = false;
    }

    int GetCriticalRollChance() {
        int CRIT_CHANCE = 5;
        if (Config.SpecialAttack == UserAbilitySelection.SNEAKATTACK && Config.AttackTreeLevel >= 3) {
            CRIT_CHANCE += 15;
        }
        return CRIT_CHANCE;
    }

    float GetCriticalHitModifier() {
        return 1.25f;
    }

    int GetHitChance(bool isHeal) {
        int hitChance = 95;
        if (isHeal) {
            hitChance = 100;
        }
        if (HasBuff<BuffBlinded>()) {
            hitChance -= 30;
        }

        return hitChance;
    }

    public int GetBasicAttackRoll() {
        bool HIT_SUCCESSFUL = TryChance(GetHitChance(false));

        if (!HIT_SUCCESSFUL) {
            return 0;
        }

        if (HasBuff<BuffPolymorph>()) {
            return 1;
        }

        bool DidCrit = TryChance(GetCriticalRollChance());
        int damage = UnityEngine.Random.Range(Config.BaseAttackMin, Config.BaseAttackMax);

        if (DidCrit) {
            damage = (int) (damage * GetCriticalHitModifier());
        }

        return damage;
    }

    public int GetSpecialAttackRoll(bool isAHealRoll) {
        bool HIT_SUCCESSFUL = TryChance(GetHitChance(isAHealRoll));

        if (!HIT_SUCCESSFUL) {
            return 0;
        }

        if (HasBuff<BuffPolymorph>()) {
            return 1;
        }

        bool DidCrit = TryChance(GetCriticalRollChance());
        int damage = UnityEngine.Random.Range(Config.BaseSpecialMin, Config.BaseSpecialMax);

        if (DidCrit) {
            damage = (int) (damage * GetCriticalHitModifier());
        }

        return damage;
    }

    public void TakeDamage(int Damage) {
        bool startedDead = currentHealth == 0;
        int DamageToHealth = Damage;

        //  let shield block if there is one
        if (HasBuff<BuffShield>()) {
            int ShieldCharges = Buffs.First(buff => buff is BuffShield).Charges;

            if (ShieldCharges > Damage) {
                Buffs.First(buff => buff is BuffShield).Charges -= Damage;
                DamageToHealth = 0;
            } else {
                DamageToHealth -= ShieldCharges;
                Buffs.RemoveAll(buff => buff is BuffShield);
            }
        }

        currentHealth = Math.Clamp(
            currentHealth - DamageToHealth,
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

    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }

    // dev
    [ContextMenu("check state")]
    void CheckState() {
        Debug.Log(Config.AttackTreeLevel + " ATTACK");
        Debug.Log(Config.SupportTreeLevel + " SUPPORT");
    }
    [ContextMenu("lvl up both")]
    void LvlUpBoth() {
        Config.AttackTreeLevel++;
        Config.SupportTreeLevel++;
    }
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
