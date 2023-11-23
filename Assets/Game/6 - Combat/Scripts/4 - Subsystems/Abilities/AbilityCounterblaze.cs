using System.Collections.Generic;
using UnityEngine;

public class AbilityCounterblaze : Effect
{
    public AbilityCounterblaze() {
        Name = "Counterblaze";
        Description = "Casts Improved Counterattack on self";
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
