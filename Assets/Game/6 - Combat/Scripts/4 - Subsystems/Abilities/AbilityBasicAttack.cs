public class AbilityBasicAttack : Ability
{
    public AbilityBasicAttack()
    {
        Name = "Basic Attack";
        Description = "A basic attack.";
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/BasicAttack");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            target,
            source.Config.PowerType,
            source.GetBasicAttackRoll()
        );

        _e.Add(DamageToTarget);

        return _e;
    }
}
