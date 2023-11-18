using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class StageChoreographer : MonoBehaviour
{
    EventProvider _eventProvider;
    public bool IsPerforming = false;

    void Awake() {
        _eventProvider = GetComponent<CombatReferee>().eventProvider;
        SetupHooks();
    }

    void SetupHooks() {
        _eventProvider.OnPhasePrompt += HandlePhasePrompts;
        _eventProvider.OnWaveReady += HandleWaveReady;
        _eventProvider.OnAbilityExecuted += HandleAbilityExecuted;
        _eventProvider.OnCharacterRevived += HandleCharacterRevived;
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

    void HandleAbilityExecuted(ExecutedAbility executedAbility) {
        foreach(CalculatedDamage dmg in executedAbility.AppliedHealthChanges) {
            ActorCharacter sourceMotor = executedAbility.Source.GetComponent<ActorCharacter>();
            if (executedAbility.Ability is AbilityBasicAttack) {
                sourceMotor.DoBasicAttackPerformance();
            } else {
                sourceMotor.DoSpecialAbilityPerformance();
            }

            ActorCharacter victimMotor = dmg.Target.GetComponent<ActorCharacter>();
            victimMotor.DoDamageTakenPerformance();
            if (dmg.Target.isDead) {
                victimMotor.DoDeathPerformance();
            } else if (dmg.StaggerCrackedByThis) {
                victimMotor.DoCrackedPerformance();
            }
        }
        // TODO: Actors should have performances too.
        // We'll want to wait until they're all finished.
        WaitPerformance(1f);
    }

    void HandleCharacterRevived(Character combatant) {
        combatant.GetComponent<ActorCharacter>().DoRevivedPerformance();
    }
   
}
