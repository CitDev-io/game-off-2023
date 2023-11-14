public delegate void StringDelegate(string String);
public delegate void CharacterDelegate(Character character);
public delegate void CombatPhaseDelegate(CombatPhase phase, Character combatant);
public delegate void StandardDelegate();
public delegate void BoolDelegate(bool boolean);

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
}
