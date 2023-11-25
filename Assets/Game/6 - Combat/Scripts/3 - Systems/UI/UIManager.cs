using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool IsPerforming = false;

    [Header("Temp Plumbing: UI Panels")]
    public GameObject BoonUI;
    EventProvider _eventProvider;
    public UI_AbilitySelection AbilitySelectionUI;
    // public UI_TargetSelection TargetSelectionUI;
    public UI_TextCrawler TextCrawlUI;
    public UI_ScalePanelManager ScalePanelUI;
    public UI_TurnOrderManager TurnOrderUI;
    bool IsSelectingAbility = false;
    bool IsSelectingTarget = false;
    bool BoonTimeout = false;

    public void TargetSelected(Character target) {
        _eventProvider.OnInput_CombatantChoseTarget?.Invoke(target);
    }
    public void AbilitySelected(AbilityCategory category) {
        _eventProvider.OnInput_CombatantChoseAbility?.Invoke(category);
    }

    public void BoonSelected(BaseBoonResolver boon) {
        if (BoonTimeout) {
            return;
        }
        StartCoroutine(BoonTimeoutRoutine());
        _eventProvider.OnInput_BoonSelected?.Invoke(boon);
    }

    public void RequestBackupToAbilitySelection() {
        _eventProvider.OnInput_BackOutOfTargetSelection?.Invoke();
    }

    IEnumerator BoonTimeoutRoutine() {
        BoonTimeout = true;
        yield return new WaitForSeconds(5f);
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
            AbilitySelectionUI.ToggleUp();
        }

        if (
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.DownArrow)
            ||
            Input.GetKeyDown(KeyCode.S)
        )) {
            AbilitySelectionUI.ToggleDown();
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
            AbilitySelected(AbilitySelectionUI.CurrentlySelected());
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
    }

    void SetupHooks() {
        _eventProvider.OnBoonOffer += HandleBoonOffer;
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnEligibleTargetsChanged += HandleEligibleTargetsChanged;
        _eventProvider.OnScaleChanged += HandleScaleChanged;
        _eventProvider.OnDamageResolved += HandleDamageResolved;
        _eventProvider.OnBuffAdded += HandleBuffAdded;
        _eventProvider.OnEffectPlanExecutionStart += HandleEffectPlanExecutionStart;
        _eventProvider.OnTurnOrderChanged += HandleTurnOrderChanged;
    }

    void HandleTurnOrderChanged(Character character, List<Character> InQueue) {
        TurnOrderUI.UpdateTurnOrder(character, InQueue);
    }

    void HandleWaveReady() {
        IsPerforming = true;
        AbilitySelectionUI.gameObject.SetActive(false);
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

    void HandleAbilityPhasePromptForCharacter(Character combatant) {
        // TurnOrderUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            List<AbilityCategory> availableAbilities = combatant.GetAvailableAbilities(LightRef, ShadowRef);
            if (availableAbilities.Contains(AbilityCategory.SPECIALATTACK)) {
                AbilitySelectionUI.SetSpecialAbilityName(combatant.Config.SpecialAttack.ToString());
            }
            if (availableAbilities.Contains(AbilityCategory.ULTIMATE)) {
                AbilitySelectionUI.SetUltimateAbilityName(combatant.Config.UltimateAbility.ToString());
            }
            AbilitySelectionUI.SetAvailableAbilities(availableAbilities);
            AbilitySelectionUI.ToggleToSelectedAbility((int) AbilityCategory.BASICATTACK);
            AbilitySelectionUI.gameObject.SetActive(true);
            IsSelectingAbility = true;
        }
    }
    void HandleTargetPhasePromptForCharacter(Character combatant) {
        AbilitySelectionUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            IsSelectingAbility = false;
            IsSelectingTarget = true;
            AbilitySelectionUI.gameObject.SetActive(false);
            
            TurnOrderUI.SetSelectionMode(true);
        }
    }
    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        TurnOrderUI.SetSelectionMode(false);
        AbilitySelectionUI.gameObject.SetActive(false);
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

    void HandleBoonOffer(List<BaseBoonResolver> boons) {
        TurnOrderUI.SetSelectionMode(false);
        AbilitySelectionUI.gameObject.SetActive(false);
        BoonUI.GetComponent<UI_BoonMenuManager>().OfferBoons(boons);
    }
}
