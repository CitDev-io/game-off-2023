public class AbilitySneakAttack : Ability
{
    public AbilitySneakAttack() {
        Name = "Poison Strike";
        Description = "Attacks an enemy and applies Poisoned";
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/PoisonStrike");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            target,
            source.Config.PowerType,
            source.GetSpecialAttackRoll()
        );

        _e.Add(DamageToTarget);

        int PoisonDamage = DamageToTarget.RawDamage / 2;

        Buff poisonDebuff = new BuffPoisoned(source, target, 1, PoisonDamage);
        _e.Add(poisonDebuff);

        return _e;
    }    
}
