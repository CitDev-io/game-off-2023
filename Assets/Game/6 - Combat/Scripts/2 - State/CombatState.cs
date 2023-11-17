using System.Collections.Generic;
using System.Linq;

public class CombatState
{
    public Ability AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    Queue<Character> TurnOrder = new Queue<Character>();

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

    public ExecutedAbility ExecuteSelectedAbility() {
        return AbilitySelected
            .Resolve(
                CurrentCombatant,
                TargetSelected
            );
    }
}
