using System.Collections.Generic;

public class AbilityPrayer : Effect
{
    Character AllyToRevive;

    public AbilityPrayer(Character ally)
    {
        Name = "Prayer";
        Description = "Resurrect fallen ally";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = false;

        AllyToRevive = ally;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, null, this);

        if (AllyToRevive != null) {
            _e.Add(new ReviveOrder(AllyToRevive, 50, this));
        }

        return _e;
    }
}
