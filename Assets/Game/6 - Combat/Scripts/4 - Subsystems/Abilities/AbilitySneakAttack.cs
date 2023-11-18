using System.Collections.Generic;

public class AbilitySneakAttack : BaseAbilityResolver
{
    public AbilitySneakAttack() {
        Name = "Poison Strike";
        Description = "Attacks an enemy and applies Poisoned";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/PoisonStrike");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            source.GetSpecialAttackRoll()
        );

        _e.Add(DamageToTarget);

        int PoisonDamage = DamageToTarget.RawDamage / 2;

        Buff poisonDebuff = new BuffPoisoned(source, target, 1, PoisonDamage);
        _e.Add(poisonDebuff);

        return _e;
    }    
}
