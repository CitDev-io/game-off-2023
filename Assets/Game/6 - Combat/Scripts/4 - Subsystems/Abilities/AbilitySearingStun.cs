using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySearingStun : Effect
{
    public AbilitySearingStun() {
        Name = "Searing Stun";
        Description = "Turns enemy to stone, stunning them";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        int BashDamage = source.GetSpecialAttackRoll(false);
        bool BashLanded = BashDamage != 0;
        
        DamageOrder DamageToTarget = new DamageOrder(
            source,
            target,
            BashDamage,
            this
        );

        _e.Add(DamageToTarget);

        if (BashLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 2);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
