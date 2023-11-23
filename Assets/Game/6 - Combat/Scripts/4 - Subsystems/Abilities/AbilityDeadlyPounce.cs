using System.Collections.Generic;

public class AbilityDeadlyPounce : Effect
{
    public AbilityDeadlyPounce() {
        Name = "Deadly Pounce";
        Description = "Deals a massive blow and stuns the enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DeadlyPounce");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);
        int DamageRoll = 2 * source.GetSpecialAttackRoll(false);
        DamageOrder DamageToTarget = new DamageOrder(
            source,
            target,
            DamageRoll,
            this
        );

        _e.Add(DamageToTarget);

        bool HitLanded = DamageRoll != 0;
        
        if (HitLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 1);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
