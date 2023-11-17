using System;
using UnityEngine;

public abstract class Ability
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }

    public abstract ExecutedAbility GetUncommitted(Character source, Character target);

    public ExecutedAbility Resolve(Character source, Character target) {
        ExecutedAbility executedAbility = GetUncommitted(source, target);
        return executedAbility.Commit();
    }

    protected int GetUnmitigatedDamageFromRaw(int rawDamage, Character target, PowerType effectPowerType) {
        int mitigationPower = GetFullVictimMitigationPower(target, effectPowerType);

        int mitigatedDamage = (int) (rawDamage * (mitigationPower / 100f));

        int unmitigatedDamage = Math.Clamp(
            rawDamage - mitigatedDamage,
            0,
            rawDamage
        );

        return unmitigatedDamage;
    }

    protected int GetMitigationPowerForPowerType(Character victim, PowerType element) {
        return IsVictimResistantToPowerType(victim, element) ? 10 : 0;
    }

    protected int GetFullVictimMitigationPower(Character victim, PowerType element) {
        return victim.Config.BaseMitigation + GetMitigationPowerForPowerType(victim, element);
    }

    protected bool IsVictimResistantToPowerType(Character victim, PowerType element) {
        // TODO: Victim may also have buffs!
        bool resistantToPowerType = victim.Config.PowerType == element;
        return resistantToPowerType;
    }

    protected CalculatedDamage CalculateFinalDamage(
        Character victim,
        PowerType effectPowerType,
        int rawDamage
    ) {
        int unmitigatedDamage = GetUnmitigatedDamageFromRaw(rawDamage, victim, effectPowerType);
        if (victim.Config.BaseSP == 0) {
            return new CalculatedDamage(
                victim,
                unmitigatedDamage,
                0,
                rawDamage,
                false
            );
        }

        bool SourceAffectsStagger = !IsVictimResistantToPowerType(victim, effectPowerType);

        int DamageDealtToStagger = SourceAffectsStagger ? rawDamage : 0;

        bool CharacterBeganCracked = victim.currentStagger == 0;
        bool CharacterIsCracked = victim.currentStagger <= DamageDealtToStagger;

        int FinalDamageToHealth = CharacterIsCracked ?
            unmitigatedDamage :
            (int) (unmitigatedDamage / 2f);
        
        CalculatedDamage result = new CalculatedDamage(
            victim,
            FinalDamageToHealth,
            DamageDealtToStagger,
            rawDamage,
            !CharacterBeganCracked && CharacterIsCracked
        );

        return result;
    }

    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }
}
