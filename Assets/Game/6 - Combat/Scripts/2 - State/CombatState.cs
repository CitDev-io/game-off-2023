using System.Collections.Generic;

public class CombatState
{
    public AttackType AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    public Queue<Character> TurnOrder = new Queue<Character>();

}
