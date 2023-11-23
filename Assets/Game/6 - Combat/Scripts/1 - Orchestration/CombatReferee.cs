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
    BoonLibrary boonLibrary;

    // Coworkers
    UIManager __uiManager;
    StageChoreographer __stageChoreographer;
    SpawnPointProvider __spawnPointProvider;

    // Phase State
    CombatPhase CurrentCombatPhase = CombatPhase.INIT;
    [SerializeField] bool CombatAwaitingUser = false;

    void Awake() {
        eventProvider = new EventProvider();
        gameState = new GameState();
        waveProvider = new WaveProvider(PlayerParty, EnemySetList);
        boonLibrary = new BoonLibrary(PlayerParty);
        __uiManager = GetComponent<UIManager>();
        __stageChoreographer = GetComponent<StageChoreographer>();
        __spawnPointProvider = GetComponent<SpawnPointProvider>();
        
        combatState = new CombatState(eventProvider, __spawnPointProvider);
    }

    void Start()
    {
        eventProvider.OnInput_CombatantChoseAbility += HandleIncomingCombatantAbilityChoice;
        eventProvider.OnInput_CombatantChoseTarget += TargetSelected;
        eventProvider.OnInput_BoonSelected += HandleUserChoseBoon;
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

                combatState.ResolvePreflightBuffsForCurrentCombatant();

                bool HasAbilityOptions = combatState.GetAvailableAbilitiesForCurrentCombatant().Count > 0;
                bool SkipToCleanup = !HasAbilityOptions || combatState.CurrentCombatant.isDead;
                if (SkipToCleanup) {
                    NextPhase = CombatPhase.CHARACTERTURN_CLEANUP;
                }
                break;
            case CombatPhase.CHARACTERTURN_CHOOSEABILITY:
                NextPhase = CombatPhase.CHARACTERTURN_CHOOSETARGET;
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

                if (combatState.CurrentCombatant.HasBuff<BuffTaunted>()) {
                    TargetSelected(combatState.CurrentCombatant.Buffs.Find(buff => buff is BuffTaunted).Source);
                    break;
                }

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

    void ExecuteAttack() {
        combatState.ExecuteSelectedAbility();
        CombatAwaitingUser = false;
    }

















    void SetupParty() {
        List<CharacterConfig> pcsToMake = waveProvider.GetPCParty();

        pcsToMake.ForEach(pc => combatState.SummonUnitForTeam(pc, TeamType.PLAYER));
    }

   
    // TODO: Simplify into less calls to combatState
    void SetupWave() {
        // make sure the queue is empty
        combatState.ClearTurnOrder();

        // TODO: orchestrate through stagechoreo
        ClearStage();

        // clear out enemies from last wave
        combatState.FullCombatantList.RemoveAll(combatant => combatant.Config.TeamType == TeamType.CPU);

        // spawn enemies for this wave
        List<CharacterConfig> enemiesToMake = waveProvider.GetEnemyWave(gameState.StageNumber, gameState.WaveNumber);

        enemiesToMake.ForEach(enemy => combatState.SummonUnitForTeam(enemy, TeamType.CPU));

        combatState.ClearWaveCounters();

        // set up combatant queue
        SeedCombatQueue();

        // set up current combatant
        combatState.MoveToNextCombatant();

        eventProvider.OnWaveReady?.Invoke();
        // let the phase driver run this
        StartCoroutine(CombatPhaseDriver());
    }

    // TODO: probably a BFP job
    void ClearStage() {
        // make sure there aren't CPUs in the field
        foreach (Transform child in transform) {
            Character checkChar = child.gameObject.GetComponent<Character>();
            if (checkChar != null && checkChar.Config.TeamType == TeamType.CPU) {
                Destroy(child.gameObject);
            }
        }
    }

    // TODO: combat state job
    void SeedCombatQueue()
    {
        // add all combatants to the queue in a random order
        List<Character> shuffledCombatants = combatState.FullCombatantList.OrderBy(combatant => Random.Range(0, 100)).ToList();
        foreach(Character combatant in shuffledCombatants)
        {
            combatState.AddCharacterToTurnOrder(combatant);
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

    // fine here
    void WaveChangeStep1() {
        combatState.MoveToNextCombatant(); // lets us make sure we cleaned up after last turn
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Count;
        if (gameState.ScalesOwned >= 5 && combatState.GetDefeatedPCs().Count > 0) {
            ReviveUI.SetActive(true);
        } else {
            WaveChangeStep2();
        }
    }

    // fine here
    void WaveChangeStep2() {
        // make up the boon offer
        List<BaseBoonResolver> boons = boonLibrary.GetRandomBoonOptionsForParty(3);

        // event announce the offer
        eventProvider.OnBoonOffer?.Invoke(boons);
    }

    // fine here
    void WaveChangeover() {
        gameState.WaveNumber++;
        SetupWave();
    }

    // fine here
    void StageChangeover() {
        combatState.MoveToNextCombatant(); // lets us make sure we cleaned up after last turn
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Count;
        gameState.StageNumber++;
        gameState.WaveNumber = 1;
        StageResetPlayerCharacters();
        SetupWave();
    }

    // fine here
    void StageResetPlayerCharacters() {
        List<Character> PCs = combatState.GetAllPCs();
        foreach (Character pc in PCs) {
            pc.currentHealth = pc.Config.BaseHP;
            pc.isDead = false;
        }
    }

    // TODO: needs love, but is close
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

    // good for ref
    void TargetSelected(Character selectedCharacter) {
        if (CurrentCombatPhase == CombatPhase.CHARACTERTURN_CHOOSETARGET && CombatAwaitingUser) {
            combatState.TargetSelected = selectedCharacter;
            CombatAwaitingUser = false;
        }
    }

    // good for ref
    void HandleUserChoseBoon(BaseBoonResolver selectedBoon) {
        selectedBoon.ApplyToEligible(PlayerParty.PartyMembers);
        WaveChangeover();
    }


    // TODO: Give most of this to combatState. too much info for CombatRef
    void HandleIncomingCombatantAbilityChoice(AbilityCategory category) {
        UserAbilitySelection abilityChosen;
        switch(category) {
            case AbilityCategory.SPECIALATTACK:
                abilityChosen = combatState.CurrentCombatant.Config.SpecialAttack;
                break;
            case AbilityCategory.ULTIMATE:
                abilityChosen = combatState.CurrentCombatant.Config.UltimateAbility;
                break;
            case AbilityCategory.BASICATTACK:
            default:
                abilityChosen = UserAbilitySelection.BASICATTACK;
                break;
        }
        AttackSelected(abilityChosen);
    }

    // TODO: Give most of this to combatState. too much info for CombatRef
    void AttackSelected(UserAbilitySelection attack) {
        if (CurrentCombatPhase == CombatPhase.CHARACTERTURN_CHOOSEABILITY && CombatAwaitingUser) {
            combatState.AbilitySelected = AttackTypeToAbility
            .Lookup(attack, combatState.CurrentCombatant.Config);

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
        Character randomTarget = EligibleTargets[UnityEngine.Random.Range(0, EligibleTargets.Count)];

        TargetSelected(randomTarget);
    }

    // TODO: give to AIStrategy
    IEnumerator IECpuChooseAbility() {
        yield return new WaitForSeconds(1f);

        bool successfulRollToSpecialAttack;
        switch (gameState.StageNumber) {
            case 1:
                successfulRollToSpecialAttack = TryChance(25);
                break;
            case 2:
                successfulRollToSpecialAttack = TryChance(35);
                break;
            case 3:
                successfulRollToSpecialAttack = TryChance(45);
                break;
            default:
                successfulRollToSpecialAttack = TryChance(55);
                break;
        }

        List<AbilityCategory> availableAbilities = combatState.CurrentCombatant.GetAvailableAbilities(2, 2);

        bool canSpecialAttack = combatState.CurrentCombatant.Config.SpecialAttack != UserAbilitySelection.NONE && availableAbilities.Contains(AbilityCategory.SPECIALATTACK);

        bool canUltimate = combatState.CurrentCombatant.Config.SpecialAttack != UserAbilitySelection.NONE && availableAbilities.Contains(AbilityCategory.ULTIMATE);

        if (!canSpecialAttack && !canUltimate) {
            AttackSelected(UserAbilitySelection.BASICATTACK);
            yield break;
        }

        if (!successfulRollToSpecialAttack) {
            AttackSelected(UserAbilitySelection.BASICATTACK);
            yield break;
        }

        if (!canUltimate) {
            AttackSelected(combatState.CurrentCombatant.Config.SpecialAttack);
            yield break;
        }

        if (!canSpecialAttack) {
            AttackSelected(combatState.CurrentCombatant.Config.UltimateAbility);
            yield break;
        }
        
        bool rollToUltimate = TryChance(50);
        if (rollToUltimate) {
            AttackSelected(combatState.CurrentCombatant.Config.UltimateAbility);
        } else {
            AttackSelected(combatState.CurrentCombatant.Config.SpecialAttack);
        }
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

    [ContextMenu("Revive All PCs")]
    void ReviveAllPCs() {
        List<Character> PCs = combatState.GetAllPCs();
        PCs.ForEach(c => combatState.ReviveCharacter(
            new ReviveOrder(c, 100, null)
        ));
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
    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }
}

