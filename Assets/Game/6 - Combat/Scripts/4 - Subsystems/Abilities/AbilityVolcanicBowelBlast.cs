using System.Collections.Generic;
using System.Diagnostics;

public class AbilityVolcanicBowelBlast : BaseAbilityResolver
{
    public AbilityVolcanicBowelBlast()
    {
        Name = "Volcanic Bowel Blast";
        Description = "Blasts fire, dealing damage to all nearby allies";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, source, this);

        List<Character> NearbyAllies = GetNearbyAlliesOfCharacter(source, AllCombatants);

        if (NearbyAllies.Count == 0) {
            return _e;
        }

        foreach(Character Ally in NearbyAllies) {
            CalculatedDamage DamageToTarget = CalculateFinalDamage(
                source,
                Ally,
                source.GetSpecialAttackRoll(false) / 4 
            );
            _e.Add(DamageToTarget);
        }

        return _e;
    }
}
