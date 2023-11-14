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
    bool IsSelectingAbility = false;


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
            IsSelectingAbility && (
            Input.GetKeyDown(KeyCode.Space)
            ||
            Input.GetKeyDown(KeyCode.KeypadEnter)
            ||
            Input.GetKeyDown(KeyCode.Return)
        )) {
            AbilitySelected(AbilitySelectionUI.CurrentlySelectedisBasic());
        }
    }

    void SetupHooks() {
        // _eventProvider.OnCharacterTurnStart += HandleCharacterTurnStart;
        _eventProvider.OnBoonOffer += HandleBoonOffer;
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
    }

    void HandleWaveReady() {
        IsPerforming = true;
        AbilitySelectionUI.gameObject.SetActive(false);
        Debug.Log("******WAVE START");
        IsPerforming = false;
    }

    void HandlePhasePrompts(CombatPhase phase, Character combatant) {
        if (phase == CombatPhase.CHARACTERTURN_CHOOSEABILITY) {
           HandleAbilityStartForCharacter(combatant);
        }
        if (phase == CombatPhase.CHARACTERTURN_CHOOSETARGET) {
            HandleAbilityEndForCharacter();
        }
    }

    void HandleAbilityEndForCharacter() {
        IsSelectingAbility = false;
        AbilitySelectionUI.gameObject.SetActive(false);
    }

    void HandleAbilityStartForCharacter(Character combatant) {
        Debug.Log("**UI SEES " + combatant.Config.Name + " IS STARTING THEIR ABILITY CHOICE**");
        if (combatant.Config.TeamType == TeamType.PLAYER) {
            AbilitySelectionUI.ToggleAvailableAbilities(true, combatant.Config.SpecialAttack != AttackType.NONE);
            AbilitySelectionUI.ToggleSelectedAbility(true);
            AbilitySelectionUI.gameObject.SetActive(true);
            IsSelectingAbility = true;
        }
    }

    void HandleBoonOffer() {
        BoonUI.SetActive(true);
    }
}
