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
    public UI_ScalePanelManager ScalePanelUI;
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
            TargetSelected(TargetSelectionUI.CurrentSelection);
        }
    }

    void SetupHooks() {
        _eventProvider.OnBoonOffer += HandleBoonOffer;
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnEligibleTargetsChanged += HandleEligibleTargetsChanged;
        _eventProvider.OnAbilityExecuted += HandleAbilityExecuted;
        _eventProvider.OnScaleChanged += HandleScaleChanged;
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
        TargetSelectionUI.gameObject.SetActive(false);
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

    void HandleBoonOffer(List<BaseBoonResolver> boons) {
        TargetSelectionUI.gameObject.SetActive(false);
        AbilitySelectionUI.gameObject.SetActive(false);
        BoonUI.GetComponent<UI_BoonMenuManager>().OfferBoons(boons);
    }
}
