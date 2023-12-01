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
    bool UserRequestedRevertToAbilitySelection = false;
    GameController_DDOL ddol;

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
        eventProvider.OnInput_BackOutOfTargetSelection += HandleUserTargetBackout;
        eventProvider.OnInput_BoonSelected += HandleUserChoseBoon;
        eventProvider.OnInput_ReviveResponse += UserResponse_Revive;
        eventProvider.OnInput_RetryResponse += HandleWaveRetryRequest;
        SetupParty();
        ddol = FindFirstObjectByType<GameController_DDOL>();        
        StartCoroutine(SetupWave());
    }

    void HandleWaveRetryRequest() {
        if (gameState.LivesLeft == 0) return;

        gameState.LivesLeft--;
        WaveRetry();
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
        List<Buff> agedOutBuffs = combatState.CurrentCombatant.AgeBuffsForPhase(CurrentPhase);
        agedOutBuffs.ForEach(buff => eventProvider.OnBuffExpired?.Invoke(buff));

        return NextPhase;
    }

    IEnumerator CombatPhaseDriver() {
        CombatPhase nextPhase = CombatPhase.INIT;
        bool CombatIsComplete = false;

        while (!CombatIsComplete) {
            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
                yield return new WaitForSeconds(0.1f);
            }
            CurrentCombatPhase = nextPhase;

            eventProvider.OnPhaseAwake?.Invoke(CurrentCombatPhase, combatState.CurrentCombatant);

            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
                yield return new WaitForSeconds(0.1f);
            }

            nextPhase = ExecuteGameLogicForPhase(CurrentCombatPhase);

            if (CheckCombatWinConditions() != CombatResult.IN_PROGRESS) {
                CombatIsComplete = true;
                eventProvider.OnCombatHasEnded?.Invoke();
            } else {
                eventProvider.OnPhasePrompt?.Invoke(CurrentCombatPhase, combatState.CurrentCombatant);

                while (CombatAwaitingUser) {
                    yield return new WaitForSeconds(0.1f);
                }

                if (UserRequestedRevertToAbilitySelection) {
                    UserRequestedRevertToAbilitySelection = false;
                    nextPhase = CombatPhase.CHARACTERTURN_CHOOSEABILITY;
                }

                if (CheckCombatWinConditions() != CombatResult.IN_PROGRESS) {
                    CombatIsComplete = true;
                    eventProvider.OnCombatHasEnded?.Invoke();
                }
            }

            while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
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
    IEnumerator SetupWave() {
        WaveInfo info = waveProvider.GetWaveInfo(gameState.StageNumber, gameState.WaveNumber);
        eventProvider.OnWaveSetupStart?.Invoke(info);
        while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
            yield return new WaitForSeconds(0.1f);
        }

        ClearStage();
        // clear out enemies from last wave
        combatState.FullCombatantList.RemoveAll(combatant => combatant.Config.TeamType == TeamType.CPU);

        // spawn enemies for this wave
        List<CharacterConfig> enemiesToMake = waveProvider.GetEnemyWave(gameState.StageNumber, gameState.WaveNumber);

        enemiesToMake.ForEach(enemy => combatState.SummonUnitForTeam(enemy, TeamType.CPU));

        combatState.ClearWaveCounters();

        // set up combatant queue
        combatState.ClearTurnOrder();
        SeedCombatQueue();

        // set up current combatant
        combatState.MoveToNextCombatant();
        if (gameState.WaveNumber == 1) {
            ddol.PlayMusic("Battle" + gameState.StageNumber.ToString());
            StageConfig stageConfig = waveProvider.GetStageConfig(gameState.StageNumber);
            eventProvider.OnStageSetup?.Invoke(stageConfig);
        }

        while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
            yield return new WaitForSeconds(0.1f);
        }
        eventProvider.OnWaveReady?.Invoke(info);

        while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
            yield return new WaitForSeconds(0.1f);
        }
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
        if (doIt && gameState.ScalesOwned >= 5) {
            gameState.ScalesOwned -= 5;
            ReviveAndHealAllPCs();
        }
        WaveChangeStep2();
    }

    void WaveChangeStep1() {
        combatState.MoveToNextCombatant(); // lets us make sure we cleaned up after last turn
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Count;
        if (gameState.ScalesOwned >= 5 && combatState.GetDefeatedPCs().Count > 0) {
            eventProvider.OnReviveOffered?.Invoke();
        } else {
            WaveChangeStep2();
        }
    }

    void WaveChangeStep2() {
        // make up the boon offer
        List<BaseBoonResolver> boons = boonLibrary.GetRandomBoonOptionsForParty(3);

        // event announce the offer
        eventProvider.OnBoonOffer?.Invoke(boons);
    }

    void WaveRetry() {
        combatState.LightPoints = 0;
        combatState.ShadowPoints = 0;
        ReviveAndHealAllPCs();
        StartCoroutine(SetupWave());
    }

    void WaveChangeover() {
        gameState.WaveNumber++;
        StartCoroutine(SetupWave());
    }

    IEnumerator StageChangeover() {
        combatState.MoveToNextCombatant(); // lets us make sure we cleaned up after last turn
        gameState.ScalesOwned += combatState.GetDefeatedCPUs().Sum(c => c.Config.ScaleBounty);
        WaveInfo FinalWaveDefeated = waveProvider.GetWaveInfo(gameState.StageNumber, gameState.WaveNumber);
        eventProvider.OnStageComplete?.Invoke(FinalWaveDefeated);
        while (__uiManager.IsPerforming || __stageChoreographer.IsPerforming()) {
            yield return new WaitForSeconds(0.1f);
        }
        gameState.StageNumber++;
        gameState.WaveNumber = 1;
        ReviveAndHealAllPCs();
        StartCoroutine(SetupWave());
    }

    // TODO: needs love, but is close
    void PROGRESSBOARD() {
        // Check Win Conditions
        if (combatState.GetAlivePCs().Count == 0) {
            eventProvider.OnWaveComplete?.Invoke();
            eventProvider.OnGameOver?.Invoke(gameState.LivesLeft);
            return;
        } else if (combatState.GetAliveCPUs().Count == 0) {
            if (gameState.WaveNumber == waveProvider.WaveCountInStage(gameState.StageNumber)) {
                if (gameState.StageNumber == waveProvider.StageCount) {
                    eventProvider.OnWaveComplete?.Invoke();
                    eventProvider.OnGameVictory?.Invoke();
                    return;
                } else {
                    eventProvider.OnWaveComplete?.Invoke();
                    StartCoroutine(StageChangeover());
                    return;
                }
            } else {
                eventProvider.OnWaveComplete?.Invoke();
                eventProvider.OnWaveVictory?.Invoke(gameState.WaveNumber);
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
        if (selectedBoon != null) {
            selectedBoon.ApplyToEligible(PlayerParty.PartyMembers);
        }
        WaveChangeover();
    }

    void HandleUserTargetBackout() {
        UserRequestedRevertToAbilitySelection = true;
        CombatAwaitingUser = false;
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
        
        Character randomTarget;
        if (EligibleTargets.Count == 0) {
            randomTarget = combatState.CurrentCombatant;
        } else {
            randomTarget = EligibleTargets[UnityEngine.Random.Range(0, EligibleTargets.Count)];
        }

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
            ReviveAndHealAllPCs();
        }
    }

    [ContextMenu("Revive All PCs")]
    void ReviveAndHealAllPCs() {
        List<Character> PCs = combatState.GetAllPCs().Where(p => p.isDead).ToList();
        PCs.ForEach(c => combatState.ReviveCharacter(
            new ReviveOrder(c, 100, null)
        ));

        combatState.GetAllPCs().ForEach(c => c.currentHealth = c.Config.BaseHP);
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

    [ContextMenu("Check dead")]
    void checkdead() {
        Debug.Log(combatState.GetDefeatedPCs().Count + "defeatedPC");
        Debug.Log(combatState.GetTurnOrder().Count + "turnorder");
    }
    
    protected bool TryChance(int percentChance) {
        return UnityEngine.Random.Range(0, 100) < percentChance;
    }
}

