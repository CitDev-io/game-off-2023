public delegate void StringDelegate(string String);
public delegate void CharacterDelegate(Character character);
public delegate void CombatPhaseDelegate(CombatPhase phase);
public delegate void StandardDelegate();

public class EventProvider
{
    public CharacterDelegate OnCharacterTurnStart;
    public CombatPhaseDelegate OnPhaseAwake;
    public CombatPhaseDelegate OnPhasePrompt;
    public CombatPhaseDelegate OnPhaseExiting;
    public StandardDelegate OnCombatHasEnded;
    public StandardDelegate OnBoonOffer;
}
