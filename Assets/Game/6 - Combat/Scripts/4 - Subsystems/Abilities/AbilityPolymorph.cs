using System.Collections.Generic;

public class AbilityPolymorph : Effect
{
    public AbilityPolymorph() {
        Name = "Polymorph";
        Description = "Transforms an enemy into a harmless sheep and negates elemental resistances for 2 turns";
        TargetScope = EligibleTargetScopeType.ENEMY;
        IsUltimate = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Polymorph");");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        bool PolymorphLanded = source.GetSpecialAttackRoll(false) != 0;

        if (!PolymorphLanded) {
            return _e;
        }

        Buff PolymorphDebuff = new BuffPolymorph(source, target, 2);
        _e.Add(PolymorphDebuff);

        Buff EWDebuff = new BuffElementalWeakness(source, target, 2);
        _e.Add(EWDebuff);

        return _e;
    }     
}
