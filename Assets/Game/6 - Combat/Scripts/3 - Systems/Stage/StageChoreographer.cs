using System.Collections;
using UnityEngine;

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
        StartCoroutine(WaitPerformance(2.5f));
    }

    void HandleAbilityExecuted(ExecutedAbility executedAbility) {
        foreach(CalculatedDamage dmg in executedAbility.AppliedHealthChanges) {
            if (dmg.Target.isDead) {
                dmg.Target.GetComponent<ActorCharacter>().DoDeathPerformance();
            } else if (dmg.StaggerCrackedByThis) {
                dmg.Target.GetComponent<ActorCharacter>().DoCrackedPerformance();
            }
        }
    }
   
}
