using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityResolver
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }
    public EligibleTargetScopeType TargetScope { get; protected set; }

    public abstract ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null);

    // don't change this signature. would be nice to not pass null when you know it's safe, but do it anyway
    public ExecutedAbility Resolve(Character source, Character target, List<Character> eligibleTargets) {
        ExecutedAbility executedAbility = GetUncommitted(source, target, eligibleTargets);
        return executedAbility.Commit();
    }

    protected int GetUnmitigatedDamageFromRaw(int rawDamage, Character target, PowerType effectPowerType) {
        // mitigation is zero if rawDamage is negative, this is a heal
        if (rawDamage < 0) {
            return rawDamage;
        }

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

    protected PowerType GetPowerTypeOfCharacter(Character character) {
        return character.Config.PowerType;
    }

    protected CalculatedDamage CalculateFinalDamage(
        Character source,
        Character victim,
        int rawDamage
    ) {
        int sourceRawDamage = rawDamage;

        if (source.HasBuff<BuffWeakness>()) {
            sourceRawDamage = (int) (sourceRawDamage * 0.5f);
        }

        if (source.HasBuff<BuffStrengthen>()) {
            sourceRawDamage *= 2;
        }

        int unmitigatedDamage = GetUnmitigatedDamageFromRaw(
            sourceRawDamage,
            victim,
            GetPowerTypeOfCharacter(source)
        );
        if (victim.Config.BaseSP == 0) {
            return new CalculatedDamage(
                victim,
                unmitigatedDamage,
                0,
                sourceRawDamage,
                false
            );
        }

        bool IsAHeal = sourceRawDamage < 0;
        bool StaggerNotInvolved = IsAHeal || IsVictimResistantToPowerType(victim, GetPowerTypeOfCharacter(source));

        int DamageDealtToStagger = StaggerNotInvolved ? 0 : sourceRawDamage;

        bool CharacterBeganCracked = victim.currentStagger == 0;
        bool CharacterIsCracked = victim.currentStagger <= DamageDealtToStagger;


        int FinalDamageToHealth = unmitigatedDamage;
        bool FinalDamageShouldBeHalved = CharacterIsCracked && !IsAHeal;
        if (FinalDamageShouldBeHalved) {
            FinalDamageToHealth = (int) (unmitigatedDamage / 2f);
        }

        bool CharacterCrackedThisTurn = !CharacterBeganCracked && CharacterIsCracked;

        CalculatedDamage result = new CalculatedDamage(
            victim,
            FinalDamageToHealth,
            DamageDealtToStagger,
            sourceRawDamage,
            CharacterCrackedThisTurn
        );

        return result;
    }

    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }
}
