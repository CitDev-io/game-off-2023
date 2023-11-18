using System.Collections.Generic;

public class AbilityFlatDotDamage : BaseAbilityResolver
{
    int TickDamage = 0;
    public AbilityFlatDotDamage(int tickDamage, string name = "Poison Effect", string description = "Poisoned Enemy does damage at the start of their turn") {
        Name = name;
        Description = description;
        TickDamage = tickDamage;
        TargetScope = EligibleTargetScopeType.ANYALIVE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DotDamage");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            TickDamage
        );

        _e.Add(DamageToTarget);

        return _e;
    }     
}
