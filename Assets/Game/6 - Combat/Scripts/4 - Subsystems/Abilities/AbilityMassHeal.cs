using System.Collections.Generic;

public class AbilityMassHeal : Effect
{
    public AbilityMassHeal()
    {
        Name = "Mass Heal";
        Description = "HEAL all heroes";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        List<Character> FriendlyTargets = CombatantListFilter.ByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.FRIENDLYORSELF
        );

        var _e = new EffectPlan(source, target, this);
        foreach(Character FriendlyTarget in FriendlyTargets) {
            DamageOrder DamageToTarget = new DamageOrder(
                source,
                FriendlyTarget,
                -source.GetSpecialAttackRoll(true) / 2,
                this
            );
            _e.Add(DamageToTarget);
        }

        return _e;
    }
}
