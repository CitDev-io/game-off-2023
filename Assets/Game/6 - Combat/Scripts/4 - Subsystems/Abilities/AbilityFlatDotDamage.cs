public class AbilityFlatDotDamage : Ability
{
    int TickDamage = 0;
    public AbilityFlatDotDamage(int tickDamage, string name = "Poison Effect", string description = "Poisoned Enemy does damage at the start of their turn") {
        Name = name;
        Description = description;
        TickDamage = tickDamage;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DotDamage");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            target,
            source.Config.PowerType,
            TickDamage
        );

        _e.Add(DamageToTarget);

        return _e;
    }     
}
