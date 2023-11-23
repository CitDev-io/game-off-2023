using System.Collections.Generic;
using UnityEngine;

public class AbilityCounterbark : BaseAbilityResolver
{
    public AbilityCounterbark() {
        Name = "Counterbark";
        Description = "Casts a Counterattack on self";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        int AbilityRoll = source.GetSpecialAttackRoll(false);
        bool AbilityLanded = AbilityRoll != 0;
        
        if (AbilityLanded) {
            Buff cbBuff = new BuffCounterattack(source, source, 1);
            _e.Add(cbBuff);
        }

        return _e;
    }     
}
