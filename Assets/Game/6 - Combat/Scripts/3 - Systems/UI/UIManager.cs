using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum SelectionState {
    NONE,
    ABILITY,
    TARGET,
    BOON,
    REVIVE
}

public class UIManager : MonoBehaviour
{
    public bool IsPerforming = false;

    [Header("Temp Plumbing: UI Panels")]
    public UI_BoonMenuManager BoonUI;
    EventProvider _eventProvider;
    // public UI_AbilitySelection AbilitySelectionUI;
    // public UI_TargetSelection TargetSelectionUI;
    public UI_BossTaunt BossTauntUI;
    public UI_BattleMenu AbilityUI;
    public UI_TextCrawler TextCrawlUI;
    public UI_GameOverPanel GameOverUI;
    public UI_ScalePanelManager ScalePanelUI;
    public UI_TurnOrderManager TurnOrderUI;
    public UI_StageCurtain StageCurtainUI;
    public UI_StageNameIntro StageNameUI;
    public SpriteRenderer BackgroundImage;
    public GameObject WinUI;
    public GameObject ReviveUI;
    GameController_DDOL ddol;
    SelectionState CurrentSelectionState = SelectionState.NONE;

    bool BoonTimeout = false;

    void Start() {
        ddol = FindFirstObjectByType<GameController_DDOL>();     
    }

    public void UserRetryResponse() {
        Debug.Log("RETRY");
        ddol.PlaySound("Menu_Select");
        GameOverUI.gameObject.SetActive(false);
        _eventProvider.OnInput_RetryResponse?.Invoke();
    }

    public void UserReviveResponse(bool revive) {
        if (CurrentSelectionState != SelectionState.REVIVE) return;
        Debug.Log("REVIVE");
        ddol.PlaySound("Menu_Select");
        CurrentSelectionState = SelectionState.NONE;
        ReviveUI.SetActive(false);
        _eventProvider.OnInput_ReviveResponse?.Invoke(revive);
    }
    public void TargetSelected(Character target) {
        if (BoonTimeout || CurrentSelectionState != SelectionState.TARGET) {
            return;
        }
        ddol.PlaySound("Menu_Select");
        CurrentSelectionState = SelectionState.NONE;
        _eventProvider.OnInput_CombatantChoseTarget?.Invoke(target);
    }
    public void AbilitySelected(AbilityCategory category) {
        if (BoonTimeout || CurrentSelectionState != SelectionState.ABILITY) {
            return;
        }
        ddol.PlaySound("Menu_Select");
        CurrentSelectionState = SelectionState.NONE;
        _eventProvider.OnInput_CombatantChoseAbility?.Invoke(category);
    }

    public void BoonSelected(BaseBoonResolver boon) {
        if (BoonTimeout || CurrentSelectionState != SelectionState.BOON) {
            return;
        }
        ddol.PlaySound("Menu_Select");
        CurrentSelectionState = SelectionState.NONE;
        BoonTimeout = true;
        StartCoroutine(BoonTimeoutRoutine());
        BoonUI.Dismiss();
        _eventProvider.OnInput_BoonSelected?.Invoke(boon);
    }

    public void RequestBackupToAbilitySelection() {
        ddol.PlaySound("Menu_Cancel");
        CurrentSelectionState = SelectionState.NONE;
        TurnOrderUI.ClearSelection();
        _eventProvider.OnInput_BackOutOfTargetSelection?.Invoke();
    }

    IEnumerator BoonTimeoutRoutine() {
        yield return new WaitForSeconds(3f);
        BoonTimeout = false;
    }

    void Awake()
    {
        _eventProvider = GetComponent<CombatReferee>().eventProvider;
        SetupHooks();
    }

    void Update() {
        if (
            CurrentSelectionState == SelectionState.ABILITY && (
            Input.GetKeyDown(KeyCode.UpArrow)
            ||
            Input.GetKeyDown(KeyCode.W)
        )) {
            AbilityUI.ToggleUp();
        }

        if (
            CurrentSelectionState == SelectionState.ABILITY && (
            Input.GetKeyDown(KeyCode.DownArrow)
            ||
            Input.GetKeyDown(KeyCode.S)
        )) {
            AbilityUI.ToggleDown();
        }

        if (
            CurrentSelectionState == SelectionState.TARGET && (
                Input.GetKeyDown(KeyCode.LeftArrow)
                ||
                Input.GetKeyDown(KeyCode.A)
        )) {
            TurnOrderUI.ToggleLeft();
        }

        if (
            CurrentSelectionState == SelectionState.TARGET && (
                Input.GetKeyDown(KeyCode.RightArrow)
                ||
                Input.GetKeyDown(KeyCode.D)
        )) {
            TurnOrderUI.ToggleRight();
        }

        if (
            CurrentSelectionState == SelectionState.BOON && (
                Input.GetKeyDown(KeyCode.LeftArrow)
                ||
                Input.GetKeyDown(KeyCode.A)
        )) {
            BoonUI.ToggleLeft();
        }

        if (
            CurrentSelectionState == SelectionState.BOON && (
                Input.GetKeyDown(KeyCode.RightArrow)
                ||
                Input.GetKeyDown(KeyCode.D)
        )) {
            BoonUI.ToggleRight();
        }

        if (CurrentSelectionState == SelectionState.TARGET && (
            Input.GetMouseButtonDown(1)
            ||
            Input.GetKeyDown(KeyCode.Escape)
            ||
            Input.GetKeyDown(KeyCode.Backspace)
            ||
            Input.GetKeyDown(KeyCode.X)
        )) {
            RequestBackupToAbilitySelection();
        }
        
        if (
            CurrentSelectionState == SelectionState.ABILITY && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            AbilitySelected(AbilityUI.CurrentlySelected());
        }

        if (
            CurrentSelectionState == SelectionState.TARGET && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            TargetSelected(TurnOrderUI.CurrentSelection);
        }

        if (
            CurrentSelectionState == SelectionState.BOON && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            BoonSelected(BoonUI.CurrentSelection);
        }
    }

    void SetupHooks() {
        _eventProvider.OnBoonOffer += HandleBoonPhasePrompt;
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnEligibleTargetsChanged += HandleEligibleTargetsChanged;
        _eventProvider.OnScaleChanged += HandleScaleChanged;
        _eventProvider.OnDamageResolved += HandleDamageResolved;
        _eventProvider.OnBuffAdded += HandleBuffAdded;
        _eventProvider.OnEffectPlanExecutionStart += HandleEffectPlanExecutionStart;
        _eventProvider.OnTurnOrderChanged += HandleTurnOrderChanged;
        _eventProvider.OnStageSetup += HandleStageSetup;
        _eventProvider.OnReviveOffered += HandleReviveOffered;
        _eventProvider.OnGameOver += HandleGameOver;
        _eventProvider.OnGameVictory += HandleGameVictory;
        _eventProvider.OnWaveComplete += HandleWaveComplete;
        _eventProvider.OnWaveSetupStart += HandleWaveSetupStart;
        _eventProvider.OnStageComplete += HandleStageComplete;
    }

    void HandleWaveComplete() {
        TextCrawlUI.ClearQueue();
        TurnOrderUI.SetSelectionMode(false);
        AbilityUI.gameObject.SetActive(false);
        CurrentSelectionState = SelectionState.NONE;
    }

    void HandleGameVictory() {
        WinUI.SetActive(true);
    }

    void HandleGameOver(int livesLeft) {
        StageCurtainUI.CurtainDown();
        GameOverUI.SetRetries(livesLeft);
        GameOverUI.gameObject.SetActive(true);
    }

    void HandleReviveOffered() {
        CurrentSelectionState = SelectionState.REVIVE;
        ReviveUI.SetActive(true);
    }

    void HandleStageSetup(StageConfig stageConfig) {
        IsPerforming = true;
        if (stageConfig.BackgroundImage != null) {
            BackgroundImage.sprite = stageConfig.BackgroundImage;
        }
        StageNameUI.SetStageInfo(stageConfig);
        StartCoroutine(IntroduceStageRoutine());
    }

    IEnumerator IntroduceStageRoutine() {
        yield return new WaitForSeconds(1f);
        StageCurtainUI.CurtainUp();
        yield return new WaitForSeconds(0.5f);
        StageNameUI.IntroduceStage();
        yield return new WaitForSeconds(4.5f);
        IsPerforming = false;
    }

    void HandleStageComplete(WaveInfo wave) {
        IsPerforming = true;
        StartCoroutine(StageCompleteRoutine(wave));
    }

    IEnumerator StageCompleteRoutine(WaveInfo wave) {
        bool isBossInWave = wave.Boss != null;

        if (isBossInWave) {
            StageCurtainUI.CurtainDown();
            yield return new WaitForSeconds(1.5f);
            BossTauntUI.Taunt(wave.Boss.Portrait, wave.Boss.Name, wave.Boss.WaveDefeatTaunt);

            yield return new WaitForSeconds(8f);
            BossTauntUI.Stow();
        } else {
            yield return new WaitForSeconds(0.5f);
        }
        IsPerforming = false;
    }

    void HandleTurnOrderChanged(Character character, List<Character> InQueue) {
        TurnOrderUI.UpdateTurnOrder(character, InQueue);
    }

    void HandleWaveSetupStart(WaveInfo waveInfo) {
        IsPerforming = true;
        StartCoroutine(EarlyIntroduceWaveRoutine(waveInfo));
    }

    void HandleWaveReady(WaveInfo waveInfo) {
        AbilityUI.gameObject.SetActive(false);
        IsPerforming = true;

        StartCoroutine(IntroduceWaveRoutine(waveInfo));
    }

    IEnumerator EarlyIntroduceWaveRoutine(WaveInfo waveInfo) {
        bool isBossInWave = waveInfo.Boss != null;

        if (isBossInWave) {
            StageCurtainUI.CurtainDown();
            yield return new WaitForSeconds(1.5f);
        }
        IsPerforming = false;
    }

    IEnumerator IntroduceWaveRoutine(WaveInfo waveInfo) {
        bool isBossInWave = waveInfo.Boss != null;

        if (isBossInWave) {
            ddol.PlayMusic("Boss" + waveInfo.StageNumber.ToString());
            yield return new WaitForSeconds(.5f);
            // setup boss UI
            BossTauntUI.Taunt(waveInfo.Boss.Portrait, waveInfo.Boss.Name, waveInfo.Boss.WaveIntroTaunt);

            //wait 
            yield return new WaitForSeconds(8f);
            BossTauntUI.Stow();
        }
        StageCurtainUI.CurtainUp();
        IsPerforming = false;
    }

    void HandlePhasePrompts(CombatPhase phase, Character combatant) {
        if (phase == CombatPhase.INIT) {
            HandleInitPhasePrompt();
        }
        if (phase == CombatPhase.CHARACTERTURN_PREFLIGHT) {
            HandlePreflightPhasePromptForCharacter(combatant);
        }
        if (phase == CombatPhase.CHARACTERTURN_CHOOSEABILITY) {
           HandleAbilityPhasePromptForCharacter(combatant);
        }
        if (phase == CombatPhase.CHARACTERTURN_CHOOSETARGET) {
            HandleTargetPhasePromptForCharacter(combatant);
        }
        if (phase == CombatPhase.CHARACTERTURN_EXECUTION) {
            HandleExecutionPhasePromptForCharacter(combatant);
        }
    }

    int LightRef = 0;
    int ShadowRef = 0;
    void HandleScaleChanged(int Light, int Dark) {
        LightRef = Light;
        ShadowRef = Dark;
        ScalePanelUI.SetScale(Light, Dark);
    }

    void HandleInitPhasePrompt() {

    }

    void HandlePreflightPhasePromptForCharacter(Character combatant) {
        TextCrawlUI.EnqueueMessage(combatant.Config.Name + "'s turn to act!");
    }

    void HandleBoonPhasePrompt(List<BaseBoonResolver> boons) {
        CurrentSelectionState = SelectionState.NONE;
        TurnOrderUI.SetSelectionMode(false);
        AbilityUI.gameObject.SetActive(false);
        BoonUI.OfferBoons(boons);
        StartCoroutine(TimedBoonOffer());
    }

    IEnumerator TimedBoonOffer() {
        yield return new WaitForSeconds(1.5f);
        CurrentSelectionState = SelectionState.BOON;
    }

    void HandleAbilityPhasePromptForCharacter(Character combatant) {
        // TurnOrderUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            List<AbilityCategory> availableAbilities = combatant.GetAvailableAbilities(LightRef, ShadowRef);
            if (availableAbilities.Contains(AbilityCategory.SPECIALATTACK)) {
                AbilityUI.SetSpecialAbilityName(
                    AttackTypeToAbility.Lookup(combatant.Config.SpecialAttack, combatant.Config).Name
                );
            }
            if (availableAbilities.Contains(AbilityCategory.ULTIMATE)) {
                AbilityUI.SetUltimateAbilityName(
                    AttackTypeToAbility.Lookup(combatant.Config.UltimateAbility, combatant.Config).Name
                );
            }
            AbilityUI.SetAvailableAbilities(availableAbilities);
            AbilityUI.ToggleToSelectedAbility((int) AbilityCategory.BASICATTACK);
            AbilityUI.gameObject.SetActive(true);
            AbilityUI.gameObject.transform.position = combatant.transform.position + new Vector3(-2.5f, 3.35f, .25f);
            CurrentSelectionState = SelectionState.ABILITY;
        }
    }
    void HandleTargetPhasePromptForCharacter(Character combatant) {
        AbilityUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            CurrentSelectionState = SelectionState.TARGET;
            AbilityUI.gameObject.SetActive(false);
            
            TurnOrderUI.SetSelectionMode(true);
        }
    }
    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        TurnOrderUI.SetSelectionMode(false);
        AbilityUI.gameObject.SetActive(false);
        CurrentSelectionState = SelectionState.NONE;
    }

    void HandleDamageResolved(CalculatedDamage dmg) {
        if (dmg.Target.isDead) {
            string targetDied = dmg.Target.Config.Name + " has been defeated!";
            TextCrawlUI.EnqueueMessage(targetDied);
        }
    }

    void HandleBuffAdded(Buff buff) {
        // if (buff.Target.isDead) {
        //     return;
        // }
        // string buffApplied = buff.Target.Config.Name + " has been afflicted by " + buff.Name + "!";
        // TextCrawlUI.EnqueueMessage(buffApplied);
    }

    void HandleEffectPlanExecutionStart(EffectPlan executedAbility) {
        string abilityCast = executedAbility.Caster.Config.Name + " used " + executedAbility.Source.Name;
        if (executedAbility.Target != null) {
            abilityCast += " on " + executedAbility.Target.Config.Name;
        }
        TextCrawlUI.EnqueueMessage(abilityCast);
    }

    void HandleEligibleTargetsChanged(List<Character> targets) {
        TurnOrderUI.SetEligibleTargets(targets);
    }
}
