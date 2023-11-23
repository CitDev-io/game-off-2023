using System.Collections.Generic;

public class AbilityHollowHowl : Effect
{
    public AbilityHollowHowl()
    {
        Name = "Hollow Howl";
        Description = "Summons an ally for aid";
        TargetScope = EligibleTargetScopeType.NONE;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        int FriendlyUnitCount = CombatantListFilter.ByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.FRIENDLYORSELF
        ).Count;

        var _e = new EffectPlan(source, target, this);

        if (FriendlyUnitCount > 4) {
            UnityEngine.Debug.Log("TOO MANY");
            return _e;
        }
        
        _e.Add(
            new SummonOrder(
                SummonableUnit.GHOST,
                source.Config.TeamType,
                this
            )
        );

        return _e;
    }
}
