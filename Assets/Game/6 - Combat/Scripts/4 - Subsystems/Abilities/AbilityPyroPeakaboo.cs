using System.Collections.Generic;

public class AbilityPyroPeakaboo : Effect
{
    public AbilityPyroPeakaboo()
    {
        Name = "Pyro Peakaboo";
        Description = "Resurrect self with half health";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, source, this);

        _e.Add(new ReviveOrder(source, 50, this));

        return _e;
    }
}
