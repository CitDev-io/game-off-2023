using System.Collections.Generic;

public delegate void StringDelegate(string String);
public delegate void CharacterDelegate(Character character);
public delegate void CharactersDelegate(List<Character> character);
public delegate void CombatPhaseDelegate(CombatPhase phase, Character combatant);
public delegate void StandardDelegate();
public delegate void BoolDelegate(bool boolean);
public delegate void ExecutedAbilityDelegate(ExecutedAbility executedAbility);

public class EventProvider
{
    public CharacterDelegate OnCharacterTurnStart;
    public CombatPhaseDelegate OnPhaseAwake;
    public CombatPhaseDelegate OnPhasePrompt;
    public CombatPhaseDelegate OnPhaseExiting;
    public StandardDelegate OnCombatHasEnded;
    public StandardDelegate OnBoonOffer;
    public StandardDelegate OnWaveReady;
    public BoolDelegate OnInput_CombatantChoseAbility;
    public CharacterDelegate OnInput_CombatantChoseTarget;
    public CharactersDelegate OnEligibleTargetsChanged;
    public ExecutedAbilityDelegate OnAbilityExecuted;
    public CharacterDelegate OnCharacterRevived;
}
