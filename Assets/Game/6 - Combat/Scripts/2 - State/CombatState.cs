using System;
using System.Collections.Generic;
using System.Linq;

public class CombatState
{
    public Effect AbilitySelected;
    public Character TargetSelected;
    public Character CurrentCombatant;
    Queue<Character> TurnOrder = new Queue<Character>();
    public List<Character> FullCombatantList = new List<Character>();
    public int LightPoints = 0;
    public int ShadowPoints = 0;
    public EventProvider _eventProvider;
    public IBattlefieldPositionProvider _bfpProvider;

    public CombatState(EventProvider eventProvider, IBattlefieldPositionProvider bfpProvider) {
        _eventProvider = eventProvider;
        _bfpProvider = bfpProvider;
    }

    public void ClearWaveCounters() {
        foreach (Character combatant in FullCombatantList) {
            combatant.GenericWaveCounter = 0;
        }
    }

    public Queue<Character> GetTurnOrder() {
        return TurnOrder;
    }

    public void ClearTurnOrder() {
        TurnOrder.Clear();
    }

    public void AddCharacterToTurnOrder(Character character) {
        TurnOrder.Enqueue(character);
    }

    void PurgeDeadCombatantsFromQueue() {
        TurnOrder = new Queue<Character>(TurnOrder.ToList().FindAll(combatant => !combatant.isDead));
    }

    public void MoveToNextCombatant() {
        PurgeDeadCombatantsFromQueue();
        if (CurrentCombatant != null) CurrentCombatant.TurnEnd();

        CurrentCombatant = TurnOrder.Dequeue();

        if (!TurnOrder.Contains(CurrentCombatant)){
            TurnOrder.Enqueue(CurrentCombatant);
        }
    }

    public void AddCharacterTurnNext(Character character) {
        Queue<Character> newQueue = new Queue<Character>();
        newQueue.Enqueue(character);
        foreach (Character combatant in GetTurnOrder()) {
            newQueue.Enqueue(combatant);
        }
        TurnOrder = newQueue;
    }

    public void ClearSelections() {
        AbilitySelected = null;
        TargetSelected = null;
    }

    public void ExecuteSelectedAbility() {
        var completedAbility = AbilitySelected
            .GetUncommitted(
                CurrentCombatant,
                TargetSelected,
                FullCombatantList
            );

        ExecuteEffectPlan(completedAbility);

        AdjustScaleByAbilityCast(completedAbility);
        _eventProvider.OnScaleChanged?.Invoke(LightPoints, ShadowPoints);
    }

    public void ResolvePreflightBuffsForCurrentCombatant() {
        List<EffectPlan> abilityEffects = CurrentCombatant
            .Buffs
            .Select(buff => buff.ResolvePreflightEffects())
            .Where(ability => ability != null)
            .ToList();
        ExecuteEffectList(abilityEffects);
    }

    public void SummonUnitForTeam(CharacterConfig config, TeamType team) {
        BattlefieldPosition bfInfo = FindOpenSpotForTeam(team);
        if (bfInfo == null) {
            return; // no space
        }
        Character character = _bfpProvider.InstantiateNewCharacterForConfig(config);

        FullCombatantList.Add(character);

        character.Config = config;
        character.SetPositionInfo(bfInfo);
        character.FirstTimeInitialization();
    }

    public void SummonUnitForTeam(SummonOrder order) {
        CharacterConfig summonedConfig = _bfpProvider.GetConfigForUnitType(order.Unit);

        if (summonedConfig == null) {
            return;
        }

        SummonUnitForTeam(
            summonedConfig,
            order.Team
        );
    }

    public void ReviveCharacter(ReviveOrder ro) {
        if (ro.character.isDead) {
            BattlefieldPosition bp = FindOpenSpotForTeam(ro.character.Config.TeamType);
            if (bp == null) {
                return; // no space
            }
            ro.character.SetPositionInfo(bp);
        }
        if (ro.character.isDead && !TurnOrder.Contains(ro.character)) {
            AddCharacterToTurnOrder(ro.character);
        }
        ro.character.currentHealth = (int)(ro.character.Config.BaseHP * (ro.percentHealth / 100f));
        
        ro.character.RemoveAllBuffs();
        ro.character.currentStagger = ro.character.Config.BaseSP;
        ro.character.isDead = false;
        _eventProvider.OnCharacterRevived?.Invoke(ro.character);
    }


























    void AdjustScaleByAbilityCast(EffectPlan _e) {
        if (_e.Caster.Config.TeamType != TeamType.PLAYER) return;
        if (!_e.Source.IsAbility) return;

        if (_e.Source is AbilityBasicAttack) {
            if (_e.Caster.Config.PowerType == PowerType.LIGHT) {
                if (LightPoints < 2){
                    LightPoints += 1;
                }
            } else {
                if (ShadowPoints < 2) {
                    ShadowPoints += 1;
                }
            }
            return;
        }

        
        if (LightPoints > 1 && ShadowPoints > 1 && _e.Source.IsUltimate) {
            LightPoints -=2;
            ShadowPoints -=2;
            return;
        } 

        if (_e.Caster.Config.PowerType == PowerType.LIGHT) {
            LightPoints -= 1;
            return;
        } else {
            ShadowPoints -= 1;
            return;
        }
    }

    BattlefieldPosition FindOpenSpotForTeam(TeamType team) {
        List<int> TakenSpotIds = FullCombatantList
            .Where(combatant => combatant.Config.TeamType == team && !combatant.isDead && combatant.PositionInfo != null)
            .Select(combatant => combatant.PositionInfo.SpotId)
            .ToList();
        
        return _bfpProvider.GetNextOpenBattlefieldPositionForTeam(TakenSpotIds, team);
    }

    float GetDamageModifierForPowerType(PowerType powerType) {
        if (powerType == PowerType.LIGHT) {
            if (LightPoints - ShadowPoints == 2) {
                return 1.25f;
            }
            if (LightPoints - ShadowPoints == 1) {
                return 1.1f;
            }
            return 1f;
        } else {
            if (ShadowPoints - LightPoints == 2) {
                return 1.25f;
            }
            if (ShadowPoints - LightPoints == 1) {
                return 1.1f;
            }
            return 1f;
        }
    }

    void ModifyDamageOrdersByScale(List<DamageOrder> orders) {
        orders.ForEach(order => {
            float damageModifier = GetDamageModifierForPowerType(order.Attacker.Config.PowerType);

            order.RawDamage = (int) (order.RawDamage * damageModifier);
        });
    }

    void FinalizeEffectPlan(EffectPlan _e) {
        List<DamageOrder> DamageOrders = _e.DamageOrders;

        ModifyDamageOrdersByScale(DamageOrders);
    }

    void ExecuteEffectPlan(EffectPlan executionPlan) {
        FinalizeEffectPlan(executionPlan);

        _eventProvider.OnEffectPlanExecutionStart?.Invoke(executionPlan);
        ResolveScaleOrders(executionPlan);

        ResolveDamageOrders(executionPlan);

        ResolveDamageTriggers(executionPlan);

        ResolveBuffAdditions(executionPlan);
        
        ResolveReviveOrders(executionPlan);

        ResolveSummonOrders(executionPlan);

        ResolveDeathTriggers(executionPlan);

        IdentifyGlobalTriggers(executionPlan);

        _eventProvider.OnEffectPlanExecutionComplete?.Invoke(executionPlan);

        ExecuteEffectList(executionPlan.EffectResponseOrders);
    }

    void IdentifyGlobalTriggers(EffectPlan _e) {
        // Check for Improved Counterattack
        if (_e.Source is AbilityBasicAttack || !_e.Source.IsAbility) return;
        if (_e.Source is AbilityCounterattack) return;

        TeamType opposingTeam = _e.Caster.Config.TeamType == TeamType.PLAYER ? TeamType.CPU : TeamType.PLAYER;

        List<Character> impCounterAttackers = FullCombatantList.FindAll(combatant => combatant.Config.TeamType == opposingTeam && combatant.HasBuff<BuffImprovedCounterAttack>());

        impCounterAttackers.ForEach(ica => {
            EffectPlan ea = new AbilityCounterattack().GetUncommitted(
                ica,
                _e.Caster,
                FullCombatantList
            );
            _e.Add(ea);
        });
    }

    void ResolveSummonOrders(EffectPlan _e) {
        _e.SummonOrders.ForEach(su => SummonUnitForTeam(su));
    }

    void ResolveReviveOrders(EffectPlan _e) {
        _e.ReviveOrders.ForEach(ro => ReviveCharacter(ro));
    }

    void ResolveScaleOrders(EffectPlan _e) {
        _e.ScaleOrders.ForEach(so => {
            LightPoints = Math.Clamp(so.LightPoints + LightPoints, 0, 2);
            ShadowPoints = Math.Clamp(so.ShadowPoints + ShadowPoints, 0, 2);
            _eventProvider.OnScaleChanged?.Invoke(LightPoints, ShadowPoints);
        });
    }

    void ResolveBuffAdditions(EffectPlan _e) {
        _e.BuffOrders.ForEach(buff => {
            buff.Target.AddBuff(buff);
            _eventProvider.OnBuffAdded?.Invoke(buff);
        });
    }

    void ResolveDamageOrders(EffectPlan _e) {
        DamageResolver dr = new DamageResolver();
        _e.DamageOrders.ForEach(damage => {
            CalculatedDamage dmgResult = dr.ResolveOrder(damage);

            _e.Add(dmgResult);
            _eventProvider.OnDamageResolved?.Invoke(dmgResult);
        });
    }

    void ResolveDamageTriggers(EffectPlan _e) {
        var damageResults = _e.DamageResults;

        damageResults.ForEach(damage => {
            bool TookNonZeroDamage = damage.DamageToHealth > 0 || damage.DamageToStagger > 0;

            if (damage.Target.HasBuff<BuffCounterattack>() && damage.Source is not AbilityCounterattack && TookNonZeroDamage) {
                EffectPlan ea = new AbilityCounterattack().GetUncommitted(
                    damage.Attacker,
                    damage.Target,
                    FullCombatantList
                );
                _e.Add(ea);
            }

            if (damage.Target.HasBuff<BuffHeatwave>() && TookNonZeroDamage && damage.Source.IsAbility) {
            BuffHeatwave heatwave = (BuffHeatwave) damage.Target.GetBuff<BuffHeatwave>();
            heatwave.Charges = Math.Clamp(heatwave.Charges - 1, 0, 99);
        }
        });
    }

    void ResolveDeathTriggers(EffectPlan _e) {
        foreach(CalculatedDamage dmg in _e.DamageResults) {
            if (!dmg.Target.isDead) continue;
            if (dmg.Target.HasBuff<BuffVolcanicBowelSyndrome>()) {
                dmg.Target.RemoveBuff<BuffVolcanicBowelSyndrome>();
                AbilityVolcanicBowelBlast bb = new AbilityVolcanicBowelBlast();
                EffectPlan blastResponse = bb.GetUncommitted(
                    dmg.Target,
                    dmg.Target,
                    FullCombatantList
                );
                _e.Add(blastResponse);
            }

            // rez effects should go last since it clears buffs
            if (dmg.Target.HasBuff<BuffPyroPeakboo>()) {
                dmg.Target.RemoveBuff<BuffPyroPeakboo>();
                AbilityPyroPeakaboo pp = new AbilityPyroPeakaboo();
                EffectPlan peakResponse = pp.GetUncommitted(
                    dmg.Target,
                    dmg.Target,
                    FullCombatantList
                );
                _e.Add(peakResponse);
            }
            
        }
    }

    void ExecuteEffectList(List<EffectPlan> effectList) {
        foreach (EffectPlan ability in effectList) {
            ExecuteEffectPlan(ability);
        }
    }











    public List<AbilityCategory> GetAvailableAbilitiesForCurrentCombatant() {
        return CurrentCombatant.GetAvailableAbilities(LightPoints, ShadowPoints);
    }
    public List<Character> GetEligibleTargetsForSelectedAbility() {
        return CombatantListFilter.ByScope(
            FullCombatantList,
            CurrentCombatant,
            AbilitySelected.TargetScope
        );
    }

    public List<Character> GetAlivePCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && !combatant.isDead);
    }

    public List<Character> GetAliveCPUs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && !combatant.isDead);
    }

    public List<Character> GetDefeatedPCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER && combatant.isDead);
    }

    public List<Character> GetDefeatedCPUs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.CPU && combatant.isDead);
    }

    public List<Character> GetAllPCs() {
        return FullCombatantList.FindAll(combatant => combatant.Config.TeamType == TeamType.PLAYER);
    }
}
