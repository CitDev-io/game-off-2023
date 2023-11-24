using System.Collections.Generic;
using UnityEngine;

public class AbilityCounterbark : Effect
{
    public AbilityCounterbark() {
        Name = "Counterbark";
        Description = "Casts a Counterattack on self";
        TargetScope = EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, source, this);

        Buff cbBuff = new BuffImprovedCounterAttack(source, source, 2);
        _e.Add(cbBuff);

        return _e;
    }     
}
