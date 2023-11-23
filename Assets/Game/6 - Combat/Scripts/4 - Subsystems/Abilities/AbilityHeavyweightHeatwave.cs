using System.Collections.Generic;
using UnityEngine;

public class AbilityHeavyweightHeatwave : Effect
{
    public AbilityHeavyweightHeatwave() {
        Name = "Heavyweight Heatwave";
        Description = "Casts Heatwave on self";
        TargetScope = EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, source, this);

        List<Character> AllEnemies = CombatantListFilter.ByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );

        Buff hwBuff = new BuffHeatwave(source, source, 3, 10, AllEnemies);

        _e.Add(hwBuff);

        return _e;
    }     
}
