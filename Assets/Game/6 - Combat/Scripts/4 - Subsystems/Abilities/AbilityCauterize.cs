using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCauterize : Effect
{
    public AbilityCauterize()
    {
        Name = "Cauterize";
        Description = "Heals an ally";
        TargetScope = EligibleTargetScopeType.FRIENDLYORSELF;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        Character RandomAlly = GetRandomFriendlyDamaged(source, AllCombatants);

        int HealAmount = -source.GetSpecialAttackRoll(true);
        int AdjustedHeal = (int) (HealAmount * 0.25f);

        Character healTarget = RandomAlly ?? source;

        DamageOrder calcDmg = new DamageOrder(
            source,
            healTarget,
            AdjustedHeal,
            this
        );
        _e.Add(calcDmg);

        return _e;
    }
}
