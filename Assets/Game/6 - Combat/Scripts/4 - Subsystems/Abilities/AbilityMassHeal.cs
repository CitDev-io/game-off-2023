using System.Collections.Generic;

public class AbilityMassHeal : BaseAbilityResolver
{
    public AbilityMassHeal()
    {
        Name = "Mass Heal";
        Description = "Heals all friendly targets";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        List<Character> FriendlyTargets = CombatantListFilter.ByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.FRIENDLYORSELF
        );

        var _e = new ExecutedAbility(source, target, this);
        foreach(Character FriendlyTarget in FriendlyTargets) {
            CalculatedDamage DamageToTarget = CalculateFinalDamage(
                source,
                FriendlyTarget,
                -source.GetSpecialAttackRoll(true) / 2
            );
            _e.Add(DamageToTarget);
        }

        return _e;
    }
}
