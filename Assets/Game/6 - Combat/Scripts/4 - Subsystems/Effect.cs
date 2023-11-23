using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Effect
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }
    public EligibleTargetScopeType TargetScope { get; protected set; }
    public bool IsUltimate = false;
    public bool IsAbility = true;

    public abstract EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants);

    protected Character GetRandomFriendlyOrSelf(Character character, List<Character> AllCombatants) {
        return CombatantListFilter.RandomByScope(
            AllCombatants,
            character,
            EligibleTargetScopeType.FRIENDLYORSELF
        );
    }

    protected Character GetRandomFriendlyDamaged(Character character, List<Character> AllCombatants) {
        var damaged = CombatantListFilter.ByScope(
            AllCombatants,
            character,
            EligibleTargetScopeType.FRIENDLYORSELF
        ).Where(candidate => candidate.currentHealth < candidate.Config.BaseHP).ToList();

        if (damaged.Count == 0) {
            return null;
        }
        return damaged[UnityEngine.Random.Range(0, damaged.Count)];
    }

    protected Character GetRandomNearbyAllyOfCharacter(Character character, List<Character> AllCombatants) {
        List<Character> nearby = GetNearbyAlliesOfCharacter(character, AllCombatants);

        if (nearby.Count == 0) {
            return null;
        }

        return nearby[UnityEngine.Random.Range(0, nearby.Count)];
    }

    protected List<Character> GetNearbyAlliesOfCharacter(Character character, List<Character> AllCombatants) {
        return CombatantListFilter.ByScope(
                AllCombatants,
                character,
                EligibleTargetScopeType.ANYOTHERALLY
            ).Where(candidate => Mathf.Abs(candidate.PositionInfo.SpotId - character.PositionInfo.SpotId) <= 1).ToList();
    }

    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }
}
