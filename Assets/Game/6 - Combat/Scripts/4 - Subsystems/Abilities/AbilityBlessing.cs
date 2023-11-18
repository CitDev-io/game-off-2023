using System.Collections.Generic;

public class AbilityBlessing : BaseAbilityResolver
{
    public AbilityBlessing()
    {
        Name = "Blessing";
        Description = "Heal a friendly target";
        TargetScope = EligibleTargetScopeType.FRIENDLYORSELF;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            -source.GetSpecialAttackRoll()
        );

        _e.Add(DamageToTarget);

        return _e;
    }
}
