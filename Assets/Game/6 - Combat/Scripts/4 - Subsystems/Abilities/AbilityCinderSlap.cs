using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCinderSlap : Effect
{
    public AbilityCinderSlap()
    {
        Name = "Cinder Slap";
        Description = "Slaps the enemy, inflicting them with Weakness";
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
        bool AttackLanded = AttackDamage != 0;
        
        if (AttackLanded) {
            DamageOrder calcDmg = new DamageOrder(
                source,
                RandomEnemy,
                AttackDamage,
                this
            );
            _e.Add(calcDmg);

            Buff WeaknessDebuff = new BuffWeakness(source, RandomEnemy, 2);
            _e.Add(WeaknessDebuff);
        }

        return _e;
    }
}
