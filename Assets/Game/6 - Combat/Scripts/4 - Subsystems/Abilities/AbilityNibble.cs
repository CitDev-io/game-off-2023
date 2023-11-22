using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNibble : BaseAbilityResolver
{
    public AbilityNibble()
    {
        Name = "Nibble";
        Description = "Takes a bite out of the enemy";
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
        int AttackDamage = source.GetSpecialAttackRoll(false);

        CalculatedDamage calcDmg = CalculateFinalDamage(
            source,
            RandomEnemy,
            (int) (AttackDamage * 1.5f)
        );
        _e.Add(calcDmg);

        return _e;
    }
}
