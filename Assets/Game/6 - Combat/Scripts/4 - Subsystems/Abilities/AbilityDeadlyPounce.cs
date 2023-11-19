using System.Collections.Generic;

public class AbilityDeadlyPounce : BaseAbilityResolver
{
    public AbilityDeadlyPounce() {
        Name = "Deadly Pounce";
        Description = "Deals a massive blow and stuns the enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DeadlyPounce");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);
        int DamageRoll = 2 * source.GetSpecialAttackRoll(false);
        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            DamageRoll
        );

        _e.Add(DamageToTarget);

        bool HitLanded = DamageRoll != 0;
        
        if (HitLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 1);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
