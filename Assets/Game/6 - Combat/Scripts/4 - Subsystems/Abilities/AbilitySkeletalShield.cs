using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySkeletalShield : Effect
{
    public AbilitySkeletalShield() {
        Name = "Skeletal Shield";
        Description = "Casts a shield that grants elemental resistance to Light and Shadow";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        int AbilityRoll = source.GetSpecialAttackRoll(false);
        bool AbilityLanded = AbilityRoll != 0;
        
        if (AbilityLanded) {
            Buff ssBuff = new BuffSkeletalShield(source, source, 3);
            _e.Add(ssBuff);
        }

        return _e;
    }     
}
