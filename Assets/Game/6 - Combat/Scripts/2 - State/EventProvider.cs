using System.Collections.Generic;

public delegate void StringDelegate(string String);
public delegate void CharacterDelegate(Character character);
public delegate void CharactersDelegate(List<Character> character);
public delegate void CombatPhaseDelegate(CombatPhase phase, Character combatant);
public delegate void StandardDelegate();
public delegate void BoolDelegate(bool boolean);
public delegate void AbilityCategoryDelegate(AbilityCategory category);
public delegate void ExecutedAbilityDelegate(ExecutedAbility executedAbility);
public delegate void BoonsDelegate(List<BaseBoonResolver> boons);
public delegate void BoonDelegate(BaseBoonResolver boon);

public class EventProvider
{
    public CombatPhaseDelegate OnPhaseAwake;
    public CombatPhaseDelegate OnPhasePrompt;
    public CombatPhaseDelegate OnPhaseExiting;
    public StandardDelegate OnCombatHasEnded;
    public BoonsDelegate OnBoonOffer;
    public StandardDelegate OnWaveReady;
    public AbilityCategoryDelegate OnInput_CombatantChoseAbility;
    public CharacterDelegate OnInput_CombatantChoseTarget;
    public BoonDelegate OnInput_BoonSelected;
    public CharactersDelegate OnEligibleTargetsChanged;
    public ExecutedAbilityDelegate OnAbilityExecuted;
    public CharacterDelegate OnCharacterRevived;
}
