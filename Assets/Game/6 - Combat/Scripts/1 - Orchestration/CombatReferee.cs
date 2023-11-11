using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CombatReferee : MonoBehaviour
{  
    [Header("Game Setup")]
    [SerializeField]
    private EnemySetList EnemySetList;
    [SerializeField]
    private PartyConfig PlayerParty;

    [Space(height: 20)]

    [Header("Temporary Exposed Plumbing")]
    // TODO: need a helper
    public GameObject CharacterPuckPrefab;
    
    // TODO: Give to UIManager
    public GameObject WinUI;
    // TODO: Give to UIManager
    public GameObject LoseUI;
    //TODO: give to UIManager
    public GameObject ReviveUI;

    // Long-Term Referee Stuff
    public GameState gameState;
    public CombatState combatState;
    public EventProvider eventProvider;
    WaveProvider waveProvider;

    // Coworkers
    UIManager __uiManager;
    StageChoreographer __stageChoreographer;
    CombatPhase CurrentCombatPhase = CombatPhase.INIT;
    [SerializeField] bool CombatAwaitingUser = false;


    void Awake() {
        // TODO: generate these via stagechoreo later, for now, using already in-scene characters
        List<Character> PCTeam = new List<Character>();
        eventProvider = new EventProvider();
        gameState = new GameState();
        combatState = new CombatState();
        waveProvider = new WaveProvider(PlayerParty, EnemySetList);
        __uiManager = GetComponent<UIManager>();
        __stageChoreographer = GetComponent<StageChoreographer>();
    }

    void Start()
    {
        SetupParty();
        SetupWave();
    }


    CombatPhase ExecuteGameLogicForPhase(CombatPhase phase) {
        switch(phase) {
            case CombatPhase.INIT:
                return CombatPhase.WAVE_SETUP;
            case CombatPhase.WAVE_SETUP:
                return CombatPhase.CHARACTERTURN_PREFLIGHT;
            case CombatPhase.CHARACTERTURN_PREFLIGHT:
                return CombatPhase.CHARACTERTURN_CHOOSEABILITY;
            case CombatPhase.CHARACTERTURN_CHOOSEABILITY:
                    if (combatState.CurrentCombatant.Config.TeamType == TeamType.CPU) {
                        Debug.Log(combatState.CurrentCombatant.gameObject.name + " is a CPU. It is now their turn.");
                        CombatAwaitingUser = true;
                        DoCpuTurn();
                    }
                return CombatPhase.CHARACTERTURN_CHOOSETARGET;
            case CombatPhase.CHARACTERTURN_CHOOSETARGET:
                if (combatState.CurrentCombatant.Config.TeamType == TeamType.PLAYER) {
                    CombatAwaitingUser = true;
                }
                return CombatPhase.CHARACTERTURN_EXECUTION;
            case CombatPhase.CHARACTERTURN_EXECUTION:
                return CombatPhase.CHARACTERTURN_CLEANUP;
            case CombatPhase.CHARACTERTURN_CLEANUP:
                return CombatPhase.CHARACTERTURN_HANDOFF;
            case CombatPhase.CHARACTERTURN_HANDOFF:
                MoveToNextCombatant();
                return CombatPhase.CHARACTERTURN_PREFLIGHT;
            default:
                return CombatPhase.WAVE_COMPLETE;
        }
    }

    IEnumerator CombatPhaseDriver() {
        Debug.Log("PHASE DRIVER STARTED");
        CombatPhase nextPhase = CombatPhase.INIT;
        bool CombatIsComplete = false;

        while (!CombatIsComplete) {
            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming) {
                yield return new WaitForSeconds(0.1f);
            }
Debug.Log("PHASE SET TO: " + nextPhase.ToString());
            CurrentCombatPhase = nextPhase;

            eventProvider.OnPhaseAwake?.Invoke(CurrentCombatPhase);

            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming) {
                yield return new WaitForSeconds(0.1f);
            }

Debug.Log("PHASE LOGIC GO: " + CurrentCombatPhase.ToString());
            nextPhase = ExecuteGameLogicForPhase(CurrentCombatPhase);

            if (CheckCombatWinConditions() != CombatResult.IN_PROGRESS) {
                CombatIsComplete = true;
                eventProvider.OnCombatHasEnded?.Invoke();
            } else {
                eventProvider.OnPhasePrompt?.Invoke(CurrentCombatPhase);
Debug.Log("PHASE PROMPTED: " + CurrentCombatPhase.ToString());
                while (CombatAwaitingUser) {
                    yield return new WaitForSeconds(0.1f);
                }

                if (CheckCombatWinConditions() != CombatResult.IN_PROGRESS) {
                    CombatIsComplete = true;
                    eventProvider.OnCombatHasEnded?.Invoke();
                }
            }

            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming) {
                yield return new WaitForSeconds(0.1f);
            }

            eventProvider.OnPhaseExiting?.Invoke(CurrentCombatPhase);
        }
        CurrentCombatPhase = CombatPhase.WAVE_COMPLETE;
        PROGRESSBOARD();
    }

    CombatResult CheckCombatWinConditions() {
        if (gameState.GetAlivePCs().Count == 0) {
            return CombatResult.DEFEAT;
        }
        if (gameState.GetAliveCPUs().Count == 0) {
            return CombatResult.VICTORY;
        }
        return CombatResult.IN_PROGRESS;
    }





















    // TODO: SOOOOO not DRY. fix this!!
    void SetupParty() {
        // spawn enemies for this wave
        List<CharacterConfig> pcsToMake = waveProvider.GetPCParty();

        // TODO: Send to SC via events
        // GetComponent<StageChoreographer>().SpawnEnemyWave(enemiesToMake);
        for (var i=0; i < pcsToMake.Count; i++) {
            // create game object
            GameObject newPc = Instantiate(CharacterPuckPrefab);
            Debug.Log(GetFriendlySpawnPointByIndex(i).position);
            newPc.name = pcsToMake[i].Name;

            // register in game state
            gameState.combatants.Add(newPc.GetComponent<Character>());

            // set up config
            newPc.GetComponent<Character>().Config = pcsToMake[i];

            // TODO: Instead of init, should be in the creation process somehow
            newPc.GetComponent<Character>().InitializeMe();

            // position it
            newPc.transform.parent = GameObject.Find("GameManager").transform;
            newPc.transform.position = GetFriendlySpawnPointByIndex(i).position;
        }
    }

   

    void SetupWave() {
        // make sure the queue is empty
        combatState.TurnOrder.Clear();

        // TODO: orchestrate through stagechoreo
        ClearStage();

        // clear out enemies from last wave
        gameState.combatants.RemoveAll(combatant => combatant.Config.TeamType == TeamType.CPU);

        // spawn enemies for this wave
        List<CharacterConfig> enemiesToMake = waveProvider.GetEnemyWave(gameState.StageNumber, gameState.WaveNumber);

        // TODO: Send to SC via events
        // GetComponent<StageChoreographer>().SpawnEnemyWave(enemiesToMake);
        for (var i=0; i < enemiesToMake.Count; i++) {
            // create game object
            GameObject newEnemy = Instantiate(CharacterPuckPrefab);
            Debug.Log(GetSpawnPointByIndex(i).position);

            // register in game state
            gameState.combatants.Add(newEnemy.GetComponent<Character>());
            newEnemy.name = enemiesToMake[i].Name;
            // set up config
            newEnemy.GetComponent<Character>().Config = enemiesToMake[i];

            // init the character
            newEnemy.GetComponent<Character>().InitializeMe();

            // position it
            newEnemy.transform.parent = GameObject.Find("GameManager").transform;
            newEnemy.transform.position = GetSpawnPointByIndex(i).position;
        }


        // set up combatant queue
        SeedCombatQueue();

        // set up current combatant
        MoveToNextCombatant();

        // let the phase driver run this
        StartCoroutine(CombatPhaseDriver());
    }

    // TODO: Decouple this, its depending on children
    Transform GetSpawnPointByIndex(int index) {
        return transform.Find("Enemy Field").transform.Find("SpawnPoint" + index.ToString());
    }

    // TODO: Decouple this, its depending on children
    Transform GetFriendlySpawnPointByIndex(int index) {
        return transform.Find("Player Field").transform.Find("SpawnPoint" + index.ToString());
    }

    // TODO: Ought to decouple and have a performance here
    public void ClearStage() {
        // make sure there aren't CPUs in the field
        foreach (Transform child in transform) {
            Character checkChar = child.gameObject.GetComponent<Character>();
            if (checkChar != null && checkChar.Config.TeamType == TeamType.CPU) {
                Destroy(child.gameObject);
            }
        }
    }

    // REFEREE JOB, maybe a shuffle resolver?
    void SeedCombatQueue()
    {
        // add all combatants to the queue in a random order
        List<Character> shuffledCombatants = gameState.combatants.OrderBy(combatant => Random.Range(0, 100)).ToList();
        foreach(Character combatant in shuffledCombatants)
        {
            combatState.TurnOrder.Enqueue(combatant);
        }
    }

    [ContextMenu("Revive All PCs")]
    void ReviveAllPCs() {
        List<Character> PCs = gameState.GetAllPCs();
        foreach (Character pc in PCs) {
            pc.currentHealth = pc.Config.BaseHP;
            pc.isDead = false;
        }
    }

    [ContextMenu("Defeat Wave")]
    void DefeatWave() {
        List<Character> chars = gameState.GetAliveCPUs();
        foreach (Character c in chars) {
            c.currentHealth = 0;
            c.isDead = true;
            CombatAwaitingUser = false;
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
        eventProvider.OnBoonOffer?.Invoke();
    }

    void WaveChangeover() {
        gameState.WaveNumber++;
        SetupWave();
    }

    void StageChangeover() {
        gameState.ScalesOwned += gameState.GetDefeatedCPUs().Count;
        gameState.StageNumber++;
        gameState.WaveNumber = 1;
        StageResetPlayerCharacters();
        SetupWave();
    }

    void StageResetPlayerCharacters() {
        List<Character> PCs = gameState.GetAllPCs();
        foreach (Character pc in PCs) {
            pc.currentHealth = pc.Config.BaseHP;
            pc.isDead = false;
        }
    }

    void PurgeDeadCombatantsFromQueue() {
        List<Character> combatantsToRemove = combatState.TurnOrder.ToList().FindAll(combatant => combatant.isDead);
        combatState.TurnOrder = new Queue<Character>(combatState.TurnOrder.ToList().FindAll(combatant => !combatant.isDead));
    }

    void PROGRESSBOARD() {
        // Check Win Conditions
        if (gameState.GetAlivePCs().Count == 0) {
            Debug.Log("The CPUs have won!");
            LoseUI.SetActive(true);
            // CleanupOnEndTrigger();
            return;
        } else if (gameState.GetAliveCPUs().Count == 0) {
            if (gameState.WaveNumber == 5) {
                if (gameState.StageNumber == 4) {
                    Debug.Log("The PCs have won the game!");
                    WinUI.SetActive(true);
                    // CleanupOnEndTrigger();
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
    }

    // TODO: Split between UIManager and Referee
    void CleanupOnEndTrigger() {
        combatState.CurrentCombatant = null;
    }

    void MoveToNextCombatant() {
        PurgeDeadCombatantsFromQueue();
        if (combatState.CurrentCombatant != null) combatState.CurrentCombatant.TurnEnd();

        // TODO: can prob just do the pop on the combat state
        combatState.CurrentCombatant = combatState.TurnOrder.Dequeue();

        eventProvider.OnCharacterTurnStart?.Invoke(combatState.CurrentCombatant);

        combatState.CurrentCombatant.TurnStart();
        combatState.TurnOrder.Enqueue(combatState.CurrentCombatant);
    }

    // TODO: Probably ref job but involves SkirmishResolver
    public void ReportCombatantClicked(Character target) {
        if (target.Config.TeamType == TeamType.CPU && combatState.CurrentCombatant.Config.TeamType == TeamType.PLAYER && CombatAwaitingUser) {
            ExecuteAttack(false, combatState.CurrentCombatant, target);
        }
    }

    // TODO: Totally a SkirmishResolver Job, ref gives more info over though
    void ExecuteAttack(bool special, Character attacker, Character target) {
        if (special) {
            Debug.Log(attacker.gameObject.name + " used a special attack on " + target.gameObject.name + "!");
        } else {
            Debug.Log(attacker.gameObject.name + " attacked " + target.gameObject.name + "!");
            target.HandleIncomingAttack(attacker.Config.PowerType, attacker);
        }
        CombatAwaitingUser = false;
    }

    // TODO: Needs to be resolved by AIStrategy
    void DoCpuTurn() {
        StartCoroutine("CpuTurn");
    }

    // TODO: give to AIStrategy
    IEnumerator CpuTurn() {
        if (combatState.CurrentCombatant.Config.TeamType == TeamType.CPU) {
            if (combatState.CurrentCombatant.currentStagger == 0) {
                combatState.CurrentCombatant.RestoreStagger();
            }
            combatState.CurrentCombatant.TurnStart();
            yield return new WaitForSeconds(1f);
            ExecuteAttack(false, combatState.CurrentCombatant, gameState.getRandomPlayerCharacter());
            combatState.CurrentCombatant.TurnEnd();
        }
        CombatAwaitingUser = false;
    }
}
