using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

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
        eventProvider = new EventProvider();
        gameState = new GameState();
        combatState = new CombatState();
        waveProvider = new WaveProvider(PlayerParty, EnemySetList);
        __uiManager = GetComponent<UIManager>();
        __stageChoreographer = GetComponent<StageChoreographer>();
    }

    void Start()
    {
        eventProvider.OnInput_CombatantChoseAbility += HandleIncomingCombatantAbilityChoice;
        eventProvider.OnInput_CombatantChoseTarget += TargetSelected;
        SetupParty();
        SetupWave();
    }


    CombatPhase ExecuteGameLogicForPhase(CombatPhase CurrentPhase) {
        CombatPhase NextPhase = CombatPhase.INIT;
        switch(CurrentPhase) {
            case CombatPhase.INIT:
                NextPhase = CombatPhase.WAVE_SETUP;
                break;
            case CombatPhase.WAVE_SETUP:
                NextPhase = CombatPhase.CHARACTERTURN_PREFLIGHT;
                break;
            case CombatPhase.CHARACTERTURN_PREFLIGHT:
                NextPhase = CombatPhase.CHARACTERTURN_CHOOSEABILITY;
                
                // TODO: This necessary?
                combatState.CurrentCombatant.TurnStart();

                combatState.ClearSelections();
                
                if (combatState.CurrentCombatant.Config.TeamType == TeamType.CPU) {
                    if (combatState.CurrentCombatant.currentStagger == 0) {
                        combatState.CurrentCombatant.RestoreStagger();
                    }
                }

                ResolvePreflightBuffEffectsForCombatant(combatState.CurrentCombatant);

                bool IsStunned = combatState.CurrentCombatant.HasBuff<BuffStunned>();
                bool SkipToCleanup = IsStunned || combatState.CurrentCombatant.isDead;
                if (SkipToCleanup) {
                    NextPhase = CombatPhase.CHARACTERTURN_CLEANUP;
                }
                break;
            case CombatPhase.CHARACTERTURN_CHOOSEABILITY:
                NextPhase = CombatPhase.CHARACTERTURN_CHOOSETARGET;

                if (combatState.CurrentCombatant.HasBuff<BuffCharmed>()) {
                    AttackSelected(UserAbilitySelection.BASICATTACK);
                    break;
                }

                CombatAwaitingUser = true;
                if (combatState.CurrentCombatant.Config.TeamType == TeamType.CPU) {
                    DoCpuAbilityChoice();
                }
                break;
            case CombatPhase.CHARACTERTURN_CHOOSETARGET:
                NextPhase = CombatPhase.CHARACTERTURN_EXECUTION;

                bool ATargetNeedsToBeSelected = combatState.AbilitySelected.TargetScope != EligibleTargetScopeType.NONE;
                bool isCpuTurn = combatState.CurrentCombatant.Config.TeamType == TeamType.CPU;

                CombatAwaitingUser = ATargetNeedsToBeSelected;

                if (ATargetNeedsToBeSelected && isCpuTurn) {
                    DoCpuTargetChoice();
                }
                break;
            case CombatPhase.CHARACTERTURN_EXECUTION:
                ExecuteAttack();
                NextPhase = CombatPhase.CHARACTERTURN_CLEANUP;
                break;
            case CombatPhase.CHARACTERTURN_CLEANUP:
                // TODO: this necessary?
                combatState.CurrentCombatant.TurnEnd();

                if (combatState.CurrentCombatant.HasBuff<BuffMultistrike>()) {
                    Debug.Log(combatState.CurrentCombatant.gameObject.name + " - MULTISTRIKE TRIGGER. Adding another turn.");
                    combatState.AddCharacterTurnNext(combatState.CurrentCombatant);
                }

                NextPhase = CombatPhase.CHARACTERTURN_HANDOFF;
                break;
            case CombatPhase.CHARACTERTURN_HANDOFF:
                combatState.MoveToNextCombatant();
                NextPhase = CombatPhase.CHARACTERTURN_PREFLIGHT;
                break;
            default:
                NextPhase = CombatPhase.WAVE_COMPLETE;
                break;
        }

        // age buffs for phase on combatant
        combatState.CurrentCombatant.AgeBuffsForPhase(CurrentPhase);

        return NextPhase;
    }

    IEnumerator CombatPhaseDriver() {
        Debug.Log("PHASE DRIVER STARTED");
        CombatPhase nextPhase = CombatPhase.INIT;
        bool CombatIsComplete = false;

        while (!CombatIsComplete) {
            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming) {
                yield return new WaitForSeconds(0.1f);
            }
Debug.LogWarning(combatState.CurrentCombatant.gameObject.name + " - PHASE SET TO: " + nextPhase.ToString());
            CurrentCombatPhase = nextPhase;

            eventProvider.OnPhaseAwake?.Invoke(CurrentCombatPhase, combatState.CurrentCombatant);

            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming) {
                yield return new WaitForSeconds(0.1f);
            }

Debug.LogWarning(combatState.CurrentCombatant.gameObject.name + " PHASE LOGIC GO: " + CurrentCombatPhase.ToString());
            nextPhase = ExecuteGameLogicForPhase(CurrentCombatPhase);

            if (CheckCombatWinConditions() != CombatResult.IN_PROGRESS) {
                CombatIsComplete = true;
                eventProvider.OnCombatHasEnded?.Invoke();
            } else {
                eventProvider.OnPhasePrompt?.Invoke(CurrentCombatPhase, combatState.CurrentCombatant);
Debug.LogWarning(combatState.CurrentCombatant.gameObject.name + " PHASE PROMPTED: " + CurrentCombatPhase.ToString());
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

            eventProvider.OnPhaseExiting?.Invoke(CurrentCombatPhase, combatState.CurrentCombatant);
        }
        CurrentCombatPhase = CombatPhase.WAVE_COMPLETE;
        PROGRESSBOARD();
    }

    CombatResult CheckCombatWinConditions() {
        if (combatState.GetAlivePCs().Count == 0) {
            return CombatResult.DEFEAT;
        }
        if (combatState.GetAliveCPUs().Count == 0) {
            return CombatResult.VICTORY;
        }
        return CombatResult.IN_PROGRESS;
    }

    void ResolvePreflightBuffEffectsForCombatant(Character combatant) {
        List<ExecutedAbility> abilityEffects = combatant
            .Buffs
            .Select(buff => buff.ResolvePreflightEffects())
            .Where(ability => ability != null)
            .ToList();

        foreach (ExecutedAbility ability in abilityEffects) {
            eventProvider.OnAbilityExecuted?.Invoke(ability);
        }
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
            combatState.FullCombatantList.Add(newPc.GetComponent<Character>());

            // set up config
            newPc.GetComponent<Character>().Config = pcsToMake[i];

            // TODO: Instead of init, should be in the creation process somehow
            newPc.GetComponent<Character>().InitializeMe();

            // position it
            UnityEngine.Vector3 targetPos = GetFriendlySpawnPointByIndex(i).position;
            newPc.transform.parent = GameObject.Find("GameManager").transform;
            newPc.transform.position = new UnityEngine.Vector3(targetPos.x, targetPos.y, targetPos.y);
        }
    }

   

    void SetupWave() {
        // make sure the queue is empty
        combatState.ClearTurnOrder();

        // TODO: orchestrate through stagechoreo
        ClearStage();

        // clear out enemies from last wave
        combatState.FullCombatantList.RemoveAll(combatant => combatant.Config.TeamType == TeamType.CPU);

        // spawn enemies for this wave
        List<CharacterConfig> enemiesToMake = waveProvider.GetEnemyWave(gameState.StageNumber, gameState.WaveNumber);

        // TODO: Send to SC via events
        // GetComponent<StageChoreographer>().SpawnEnemyWave(enemiesToMake);
        for (var i=0; i < enemiesToMake.Count; i++) {
            // create game object
            GameObject newEnemy = Instantiate(CharacterPuckPrefab);

            // register in game state
            combatState.FullCombatantList.Add(newEnemy.GetComponent<Character>());
            newEnemy.name = enemiesToMake[i].Name;
            // set up config
            newEnemy.GetComponent<Character>().Config = enemiesToMake[i];

            // init the character
            newEnemy.GetComponent<Character>().InitializeMe();

            // position it
            UnityEngine.Vector3 targetPos = GetSpawnPointByIndex(i).position;
            newEnemy.transform.parent = GameObject.Find("GameManager").transform;
            newEnemy.transform.position = new UnityEngine.Vector3(targetPos.x, targetPos.y, targetPos.y);
        }


        // set up combatant queue
        SeedCombatQueue();

        // set up current combatant
        combatState.MoveToNextCombatant();

        eventProvider.OnWaveReady?.Invoke();
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

    void ClearStage() {
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
        List<Character> shuffledCombatants = combatState.FullCombatantList.OrderBy(combatant => Random.Range(0, 100)).ToList();
        foreach(Character combatant in shuffledCombatants)
        {
            combatState.AddCharacterToTurnOrder(combatant);
        }
    }

    [ContextMenu("Revive All PCs")]
    void ReviveAllPCs() {
        List<Character> PCs = combatState.GetAllPCs();
        foreach (Character pc in PCs) {
            if (pc.isDead) {
                combatState.AddCharacterToTurnOrder(pc);
            }
            pc.currentHealth = pc.Config.BaseHP;
            pc.isDead = false;
            eventProvider.OnCharacterRevived?.Invoke(pc);
        }
    }

    [ContextMenu("Defeat Wave")]
    void DefeatWave() {
        List<Character> chars = combatState.GetAliveCPUs();
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
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Count;
        if (gameState.ScalesOwned >= 5 && combatState.GetDefeatedPCs().Count > 0) {
            ReviveUI.SetActive(true);
        } else {
            WaveChangeStep2();
        }
    }

    void WaveChangeStep2() {
        eventProvider.OnBoonOffer?.Invoke();
    }

    void WaveChangeover() {
        gameState.WaveNumber++;
        SetupWave();
    }

    void StageChangeover() {
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Count;
        gameState.StageNumber++;
        gameState.WaveNumber = 1;
        StageResetPlayerCharacters();
        SetupWave();
    }

    void StageResetPlayerCharacters() {
        List<Character> PCs = combatState.GetAllPCs();
        foreach (Character pc in PCs) {
            pc.currentHealth = pc.Config.BaseHP;
            pc.isDead = false;
        }
    }

    void PROGRESSBOARD() {
        // Check Win Conditions
        if (combatState.GetAlivePCs().Count == 0) {
            // TODO: Should trigger an event and let UI Manager handle this.
            Debug.Log("The CPUs have won!");
            LoseUI.SetActive(true);
            return;
        } else if (combatState.GetAliveCPUs().Count == 0) {
            if (gameState.WaveNumber == waveProvider.WaveCountInStage(gameState.StageNumber)) {
                if (gameState.StageNumber == waveProvider.StageCount) {
                    // TODO: Should trigger an event and let UI Manager handle this.
                    Debug.Log("The PCs have won the game!");
                    WinUI.SetActive(true);
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

    void TargetSelected(Character selectedCharacter) {
        if (CurrentCombatPhase == CombatPhase.CHARACTERTURN_CHOOSETARGET && CombatAwaitingUser) {
            combatState.TargetSelected = selectedCharacter;
            CombatAwaitingUser = false;
        }
    }

    void HandleIncomingCombatantAbilityChoice(bool isBasic) {
        AttackSelected(isBasic ? UserAbilitySelection.BASICATTACK : combatState.CurrentCombatant.Config.SpecialAttack);
    }

    void AttackSelected(UserAbilitySelection attack) {
        if (CurrentCombatPhase == CombatPhase.CHARACTERTURN_CHOOSEABILITY && CombatAwaitingUser) {
            combatState.AbilitySelected = AttackTypeToAbility
            .Lookup(attack);

            List<Character> EligibleTargets = CombatantListFilter.ByScope(
                combatState.FullCombatantList,
                combatState.CurrentCombatant,
                combatState.AbilitySelected.TargetScope
            );

            eventProvider.OnEligibleTargetsChanged?.Invoke(
                EligibleTargets
            );
            CombatAwaitingUser = false;
        }
    }

    void ExecuteAttack() {
        ExecutedAbility completedAbility = combatState.ExecuteSelectedAbility();
        eventProvider.OnAbilityExecuted?.Invoke(completedAbility);
        CombatAwaitingUser = false;
    }

    // TODO: Needs to be resolved by AIStrategy
    void DoCpuAbilityChoice() {
        StartCoroutine("IECpuChooseAbility");
    }

    void DoCpuTargetChoice() {
        StartCoroutine("IECpuChooseTarget");
    }


    IEnumerator IECpuChooseTarget() {
        yield return new WaitForSeconds(1f);
        List<Character> EligibleTargets = combatState.GetEligibleTargetsForSelectedAbility();

        // get random eligibletarget
        Character randomTarget = EligibleTargets[Random.Range(0, EligibleTargets.Count)];

        TargetSelected(randomTarget);
    }

    // TODO: give to AIStrategy
    IEnumerator IECpuChooseAbility() {
        yield return new WaitForSeconds(1f);
        if (combatState.CurrentCombatant.HasBuff<BuffCharmed>()) {
            AttackSelected(UserAbilitySelection.BASICATTACK);
            yield break;
        }

        AttackSelected(UserAbilitySelection.BASICATTACK);
    }

    void Update(){
        CheatCodes();
    }

    void CheatCodes() {
        if (Input.GetKeyDown(KeyCode.V)) {
            DefeatWave();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            ReviveAllPCs();
        }
    }
}

