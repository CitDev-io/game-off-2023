using System.Collections.Generic;

public class AbilityShieldBash : BaseAbilityResolver
{
    public AbilityShieldBash() {
        Name = "Shield Bash";
        Description = "Bashes an enemy upside the head";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            source.GetSpecialAttackRoll()
        );

        _e.Add(DamageToTarget);

        bool StunLanded = TryChance(75);

        if (StunLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 1);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
