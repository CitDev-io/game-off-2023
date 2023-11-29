using System.Collections.Generic;

public delegate void StringDelegate(string String);
public delegate void CharacterDelegate(Character character);
public delegate void CharactersDelegate(List<Character> character);
public delegate void CombatPhaseDelegate(CombatPhase phase, Character combatant);
public delegate void StandardDelegate();
public delegate void BoolDelegate(bool boolean);
public delegate void AbilityCategoryDelegate(AbilityCategory category);
public delegate void EffectPlanDelegate(EffectPlan executedAbility);
public delegate void BoonsDelegate(List<BaseBoonResolver> boons);
public delegate void BoonDelegate(BaseBoonResolver boon);
public delegate void ScaleDelegate(int light, int dark);
public delegate void CalculatedDamageDelegate(CalculatedDamage cd);
public delegate void BuffDelegate(Buff buff);
public delegate void TurnOrderDelegate(Character character, List<Character> InQueue);
public delegate void StageDelegate(StageConfig stageConfig);
public delegate void IntDelegate(int integer);
public delegate void WaveInfoDelegate(WaveInfo waveInfo);

public class EventProvider
{
    public CalculatedDamageDelegate OnDamageResolved;
    public BuffDelegate OnBuffAdded;
    public BuffDelegate OnBuffExpired;
    public CharacterDelegate OnCharacterRevived;
    public ScaleDelegate OnScaleChanged;
    public EffectPlanDelegate OnEffectPlanExecutionStart;
    public EffectPlanDelegate OnEffectPlanExecutionComplete;
    public TurnOrderDelegate OnTurnOrderChanged;
    public StandardDelegate OnReviveOffered;
    public IntDelegate OnGameOver;
    public StandardDelegate OnGameVictory;
    public StandardDelegate OnWaveComplete;
    public IntDelegate OnWaveVictory;

    public WaveInfoDelegate OnStageComplete;
    public CharacterDelegate OnCharacterSummoned;
    public StageDelegate OnStageSetup;

    public CombatPhaseDelegate OnPhaseAwake;
    public CombatPhaseDelegate OnPhasePrompt;
    public CombatPhaseDelegate OnPhaseExiting;
    public StandardDelegate OnCombatHasEnded;
    public BoonsDelegate OnBoonOffer;
    public WaveInfoDelegate OnWaveSetupStart;
    public WaveInfoDelegate OnWaveReady;
    public CharactersDelegate OnEligibleTargetsChanged;

    public StandardDelegate OnInput_RetryResponse;
    public BoolDelegate OnInput_ReviveResponse;
    public StandardDelegate OnInput_BackOutOfTargetSelection;
    public AbilityCategoryDelegate OnInput_CombatantChoseAbility;
    public CharacterDelegate OnInput_CombatantChoseTarget;
    public BoonDelegate OnInput_BoonSelected;
}
