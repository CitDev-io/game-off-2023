using System.Collections.Generic;

public class AbilityBasicAttack : Effect
{
    public AbilityBasicAttack()
    {
        Name = "Basic Attack";
        Description = "A basic attack.";
        TargetScope = EligibleTargetScopeType.ENEMY;
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
