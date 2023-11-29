using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool IsPerforming = false;

    [Header("Temp Plumbing: UI Panels")]
    public UI_BoonMenuManager BoonUI;
    EventProvider _eventProvider;
    // public UI_AbilitySelection AbilitySelectionUI;
    // public UI_TargetSelection TargetSelectionUI;
    public UI_BattleMenu AbilityUI;
    public UI_TextCrawler TextCrawlUI;
    public UI_ScalePanelManager ScalePanelUI;
    public UI_TurnOrderManager TurnOrderUI;
    public UI_StageCurtain StageCurtainUI;
    public UI_StageNameIntro StageNameUI;
    public SpriteRenderer BackgroundImage;
    bool IsSelectingAbility = false;
    bool IsSelectingTarget = false;
    bool IsSelectingBoon = false;
    bool BoonTimeout = false;

    public void TargetSelected(Character target) {
        _eventProvider.OnInput_CombatantChoseTarget?.Invoke(target);
    }
    public void AbilitySelected(AbilityCategory category) {
        _eventProvider.OnInput_CombatantChoseAbility?.Invoke(category);
    }

    public void BoonSelected(BaseBoonResolver boon) {
        if (BoonTimeout || !IsSelectingBoon) {
            return;
        }
        Debug.Log("SELECTED A BOON");
        IsSelectingBoon = false;
        BoonTimeout = true;
        StartCoroutine(BoonTimeoutRoutine());
        BoonUI.Dismiss();
        _eventProvider.OnInput_BoonSelected?.Invoke(boon);
    }

    public void RequestBackupToAbilitySelection() {
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
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.UpArrow)
            ||
            Input.GetKeyDown(KeyCode.W)
        )) {
            AbilityUI.ToggleUp();
        }

        if (
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.DownArrow)
            ||
            Input.GetKeyDown(KeyCode.S)
        )) {
            AbilityUI.ToggleDown();
        }

        if (
            IsSelectingTarget && (
                Input.GetKeyDown(KeyCode.LeftArrow)
                ||
                Input.GetKeyDown(KeyCode.A)
        )) {
            TurnOrderUI.ToggleLeft();
        }

        if (
            IsSelectingTarget && (
                Input.GetKeyDown(KeyCode.RightArrow)
                ||
                Input.GetKeyDown(KeyCode.D)
        )) {
            TurnOrderUI.ToggleRight();
        }

        if (
            IsSelectingBoon && (
                Input.GetKeyDown(KeyCode.LeftArrow)
                ||
                Input.GetKeyDown(KeyCode.A)
        )) {
            BoonUI.ToggleLeft();
        }

        if (
            IsSelectingBoon && (
                Input.GetKeyDown(KeyCode.RightArrow)
                ||
                Input.GetKeyDown(KeyCode.D)
        )) {
            BoonUI.ToggleRight();
        }

        if (IsSelectingTarget && (
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
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            AbilitySelected(AbilityUI.CurrentlySelected());
        }

        if (
            IsSelectingTarget && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            TargetSelected(TurnOrderUI.CurrentSelection);
        }

        if (
            IsSelectingBoon && (
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
        StageCurtainUI.gameObject.SetActive(true);
        StageCurtainUI.CurtainUp();
        yield return new WaitForSeconds(0.5f);
        StageNameUI.IntroduceStage();
        yield return new WaitForSeconds(4.5f);
        IsPerforming = false;
    }

    void HandleTurnOrderChanged(Character character, List<Character> InQueue) {
        TurnOrderUI.UpdateTurnOrder(character, InQueue);
    }

    void HandleWaveReady() {
        IsPerforming = true;
        AbilityUI.gameObject.SetActive(false);
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
        IsSelectingAbility = false;
        IsSelectingBoon = true;
        IsSelectingTarget = false;
        TurnOrderUI.SetSelectionMode(false);
        AbilityUI.gameObject.SetActive(false);
        BoonUI.OfferBoons(boons);
    }

    void HandleAbilityPhasePromptForCharacter(Character combatant) {
        // TurnOrderUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
        Debug.Log("ABILITY PHASE PROMPT");
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
            IsSelectingAbility = true;
            IsSelectingBoon = false;
            IsSelectingTarget = false;
        }
    }
    void HandleTargetPhasePromptForCharacter(Character combatant) {
        AbilityUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            IsSelectingAbility = false;
            IsSelectingBoon = false;
            IsSelectingTarget = true;
            AbilityUI.gameObject.SetActive(false);
            
            TurnOrderUI.SetSelectionMode(true);
        }
    }
    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        TurnOrderUI.SetSelectionMode(false);
        AbilityUI.gameObject.SetActive(false);
        IsSelectingAbility = false;
        IsSelectingBoon = false;
        IsSelectingTarget = false;
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
