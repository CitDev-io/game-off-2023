using System.Collections.Generic;

public class AbilityCelestialBarrage : BaseAbilityResolver
{
    public AbilityCelestialBarrage() {
        Name = "Celestial Barrage";
        Description = "Beckons 8 Celestial Light Strikes to rain down on random enemies";
        IsUltimate = true;
        TargetScope = EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    int COUNT_LIGHT_STRIKES = 8;
    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        for (var i =0; i < COUNT_LIGHT_STRIKES; i++) {
            Character RandomEnemy = CombatantListFilter.RandomByScope(
                AllCombatants,
                source,
                EligibleTargetScopeType.ENEMY
            );
            int DamageRoll = source.GetSpecialAttackRoll(false);
            int NerfedABitDamage = (int) (DamageRoll * 0.25f);
            CalculatedDamage DamageToTarget = CalculateFinalDamage(
                source,
                RandomEnemy,
                NerfedABitDamage
            );
            
            _e.Add(DamageToTarget);
        }

        return _e;
    }     
}
