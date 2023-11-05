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
    public int StageNumber = 1;
    public int WaveNumber = 4;


    void Start()
    {
        SetupWave();   
    }

    Transform GetSpawnPointByIndex(int index) {
        
        return transform.Find("Enemy Field").transform.Find("SpawnPoint" + index.ToString());
    }

    void SetupWave() {
        // make sure the queue is empty
        combatantQueue.Clear();

        // make sure there aren't CPUs in the field
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<CpuCharacter>() != null) {
                Destroy(child.gameObject);
            }
        }

        // clear out enemies from last wave
        combatants.RemoveAll(combatant => combatant is CpuCharacter);

        // spawn enemies for this wave
        List<EnemyType> enemiesToMake = GetEnemiesForWave();
        for (var i=0; i < enemiesToMake.Count; i++) {
            GameObject newEnemy = Instantiate(Resources.Load("Characters/CPU/" + enemiesToMake[i].ToString())) as GameObject;
            newEnemy.transform.parent = GameObject.Find("Combat Field").transform;
            newEnemy.transform.position = GetSpawnPointByIndex(i).position;
            Debug.Log(GetSpawnPointByIndex(i).position);
            combatants.Add(newEnemy.GetComponent<CpuCharacter>());
        }
        
        // set up combatant queue
        SeedCombatQueue();

        // set up current combatant
        PROGRESSBOARD();
    }


    void SeedCombatQueue()
    {
        // add all combatants to the queue in a random order
        List<Combatant> shuffledCombatants = combatants.OrderBy(combatant => Random.Range(0, 100)).ToList();
        foreach(Combatant combatant in shuffledCombatants)
        {
            combatantQueue.Enqueue(combatant);
        }
    }

    void StageChangeover() {
        StageNumber++;
        WaveNumber = 4;
        StageResetPlayerCharacters();
        SetupWave();
    }

    void StageResetPlayerCharacters() {
        List<Combatant> PCs = GetAllPCs();
        foreach (Combatant pc in PCs) {
            pc.currentHealth = pc.maximumHealth;
            pc.isDead = false;
        }
    }

    void PROGRESSBOARD() {
        // Check Win Conditions
        if (GetAlivePCs().Count == 0) {
            Debug.Log("The CPUs have won!");
            LoseUI.SetActive(true);
            CleanupOnEndTrigger();
            return;
        } else if (GetAliveCPUs().Count == 0) {
            if (WaveNumber == 5) {
                if (StageNumber == 4) {
                    Debug.Log("The PCs have won the game!");
                    WinUI.SetActive(true);
                    CleanupOnEndTrigger();
                    return;
                } else {
                    StageChangeover();
                }
            } else {
                WaveNumber++;
                SetupWave();
                return;
            }
        }

        MoveToNextCombatant();
    }

    void CleanupOnEndTrigger() {
        currentCombatant = null;
        AdjustCurrentCombatantUI();
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
        if (currentCombatant != null && currentCombatant is PlayerCharacter) {
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

    List<Combatant> GetAllPCs() {
        return combatantQueue.ToList().FindAll(combatant => combatant is PlayerCharacter);
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

    List<EnemyType> GetEnemiesForWave() {
        List<EnemyType> enemies = new List<EnemyType>();
        switch (WaveNumber) {
            case 1:
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobDark);
                break;
            case 2:
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobDark);
                break;
            case 3:
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobDark);
                enemies.Add(EnemyType.Stage1MobDark);
                break;
            case 4:
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobDark);
                enemies.Add(EnemyType.Stage1MobDark);
                break;
            case 5:
                enemies.Add(EnemyType.Stage1MobLight);
                enemies.Add(EnemyType.Stage1MobDark);
                enemies.Add(EnemyType.Stage1Boss);
                break;
        }
        return enemies;
    }
}
