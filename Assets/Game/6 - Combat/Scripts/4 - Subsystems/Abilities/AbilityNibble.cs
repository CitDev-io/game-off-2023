using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityNibble : Effect
{
    public AbilityNibble()
    {
        Name = "Nibble";
        Description = "Takes a bite out of the enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        Character RandomEnemy = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );
        int AttackDamage = source.GetSpecialAttackRoll(false);

        DamageOrder calcDmg = new DamageOrder(
            source,
            RandomEnemy,
            (int) (AttackDamage * 1.5f),
            this
        );
        _e.Add(calcDmg);

        return _e;
    }
}
