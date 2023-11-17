public class AbilityShieldBash : Ability
{
    public AbilityShieldBash() {
        Name = "Shield Bash";
        Description = "Bashes an enemy upside the head";
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
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

        bool StunLanded = TryChance(75);

        if (StunLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 1);
            _e.Add(stunDebuff);
        }

        return _e;
    }     
}
