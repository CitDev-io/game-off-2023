using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatState
{
    public BaseAbilityResolver AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    Queue<Character> TurnOrder = new Queue<Character>();
    public List<Character> FullCombatantList = new List<Character>();

    public void ClearWaveCounters() {
        foreach (Character combatant in FullCombatantList) {
            combatant.GenericWaveCounter = 0;
        }
    }

    public Queue<Character> GetTurnOrder() {
        return TurnOrder;
    }

    public void ClearTurnOrder() {
        TurnOrder.Clear();
    }

    public void AddCharacterToTurnOrder(Character character) {
        TurnOrder.Enqueue(character);
    }

    void PurgeDeadCombatantsFromQueue() {
        TurnOrder = new Queue<Character>(TurnOrder.ToList().FindAll(combatant => !combatant.isDead));
    }

    public void MoveToNextCombatant() {
        PurgeDeadCombatantsFromQueue();
        if (CurrentCombatant != null) CurrentCombatant.TurnEnd();

        CurrentCombatant = TurnOrder.Dequeue();

        if (!TurnOrder.Contains(CurrentCombatant)){
            TurnOrder.Enqueue(CurrentCombatant);
        }
    }

    public void AddCharacterTurnNext(Character character) {
        Queue<Character> newQueue = new Queue<Character>();
        newQueue.Enqueue(character);
        foreach (Character combatant in GetTurnOrder()) {
            newQueue.Enqueue(combatant);
        }
        TurnOrder = newQueue;
    }

    public void ClearSelections() {
        AbilitySelected = null;
        TargetSelected = null;
    }

    public ExecutedAbility ExecuteSelectedAbility() {
        return AbilitySelected
            .Resolve(
                CurrentCombatant,
                TargetSelected,
                FullCombatantList
            );
    }

    public List<Character> GetEligibleTargetsForSelectedAbility() {
        return CombatantListFilter.ByScope(
            FullCombatantList,
            CurrentCombatant,
            AbilitySelected.TargetScope
        );
    }

    public List<Character> GetAlivePCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && !combatant.isDead);
    }

    public List<Character> GetAliveCPUs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && !combatant.isDead);
    }

    public List<Character> GetDefeatedPCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && combatant.isDead);
    }

    public List<Character> GetDefeatedCPUs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && combatant.isDead);
    }

    public List<Character> GetAllPCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER);
    }
}
