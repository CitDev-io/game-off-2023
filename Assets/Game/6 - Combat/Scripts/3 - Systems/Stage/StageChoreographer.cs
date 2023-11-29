using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageChoreographer : MonoBehaviour
{
    EventProvider _eventProvider;
    List<string> PerformancesInFlight = new List<string>();
    /*
        WILO

        Would love to keep a running tally of all of my
        actors.

        when we receive a waveclear signal, we should wait
        for all of the actors to complete their performances
        then we send an ALL CLEAR to the event provider

        
        referee should wait for ALL CLEAR before proceeding

    */

    public bool IsPerforming() {
        int MyPerformanceCount = PerformancesInFlight.Count;
            int MyActorsPerformingCount = MyActors.Count(actor => actor.IsPerforming);
        return MyPerformanceCount > 0 || MyActorsPerformingCount > 0;
    }
    List<ActorCharacter> MyActors = new List<ActorCharacter>();

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
        _eventProvider.OnEffectPlanExecutionStart += HandleEffectPlanExecutionStart;
        _eventProvider.OnBuffAdded += HandleBuffAdded;
        _eventProvider.OnBuffExpired += HandleBuffRemoved;
        _eventProvider.OnStageComplete += HandleStageComplete;
        _eventProvider.OnCharacterSummoned += HandleCharacterSummoned;
    }

    void HandleCharacterSummoned(Character character) {
        MyActors.Add(character.GetComponent<ActorCharacter>());
    }

    void HandleStageComplete() {
        MyActors
            .Where(actor => actor._character.isDead && actor._character.Config.TeamType == TeamType.PLAYER)
            .ToList()
            .ForEach(actor => {
            if (actor.GetComponent<Character>().isDead) {
                actor.EnqueuePerformance(CharacterActorPerformance.FADEOUT);
            }
        });
    }

    void PolymorphCharacter(Character character, bool isPolymorphed) {
        ActorCharacter actor = character.GetComponent<ActorCharacter>();
        if (isPolymorphed) {
            actor.EnqueuePerformance(CharacterActorPerformance.POLYMORPH);
        } else {
            actor.EnqueuePerformance(CharacterActorPerformance.UNPOLYMORPH);
        }
    }

    void HandleBuffAdded(Buff buff) {
        if (buff is BuffPolymorph) {
            PolymorphCharacter(buff.Target, true);
        }
        buff.Target.GetComponent<ActorCharacter>().FloatingBuffDown(buff);
    }

    void HandleBuffRemoved(Buff buff) {
        if (buff is BuffPolymorph) {
            PolymorphCharacter(buff.Target, false);
        }
        buff.Target.GetComponent<ActorCharacter>().FloatingBuffUp(buff);
    }

    void HandleWaveReady() {
        StartCoroutine(WaitPerformance(1.5f, "wavereadylull"));
    }

    IEnumerator WaitPerformance(float duration, string name) {
        PerformancesInFlight.Add(name);
        Debug.Log("IN " + name);
        yield return new WaitForSeconds(duration);
        PerformancesInFlight.Remove(name);
        Debug.Log("OUT " + name);
    }

    void HandlePhasePrompts(CombatPhase phase, Character combatant) {
        if (phase == CombatPhase.CHARACTERTURN_EXECUTION) {
            HandleExecutionPhasePromptForCharacter(combatant);
        }
    }

    void HandleExecutionPhasePromptForCharacter(Character combatant) {
        // StartCoroutine(WaitPerformance(10f, "executionlull"));
    }

    void HandleEffectPlanExecutionStart(EffectPlan plan) {
        if (plan.Source is AbilityBasicAttack) {
            plan.Caster.GetComponent<ActorCharacter>().EnqueuePerformance(CharacterActorPerformance.BASICATTACK);
        } else {
            plan.Caster.GetComponent<ActorCharacter>().EnqueuePerformance(CharacterActorPerformance.SPECIALATTACK);
        }

        if (plan.Source is AbilityHollowHowl) {
            Debug.Log("RAAAAAWR");
            WaitPerformance(4f, "howling");
        }
    }

    void HandleDamageResolved(CalculatedDamage cd) {
        // ActorCharacter sourceMotor = cd.Attacker.GetComponent<ActorCharacter>();
        // if (cd.Source is AbilityBasicAttack) {
        //     sourceMotor.EnqueuePerformance(CharacterActorPerformance.BASICATTACK);
        // } else {
        //     sourceMotor.EnqueuePerformance(CharacterActorPerformance.SPECIALATTACK);
        // }
        ActorCharacter victimMotor = cd.Target.GetComponent<ActorCharacter>();

        victimMotor.FloatingDamageText(cd.DamageToHealth);

        if (cd.DamageToHealth > 0) {
            
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
