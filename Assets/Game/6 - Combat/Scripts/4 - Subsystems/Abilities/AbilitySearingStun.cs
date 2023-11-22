using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilitySearingStun : BaseAbilityResolver
{
    public AbilitySearingStun(int AttackLevel = 0, int SupportLevel = 0) {
        Name = "Searing Stun";
        Description = "Turns enemy to stone, stunning them";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        int BashDamage = source.GetSpecialAttackRoll(false);
        bool BashLanded = BashDamage != 0;
        
        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            BashDamage
        );

        _e.Add(DamageToTarget);

        if (BashLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 2);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
