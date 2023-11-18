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
    public UI_TargetSelection TargetSelectionUI;
    public UI_TextCrawler TextCrawlUI;
    bool IsSelectingAbility = false;
    bool IsSelectingTarget = false;

    public void TargetSelected(Character target) {
        _eventProvider.OnInput_CombatantChoseTarget?.Invoke(target);
    }
    public void AbilitySelected(bool isBasic) {
        _eventProvider.OnInput_CombatantChoseAbility?.Invoke(isBasic);
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
            ||
            Input.GetKeyDown(KeyCode.S)
            ||
            Input.GetKeyDown(KeyCode.DownArrow)
        )) {
            AbilitySelectionUI.Toggle();
        }

        if (
            IsSelectingTarget && (
                Input.GetKeyDown(KeyCode.UpArrow)
                ||
                Input.GetKeyDown(KeyCode.W)
        )) {
            TargetSelectionUI.ToggleUp();
        }

        if (
            IsSelectingTarget && (
                Input.GetKeyDown(KeyCode.DownArrow)
                ||
                Input.GetKeyDown(KeyCode.S)
        )) {
            TargetSelectionUI.ToggleDown();
        }
        
        if (
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            AbilitySelected(AbilitySelectionUI.CurrentlySelectedisBasic());
        }

        if (
            IsSelectingTarget && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            TargetSelected(TargetSelectionUI.CurrentSelection);
        }
    }

    void SetupHooks() {
        _eventProvider.OnBoonOffer += HandleBoonOffer;
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnEligibleTargetsChanged += HandleEligibleTargetsChanged;
        _eventProvider.OnAbilityExecuted += HandleAbilityExecuted;
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

    void HandleInitPhasePrompt() {

    }

    void HandlePreflightPhasePromptForCharacter(Character combatant) {
        TextCrawlUI.EnqueueMessage(combatant.Config.Name + "'s turn to act!");
    }

    void HandleAbilityPhasePromptForCharacter(Character combatant) {
        TargetSelectionUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            AbilitySelectionUI.SetSpecialAbilityName(combatant.Config.SpecialAttack.ToString());
            AbilitySelectionUI.ToggleAvailableAbilities(true, combatant.Config.SpecialAttack != UserAbilitySelection.NONE);
            AbilitySelectionUI.ToggleSelectedAbility(true);
            AbilitySelectionUI.gameObject.SetActive(true);
            IsSelectingAbility = true;
        }
    }
    void HandleTargetPhasePromptForCharacter(Character combatant) {
        AbilitySelectionUI.gameObject.SetActive(false);
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            IsSelectingAbility = false;
            IsSelectingTarget = true;
            TargetSelectionUI.ToggleEligibleTarget(0);
            AbilitySelectionUI.gameObject.SetActive(false);
            TargetSelectionUI.gameObject.SetActive(true);
        }
    }
    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        TargetSelectionUI.gameObject.SetActive(false);
        AbilitySelectionUI.gameObject.SetActive(false);
        IsSelectingTarget = false;
    }

    void HandleAbilityExecuted(ExecutedAbility executedAbility) {
        string abilityCast = executedAbility.Source.Config.Name + " used " + executedAbility.Ability.Name;
        if (executedAbility.Target != null) {
            abilityCast += " on " + executedAbility.Target.Config.Name;
        }
        TextCrawlUI.EnqueueMessage(abilityCast);

        foreach(CalculatedDamage dmg in executedAbility.AppliedHealthChanges) {
            string damageDealt =  dmg.Target.Config.Name;
            if (dmg.DamageToHealth < 0) {
                damageDealt += " was healed for ";
            } else {
                damageDealt += " took ";
            }
            damageDealt += Mathf.Abs(dmg.DamageToHealth);
            if (dmg.DamageToHealth < 0) {
                damageDealt += " health";
            } else {
                damageDealt += " damage (" + dmg.DamageToStagger + " stagger)";
            }
            
            TextCrawlUI.EnqueueMessage(damageDealt);

            if (dmg.Target.isDead) {
                string targetDied = dmg.Target.Config.Name + " has been defeated!";
                TextCrawlUI.EnqueueMessage(targetDied);
            } else if (dmg.StaggerCrackedByThis) {
                string staggerCracked = dmg.Target.Config.Name + "'s stagger has been cracked!";
                TextCrawlUI.EnqueueMessage(staggerCracked);
            }
        }

        foreach(Buff buff in executedAbility.AppliedBuffs) {
            if (buff.Target.isDead) {
                continue;
            }
            string buffApplied = buff.Target.Config.Name + " has been afflicted by " + buff.Name + "!";
            TextCrawlUI.EnqueueMessage(buffApplied);
        }

    }

    void HandleEligibleTargetsChanged(List<Character> targets) {
        TargetSelectionUI.SetEligibleTargets(targets);
    }

    void HandleBoonOffer() {
        TargetSelectionUI.gameObject.SetActive(false);
        AbilitySelectionUI.gameObject.SetActive(false);
        BoonUI.GetComponent<UI_BoonMenuManager>().DoAppearPerformance();
    }
}
