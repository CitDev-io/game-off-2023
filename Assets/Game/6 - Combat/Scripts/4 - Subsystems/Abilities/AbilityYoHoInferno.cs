using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityYoHoInferno : Effect
{
    public AbilityYoHoInferno()
    {
        Name = "Yo-Ho Inferno";
        Description = "Double attacks its target";
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

        int AttackDamage1 = source.GetSpecialAttackRoll(false);

        DamageOrder cd1 = new DamageOrder(
            source,
            RandomEnemy,
            AttackDamage1,
            this
        );
        _e.Add(cd1);

        int AttackDamage2 = source.GetSpecialAttackRoll(false);

        DamageOrder cd2 = new DamageOrder(
            source,
            RandomEnemy,
            AttackDamage2,
            this
        );
        _e.Add(cd2);

        return _e;
    }
}
