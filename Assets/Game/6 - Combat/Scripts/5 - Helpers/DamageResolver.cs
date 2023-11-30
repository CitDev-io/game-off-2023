using System;

public class DamageResolver {
    public CalculatedDamage ResolveOrder(DamageOrder order) {
        CalculatedDamage damage = CalculateDamageResult(order);

        damage.Target.TakeDamage(damage.DamageToHealth);
        damage.Target.TakeStagger(damage.DamageToStagger);

        return damage;
    }

    CalculatedDamage CalculateDamageResult(
        DamageOrder order
    ) {
        Character attacker = order.Attacker;
        Character victim = order.Victim;
        int rawDamage = order.RawDamage;

        int attackerRawDamage = rawDamage;

        if (attacker.HasBuff<BuffWeakness>()) {
            attackerRawDamage = (int) (attackerRawDamage * 0.5f);
        }

        if (attacker.HasBuff<BuffStrengthen>()) {
            attackerRawDamage *= 2;
        }

        int unmitigatedDamage = GetUnmitigatedDamageFromRaw(
            attackerRawDamage,
            victim,
            GetPowerTypeOfCharacter(attacker)
        );

        bool IsAHeal = attackerRawDamage < 0;

        bool IsVulnerableToAttack = victim.HasBuff<BuffElementalVulnerability>() && !IsVictimResistantToPowerType(victim, GetPowerTypeOfCharacter(attacker));

        if (!IsAHeal && IsVulnerableToAttack) {
            unmitigatedDamage = (int) (unmitigatedDamage * 1.25f);
        }

        if (victim.Config.BaseSP == 0) {
            return new CalculatedDamage(
                attacker,
                victim,
                unmitigatedDamage,
                0,
                attackerRawDamage,
                false,
                order.Source
            );
        }

        bool StaggerNotInvolved = IsAHeal || IsVictimResistantToPowerType(victim, GetPowerTypeOfCharacter(attacker));

        int DamageDealtToStagger = StaggerNotInvolved ? 0 : attackerRawDamage;

        bool CharacterBeganCracked = victim.currentStagger == 0;
        bool CharacterIsCracked = victim.currentStagger <= DamageDealtToStagger;
        bool CharacterCrackedThisTurn = !CharacterBeganCracked && CharacterIsCracked;


        int FinalDamageToHealth = unmitigatedDamage;
        bool FinalDamageShouldBeHalved = !CharacterIsCracked && !IsAHeal;
        if (FinalDamageShouldBeHalved) {
            FinalDamageToHealth = (int) (unmitigatedDamage / 2f);
        }


        CalculatedDamage result = new CalculatedDamage(
            attacker,
            victim,
            FinalDamageToHealth,
            DamageDealtToStagger,
            attackerRawDamage,
            CharacterCrackedThisTurn,
            order.Source
        );

        return result;
    }

    int GetUnmitigatedDamageFromRaw(int rawDamage, Character target, PowerType effectPowerType) {
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

    int GetMitigationPowerForPowerType(Character victim, PowerType element) {
        bool IsResistant =  IsVictimResistantToPowerType(victim, element);
        int MitigationPower = IsResistant ? 10 : 0;
        return MitigationPower;
    }

    int GetFullVictimMitigationPower(Character victim, PowerType element) {
        if (victim == null || victim.Config == null) return 0; //WHY WOULD THIS HAPPEN???
        return victim.Config.BaseMitigation + GetMitigationPowerForPowerType(victim, element);
    }

    bool IsVictimResistantToPowerType(Character victim, PowerType element) {
        if (victim.HasBuff<BuffSkeletalShield>()) {
            return true;
        }

        if (victim.HasBuff<BuffElementalWeakness>()) {
            return false;
        }
        bool resistantToPowerType = victim.Config.PowerType == element;
        return resistantToPowerType;
    }

    PowerType GetPowerTypeOfCharacter(Character character) {
        return character.Config.PowerType;
    }
}
