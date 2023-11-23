using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFlareBlitz : Effect
{
    public AbilityFlareBlitz()
    {
        Name = "Flare Blitz";
        Description = "Blasts two adjacent enemies with fire";
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
            int AdjustedDamage = (int) (AttackDamage * 0.75f);
            Character catchingAStray = GetRandomNearbyAllyOfCharacter(RandomEnemy, AllCombatants);
            
            DamageOrder calcDmg = new DamageOrder(
                source,
                RandomEnemy,
                AdjustedDamage,
                this
            );
            _e.Add(calcDmg);

            if (catchingAStray != null) {
                DamageOrder dmg2 = new DamageOrder(
                    source,
                    catchingAStray,
                    AdjustedDamage,
                    this
                );
                _e.Add(dmg2);
            }
        }

        return _e;
    }
}
