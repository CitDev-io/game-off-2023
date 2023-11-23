using System.Collections.Generic;

public class AbilityPegLegPlague : Effect
{
    public AbilityPegLegPlague() {
        Name = "Pegleg Plague";
        Description = "Silences the enemy";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Polymorph");");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        bool CastLanded = source.GetSpecialAttackRoll(false) != 0;

        if (!CastLanded) {
            return _e;
        }

        Buff SilenceDebuff = new BuffSilenced(source, target, 2);
        _e.Add(SilenceDebuff);

        return _e;
    }     
}
