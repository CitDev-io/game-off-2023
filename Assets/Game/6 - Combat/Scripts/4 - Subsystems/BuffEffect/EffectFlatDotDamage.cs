using System.Collections.Generic;

public class EffectFlatDotDamage : Effect
{
    int TickDamage = 0;
    public EffectFlatDotDamage(int tickDamage, string name = "Poison Effect", string description = "Poisoned Enemy does damage at the start of their turn") {
        Name = name;
        Description = description;
        TickDamage = tickDamage;
        TargetScope = EligibleTargetScopeType.ANYALIVE;
        IsAbility = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/DotDamage");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        DamageOrder DamageToTarget = new DamageOrder(
            source,
            target,
            TickDamage,
            this
        );

        _e.Add(DamageToTarget);

        return _e;
    }     
}
