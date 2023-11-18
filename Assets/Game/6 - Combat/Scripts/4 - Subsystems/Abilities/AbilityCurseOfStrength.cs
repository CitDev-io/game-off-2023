using System;
using System.Collections.Generic;

public class AbilityCurseOfStrength : BaseAbilityResolver
{
    public AbilityCurseOfStrength() {
        Name = "Curse of Strength";
        Description = "Curses an Enemy and Empowers an Ally";
        TargetScope = EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        Character RandomEnemy = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );
        if (RandomEnemy != null) {
            Buff weaknessDebuff = new BuffWeakness(source, RandomEnemy, 1);
            _e.Add(weaknessDebuff);
        }

        Character RandomAlly = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.FRIENDLYORSELF
        );
        if (RandomAlly != null) {
            Buff strengthenBuff = new BuffStrengthen(source, RandomAlly, 1);
            _e.Add(strengthenBuff);
        }

        return _e;
    } 
}
