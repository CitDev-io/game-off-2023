using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlazeBarrage : BaseAbilityResolver
{
    public AbilityBlazeBarrage()
    {
        Name = "Blaze Barrage";
        Description = "Deals medium damage to 3 different enemies at random";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        List<Character> AllEnemies = CombatantListFilter.ByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );
        int AttackDamage = source.GetSpecialAttackRoll(false);
        bool AttackLanded = AttackDamage != 0;
        int AdjustedDamage = (int) (AttackDamage * 0.5f);

        if (!AttackLanded) {
            return _e;
        }

        List<Character> Targets = new List<Character>();
        if (AllEnemies.Count < 4) {
            Targets = AllEnemies;
        } else {
            while (AllEnemies.Count > 3) {
                int RandomIndex = Random.Range(0, AllEnemies.Count);
                AllEnemies.RemoveAt(RandomIndex);
            }
            Targets = AllEnemies;
        }

        foreach(var Target in Targets) {
            CalculatedDamage calcDmg = CalculateFinalDamage(
                source,
                Target,
                AdjustedDamage
            );
            _e.Add(calcDmg);
        }

        return _e;
    }
}
