using System.Collections.Generic;

public class AbilityPolymorph : BaseAbilityResolver
{
    public AbilityPolymorph() {
        Name = "Polymorph";
        Description = "Transforms an enemy into a harmless sheep and negates elemental resistances for 2 turns";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Polymorph");");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> eligibleTargets = null)
    {
        var _e = new ExecutedAbility(source, target, this);

        Buff PolymorphDebuff = new BuffPolymorph(source, target, 2);
        _e.Add(PolymorphDebuff);

        Buff EWDebuff = new BuffElementalWeakness(source, target, 2);
        _e.Add(EWDebuff);

        return _e;
    }     
}
