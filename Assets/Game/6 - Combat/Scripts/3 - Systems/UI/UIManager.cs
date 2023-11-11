using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool IsPerforming = false;

    [Header("Temp Plumbing: UI Panels")]
    public GameObject combatantUI;
    public GameObject BoonUI;
    EventProvider _eventProvider;

    void Start()
    {
        _eventProvider = GetComponent<CombatReferee>().eventProvider;
        SetupHooks();
    }

    void SetupHooks() {
        _eventProvider.OnCharacterTurnStart += HandleCharacterTurnStart;
        _eventProvider.OnBoonOffer += HandleBoonOffer;
    }

    void HandleBoonOffer() {
        BoonUI.SetActive(true);
    }



    void HandleCharacterTurnStart(Character ActiveCharacter) {
        if (ActiveCharacter.Config.TeamType == TeamType.PLAYER) {
            combatantUI.SetActive(true);
        } else {
            combatantUI.SetActive(false);
        }
    }
    
    void AdjustCurrentCombatantUI() {
        
    }
}
