using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatReferee : MonoBehaviour
{  
    // TODO: Give to UIManager
    public GameObject combatantUI;
    // TODO: Give to UIManager
    public GameObject WinUI;
    // TODO: Give to UIManager
    public GameObject LoseUI;
    //TODO: give to UIManager
    public GameObject ReviveUI;
    //TODO: give to UIManager
    public GameObject BoonUI;

    // Long-Term Referee Stuff
    public GameState gameState;
    public CombatState combatState;
    public EventProvider eventProvider;

    void Awake() {
        // TODO: generate these via stagechoreo later, for now, using already in-scene characters
        List<Combatant> PCTeam = new List<Combatant>();
        PCTeam.Add(GameObject.Find("Warlock").GetComponent<Combatant>());
        PCTeam.Add(GameObject.Find("Priest").GetComponent<Combatant>());
        PCTeam.Add(GameObject.Find("Rogue").GetComponent<Combatant>());
        PCTeam.Add(GameObject.Find("Paladin").GetComponent<Combatant>());
        eventProvider = new EventProvider();
        gameState = new GameState(PCTeam);
        combatState = new CombatState();
    }

    void Start()
    {
        SetupWave();   
    }

    //TODO: Go to stagechoreo
    Transform GetSpawnPointByIndex(int index) {
        return transform.Find("Enemy Field").transform.Find("SpawnPoint" + index.ToString());
    }

    //TODO: Orchestrate between WaveProvider and StageChoreo
    void SetupWave() {
        // make sure the queue is empty
        combatState.TurnOrder.Clear();

        // make sure there aren't CPUs in the field
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<CpuCharacter>() != null) {
                Destroy(child.gameObject);
            }
        }

        // clear out enemies from last wave
        gameState.combatants.RemoveAll(combatant => combatant is CpuCharacter);

        // spawn enemies for this wave
        List<EnemyType> enemiesToMake = GetEnemiesForWave();
        for (var i=0; i < enemiesToMake.Count; i++) {
            GameObject newEnemy = Instantiate(Resources.Load("Characters/CPU/" + enemiesToMake[i].ToString())) as GameObject;
            newEnemy.transform.parent = GameObject.Find("Combat Field").transform;
            newEnemy.transform.position = GetSpawnPointByIndex(i).position;
            Debug.Log(GetSpawnPointByIndex(i).position);
            gameState.combatants.Add(newEnemy.GetComponent<CpuCharacter>());
        }
        
        // set up combatant queue
        SeedCombatQueue();

        // set up current combatant
        PROGRESSBOARD();
    }

    // REFEREE JOB, maybe a shuffle resolver?
    void SeedCombatQueue()
    {
        // add all combatants to the queue in a random order
        List<Combatant> shuffledCombatants = gameState.combatants.OrderBy(combatant => Random.Range(0, 100)).ToList();
        foreach(Combatant combatant in shuffledCombatants)
        {
            combatState.TurnOrder.Enqueue(combatant);
        }
    }

    void ReviveAllPCs() {
        List<Combatant> PCs = gameState.GetAllPCs();
        foreach (Combatant pc in PCs) {
            pc.currentHealth = pc.maximumHealth;
            pc.isDead = false;
        }
    }

    public void UserResponse_Revive(bool doIt) {
        ReviveUI.SetActive(false);
        if (doIt && gameState.ScalesOwned >= 5) {
            gameState.ScalesOwned -= 5;
            ReviveAllPCs();
            WaveChangeStep2();
        }
    }

    public void UserResponse_Boon() {
        BoonUI.SetActive(false);
        WaveChangeover();
    }

    void WaveChangeStep1() {
        gameState.ScalesOwned += gameState.GetDefeatedCPUs().Count;
        if (gameState.ScalesOwned >= 5 && gameState.GetDefeatedPCs().Count > 0) {
            ReviveUI.SetActive(true);
        } else {
            WaveChangeStep2();
        }
    }

    //TODO: UI Manager
    void WaveChangeStep2() {
        BoonUI.SetActive(true);
    }

    void WaveChangeover() {
        gameState.WaveNumber++;
        SetupWave();
    }

    void StageChangeover() {
        gameState.StageNumber++;
        gameState.WaveNumber = 1;
        StageResetPlayerCharacters();
        SetupWave();
    }

    void StageResetPlayerCharacters() {
        List<Combatant> PCs = gameState.GetAllPCs();
        foreach (Combatant pc in PCs) {
            pc.currentHealth = pc.maximumHealth;
            pc.isDead = false;
        }
    }

    void PurgeDeadCombatantsFromQueue() {
        List<Combatant> combatantsToRemove = combatState.TurnOrder.ToList().FindAll(combatant => combatant.isDead);
        combatState.TurnOrder = new Queue<Combatant>(combatState.TurnOrder.ToList().FindAll(combatant => !combatant.isDead));
    }

    void PROGRESSBOARD() {
        PurgeDeadCombatantsFromQueue();
        // Check Win Conditions
        if (gameState.GetAlivePCs().Count == 0) {
            Debug.Log("The CPUs have won!");
            LoseUI.SetActive(true);
            CleanupOnEndTrigger();
            return;
        } else if (gameState.GetAliveCPUs().Count == 0) {
            if (gameState.WaveNumber == 5) {
                if (gameState.StageNumber == 4) {
                    Debug.Log("The PCs have won the game!");
                    WinUI.SetActive(true);
                    CleanupOnEndTrigger();
                    return;
                } else {
                    StageChangeover();
                    return;
                }
            } else {
                WaveChangeStep1();
                return;
            }
        }

        MoveToNextCombatant();
    }

    // TODO: Split between UIManager and Referee
    void CleanupOnEndTrigger() {
        combatState.CurrentCombatant = null;
        AdjustCurrentCombatantUI();
    }

    void MoveToNextCombatant() {
        if (combatState.CurrentCombatant != null) combatState.CurrentCombatant.TurnEnd();

        // TODO: can prob just do the pop on the combat state
        combatState.CurrentCombatant = combatState.TurnOrder.Dequeue();

        AdjustCurrentCombatantUI();
        Debug.Log("It is now " + combatState.CurrentCombatant.gameObject.name + "'s turn.");
        combatState.CurrentCombatant.TurnStart();
        combatState.TurnOrder.Enqueue(combatState.CurrentCombatant);
        if (combatState.CurrentCombatant is CpuCharacter) {
            Debug.Log(combatState.CurrentCombatant.gameObject.name + " is a CPU. It is now their turn.");
            DoCpuTurn();
        }
    }

    // TODO: Give to UIManager
    void AdjustCurrentCombatantUI() {
        if (combatState.CurrentCombatant != null && combatState.CurrentCombatant is PlayerCharacter) {
            combatantUI.SetActive(true);
        } else {
            combatantUI.SetActive(false);
        }
    }

    // TODO: Probably ref job but involves SkirmishResolver
    public void ReportCombatantClicked(Combatant target) {
        if (target is CpuCharacter && combatState.CurrentCombatant is PlayerCharacter) {
            ExecuteAttack(false, combatState.CurrentCombatant, target);
        }
    }

    // TODO: Totally a SkirmishResolver Job, ref gives more info over though
    void ExecuteAttack(bool special, Combatant attacker, Combatant target) {
        if (special) {
            Debug.Log(attacker.gameObject.name + " used a special attack on " + target.gameObject.name + "!");
        } else {
            Debug.Log(attacker.gameObject.name + " attacked " + target.gameObject.name + "!");
            target.HandleIncomingAttack(attacker.powerType, attacker);
        }
        PROGRESSBOARD();
    }

    // TODO: Needs to be resolved by AIStrategy
    void DoCpuTurn() {
        StartCoroutine("CpuTurn");
    }

    // TODO: give to AIStrategy
    IEnumerator CpuTurn() {
        if (combatState.CurrentCombatant is CpuCharacter cpu) {
            if (cpu.currentStagger == 0) {
                cpu.RestoreStagger();
            }
            yield return new WaitForSeconds(1f);
            ExecuteAttack(false, combatState.CurrentCombatant, gameState.getRandomPlayerCharacter());
        } else {
            yield break;
        }
    }

    // TODO: Belongs to WaveProvider
    List<EnemyType> GetEnemiesForWave() {
        List<EnemyType> enemies = new List<EnemyType>();
        switch (gameState.WaveNumber) {
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
