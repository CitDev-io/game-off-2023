using System.Collections.Generic;

public class AbilityCounterattack : BaseAbilityResolver
{
    public AbilityCounterattack()
    {
        Name = "Count Attack";
        Description = "Attacks an enemy back when attacked";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/BasicAttack");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            source.GetBasicAttackRoll()
        );

        _e.Add(DamageToTarget);

        return _e;
    }
}
