using System.Collections;
using UnityEngine;

public class StageChoreographer : MonoBehaviour
{
    EventProvider _eventProvider;
    public bool IsPerforming = false;

    /*
        WILO

        Would love to keep a running tally of all of my
        actors.

        when we receive a waveclear signal, we should wait
        for all of the actors to complete their performances
        then we send an ALL CLEAR to the event provider

        
        referee should wait for ALL CLEAR before proceeding

    */

    void Awake() {
        _eventProvider = GetComponent<CombatReferee>().eventProvider;
        SetupHooks();
    }

    void SetupHooks() {
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnEffectPlanExecutionComplete += HandleAbilityExecuted;
        _eventProvider.OnCharacterRevived += HandleCharacterRevived;
        _eventProvider.OnDamageResolved += HandleDamageResolved;
    }

    void HandleWaveReady() {
        StartCoroutine(WaitPerformance(1.5f));
    }

    IEnumerator WaitPerformance(float duration) {
        IsPerforming = true;
        yield return new WaitForSeconds(duration);
        IsPerforming = false;
    }

    void HandlePhasePrompts(CombatPhase phase, Character combatant) {
        if (phase == CombatPhase.CHARACTERTURN_EXECUTION) {
            HandleExecutionPhasePromptForCharacter(combatant);
        }
    }

    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        StartCoroutine(WaitPerformance(1f));
    }

    void HandleDamageResolved(CalculatedDamage cd) {
        ActorCharacter sourceMotor = cd.Attacker.GetComponent<ActorCharacter>();
        if (cd.Source is AbilityBasicAttack) {
            sourceMotor.EnqueuePerformance(CharacterActorPerformance.BASICATTACK);
        } else {
            sourceMotor.EnqueuePerformance(CharacterActorPerformance.SPECIALATTACK);
        }

        if (cd.DamageToHealth > 0) {
            ActorCharacter victimMotor = cd.Target.GetComponent<ActorCharacter>();
            
            victimMotor.EnqueuePerformance(CharacterActorPerformance.TAKEDAMAGE);
            if (cd.Target.isDead) {
                victimMotor.EnqueuePerformance(CharacterActorPerformance.DIE);
            } else if (cd.StaggerCrackedByThis) {
                victimMotor.EnqueuePerformance(CharacterActorPerformance.CRACKED);
            }
        }
    }

    void HandleAbilityExecuted(EffectPlan executedAbility) {
        // after all things have resolved
    }

    void HandleCharacterRevived(Character character) {
        character.GetComponent<ActorCharacter>().EnqueuePerformance(CharacterActorPerformance.REVIVE);
    }
}
