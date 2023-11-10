using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState
{
    // TODO: Will be an Ability Object or NULL
    public string AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    public Queue<Character> TurnOrder = new Queue<Character>();
}
