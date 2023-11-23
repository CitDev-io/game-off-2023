using System.Collections.Generic;
using UnityEngine;

public class AbilityTimberTip : Effect
{
    public AbilityTimberTip() {
        Name = "Timber Tip";
        Description = "Tips the balance of Light and Shadow and attacks an enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        int AbilityRoll = source.GetSpecialAttackRoll(false);
        bool AbilityLanded = AbilityRoll != 0;

        if (AbilityLanded) {
            int LightPtAdjustment = -2;
            int ShadowPtAdjustment = 2;

            if (source.Config.PowerType == PowerType.LIGHT) {
                LightPtAdjustment = 2;
                ShadowPtAdjustment = -2;
            }
            _e.Add(new ScaleOrder(
                LightPtAdjustment,
                ShadowPtAdjustment
            ));

            DamageOrder DamageToTarget = new DamageOrder(
                source,
                target,
                AbilityRoll,
                this
            );
            _e.Add(DamageToTarget);
        }

        return _e;
    }     
}
