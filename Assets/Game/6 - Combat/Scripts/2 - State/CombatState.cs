using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState
{
    public AttackType AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    public Queue<Character> TurnOrder = new Queue<Character>();

}
