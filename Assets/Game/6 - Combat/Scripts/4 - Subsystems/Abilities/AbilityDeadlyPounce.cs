using System.Collections.Generic;

public class AbilityDeadlyPounce : BaseAbilityResolver
{
    public AbilityDeadlyPounce() {
        Name = "Deadly Pounce";
        Description = "Deals a massive blow and stuns the enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DeadlyPounce");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            2 * source.GetSpecialAttackRoll()
        );

        _e.Add(DamageToTarget);

        
        Buff stunDebuff = new BuffStunned(source, target, 1);
        _e.Add(stunDebuff);

        return _e;
    }     
}
