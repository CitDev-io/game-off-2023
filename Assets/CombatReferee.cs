using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatReferee : MonoBehaviour
{
    [SerializeField]
    public List<Combatant> combatants = new List<Combatant>();
    [SerializeField]
    public Queue<Combatant> combatantQueue = new Queue<Combatant>();
    public Combatant currentCombatant;
    public GameObject combatantUI;
    public GameObject WinUI;
    public GameObject LoseUI;


    void Start()
    {
        // introduce combatants

        // set up combatant queue
        SeedCombatQueue();

        PROGRESSBOARD();     
    }

    void SeedCombatQueue()
    {
        foreach(Combatant combatant in combatants)
        {
            combatantQueue.Enqueue(combatant);
        }
    }

    void PROGRESSBOARD() {
        // Check Win Conditions
        if (GetAlivePCs().Count == 0) {
            Debug.Log("The CPUs have won!");
            LoseUI.SetActive(true);
            return;
        } else if (GetAliveCPUs().Count == 0) {
            Debug.Log("The PCs have won!");
            WinUI.SetActive(true);
            return;
        }

        MoveToNextCombatant();
    }

    void MoveToNextCombatant() {
        if (currentCombatant != null) currentCombatant.TurnEnd();



        currentCombatant = combatantQueue.Dequeue();
        while (currentCombatant.isDead && combatantQueue.Count > 0) {
            currentCombatant = combatantQueue.Dequeue();
        }
        AdjustCurrentCombatantUI();
        Debug.Log("It is now " + currentCombatant.gameObject.name + "'s turn.");
        currentCombatant.TurnStart();
        combatantQueue.Enqueue(currentCombatant);
        if (currentCombatant is CpuCharacter) {
            Debug.Log(currentCombatant.gameObject.name + " is a CPU. It is now their turn.");
            DoCpuTurn();
        }
    }

    void AdjustCurrentCombatantUI() {
        if (currentCombatant is PlayerCharacter) {
            combatantUI.SetActive(true);
        } else {
            combatantUI.SetActive(false);
        }
    }

    public void ReportCombatantClicked(Combatant target) {
        if (target is CpuCharacter && currentCombatant is PlayerCharacter) {
            ExecuteAttack(false, currentCombatant, target);
        }
    }

    void ExecuteAttack(bool special, Combatant attacker, Combatant target) {
        if (special) {
            Debug.Log(attacker.gameObject.name + " used a special attack on " + target.gameObject.name + "!");
        } else {
            Debug.Log(attacker.gameObject.name + " attacked " + target.gameObject.name + "!");
            int DamageDealt = Random.Range(attacker.MinDamage, attacker.MaxDamage);
            target.TakeDamage(DamageDealt);
        }
        PROGRESSBOARD();
    }

    void DoCpuTurn() {
        StartCoroutine("CpuTurn");
    }

    List<Combatant> GetAlivePCs() {
        return combatantQueue.ToList().FindAll(combatant => combatant is PlayerCharacter && !combatant.isDead);
    }

    List<Combatant> GetAliveCPUs() {
        return combatantQueue.ToList().FindAll(combatant => combatant is CpuCharacter && !combatant.isDead);
    }

    Combatant getRandomPlayerCharacter() {
        List<Combatant> playerCharacters = GetAlivePCs();

        Combatant randomPc = playerCharacters[Random.Range(0, playerCharacters.Count)];

        return randomPc;
    }

    IEnumerator CpuTurn() {
        yield return new WaitForSeconds(1f);
        ExecuteAttack(false, currentCombatant, getRandomPlayerCharacter());
    }
}
