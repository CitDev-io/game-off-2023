using System.Collections.Generic;

public class AbilityCounterattack : Effect
{
    public AbilityCounterattack()
    {
        Name = "Counter Attack";
        Description = "Attacks an enemy back when attacked";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsAbility = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/BasicAttack");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        DamageOrder DamageToTarget = new DamageOrder(
            source,
            target,
            source.GetBasicAttackRoll(),
            this
        );

        _e.Add(DamageToTarget);

        return _e;
    }
}
