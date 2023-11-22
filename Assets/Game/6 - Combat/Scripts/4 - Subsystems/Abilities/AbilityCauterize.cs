using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCauterize : BaseAbilityResolver
{
    public AbilityCauterize()
    {
        Name = "Cauterize";
        Description = "Heals an ally";
        TargetScope = EligibleTargetScopeType.FRIENDLYORSELF;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        Character RandomAlly = GetRandomFriendlyDamaged(source, AllCombatants);

        int HealAmount = -source.GetSpecialAttackRoll(true);
        int AdjustedHeal = (int) (HealAmount * 0.25f);

        Character healTarget = RandomAlly ?? source;

        CalculatedDamage calcDmg = CalculateFinalDamage(
            source,
            healTarget,
            AdjustedHeal
        );
        _e.Add(calcDmg);

        return _e;
    }
}
