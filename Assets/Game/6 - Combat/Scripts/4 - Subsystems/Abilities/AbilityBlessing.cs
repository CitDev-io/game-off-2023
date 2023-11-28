using System.Collections.Generic;

public class AbilityBlessing : Effect
{
    int _attackLevel = 0;
    int _supportLevel = 0;
    public AbilityBlessing(int AttackLevel = 0, int SupportLevel = 0)
    {
        _attackLevel = AttackLevel;
        _supportLevel = SupportLevel;
        Name = "Blessing";
        Description = "HEAL a target hero";
        TargetScope = _attackLevel > 0 ? EligibleTargetScopeType.ANYALIVE : EligibleTargetScopeType.FRIENDLYORSELF;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/Blessing");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        bool IsAHeal = source.Config.TeamType == target.Config.TeamType;
        int DamageMod = IsAHeal ? -1 : 1;

        int BlessingEffectiveness = source.GetSpecialAttackRoll(IsAHeal) * DamageMod;
        
        if (IsAHeal) {
            if (_supportLevel > 0) {
                target.RemoveRandomDebuff();
            }
            if (_supportLevel > 1) {
                BlessingEffectiveness = (int) (BlessingEffectiveness * 1.5f);
            }
            if (_supportLevel == 3 && source.GenericWaveCounter == 0) {
                Character randomDeadAlly = CombatantListFilter.RandomByScope(
                    AllCombatants,
                    source,
                    EligibleTargetScopeType.DEADFRIENDLY
                );

                if (randomDeadAlly != null) {
                    source.GenericWaveCounter++;

                    int tenthOfPriestHp = (int) (source.currentHealth / 10f);

                    _e.AddToRevivalList(
                        new ReviveOrder(
                            randomDeadAlly,
                            tenthOfPriestHp,
                            this
                        )
                    );
                }
            }
        } else {
            bool attackDidHit = BlessingEffectiveness != 0;
            if (attackDidHit) {
                if (_attackLevel > 1) {
                    Character RandomAlly = CombatantListFilter.RandomByScope(
                        AllCombatants,
                        source,
                        EligibleTargetScopeType.FRIENDLYORSELF
                    );
                    DamageOrder HealingToRandomAlly = new DamageOrder(
                        source,
                        RandomAlly,
                        -BlessingEffectiveness / 2,
                        this
                    );
                    _e.Add(HealingToRandomAlly);
                }

                if (_attackLevel == 3) {
                    Buff evBuff = new BuffElementalVulnerability(
                        source,
                        target,
                        2
                    );
                    _e.Add(evBuff);
                }
            }
        }

        DamageOrder DamageOrHealingToTarget = new DamageOrder(
            source,
            target,
            BlessingEffectiveness,
            this
        );
        _e.Add(DamageOrHealingToTarget);

        return _e;
    }
}
