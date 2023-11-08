using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState
{
    // TODO: Will be an Ability Object or NULL
    public string AbilitySelected;
    public Combatant TargetSelected;
    public Combatant CurrentCombatant;
    public Queue<Combatant> TurnOrder = new Queue<Combatant>();
}
