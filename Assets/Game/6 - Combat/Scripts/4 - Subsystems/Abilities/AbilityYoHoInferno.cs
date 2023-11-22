using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityYoHoInferno : BaseAbilityResolver
{
    public AbilityYoHoInferno()
    {
        Name = "Yo-Ho Inferno";
        Description = "Double attacks its target";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        Character RandomEnemy = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );

        int AttackDamage1 = source.GetSpecialAttackRoll(false);

        CalculatedDamage cd1 = CalculateFinalDamage(
            source,
            RandomEnemy,
            AttackDamage1
        );
        _e.Add(cd1);

        int AttackDamage2 = source.GetSpecialAttackRoll(false);

        CalculatedDamage cd2 = CalculateFinalDamage(
            source,
            RandomEnemy,
            AttackDamage2
        );
        _e.Add(cd2);

        return _e;
    }
}
