using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFlareBlitz : BaseAbilityResolver
{
    public AbilityFlareBlitz()
    {
        Name = "Flare Blitz";
        Description = "Blasts two adjacent enemies with fire";
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
        bool AttackLanded = AttackDamage != 0;
        

        if (AttackLanded) {
            int AdjustedDamage = (int) (AttackDamage * 0.75f);
            Character catchingAStray = GetRandomNearbyAllyOfCharacter(RandomEnemy, AllCombatants);
            
            CalculatedDamage calcDmg = CalculateFinalDamage(
                source,
                RandomEnemy,
                AdjustedDamage
            );
            _e.Add(calcDmg);

            if (catchingAStray != null) {
                CalculatedDamage dmg2 = CalculateFinalDamage(
                    source,
                    catchingAStray,
                    AdjustedDamage
                );
                _e.Add(dmg2);
            }
        }

        return _e;
    }
}
