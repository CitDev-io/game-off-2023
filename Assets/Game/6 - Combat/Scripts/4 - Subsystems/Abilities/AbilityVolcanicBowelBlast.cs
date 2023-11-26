using System.Collections.Generic;
using System.Diagnostics;

public class AbilityVolcanicBowelBlast : Effect
{
    public AbilityVolcanicBowelBlast()
    {
        Name = "Volcanic Bowel Blast";
        Description = "Blasts fire, dealing damage to all nearby allies";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, null, this);

        List<Character> NearbyAllies = GetNearbyAlliesOfCharacter(source, AllCombatants);

        if (NearbyAllies.Count == 0) {
            return _e;
        }

        foreach(Character Ally in NearbyAllies) {
            DamageOrder DamageToTarget = new DamageOrder(
                source,
                Ally,
                source.GetSpecialAttackRoll(false) / 4,
                this
            );
            _e.Add(DamageToTarget);
        }

        return _e;
    }
}
