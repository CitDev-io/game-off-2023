using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityShieldBash : BaseAbilityResolver
{
    int _attackLevel = 0;
    int _supportLevel = 0;
    public AbilityShieldBash(int AttackLevel = 0, int SupportLevel = 0) {
        _attackLevel = AttackLevel;
        _supportLevel = SupportLevel;
        Name = "Shield Bash";
        Description = "Bashes an enemy upside the head";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");   
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);

        int BashDamage = source.GetSpecialAttackRoll(false);
        bool BashLanded = BashDamage != 0;
        if (_attackLevel > 1) {
            BashDamage = (int) (BashDamage * 1.25f);
        }
        CalculatedDamage DamageToTarget = CalculateFinalDamage(
            source,
            target,
            BashDamage
        );

        _e.Add(DamageToTarget);

        if (!BashLanded) {
            return _e;
        }

        if (_attackLevel > 0) {
            List<Character> NearbyAlliesOfVictim = GetNearbyAlliesOfCharacter(target, AllCombatants);

            float AdjacentDamageMultiplier = 0.25f;

            foreach (Character Adjacent in NearbyAlliesOfVictim) {
                CalculatedDamage DamageToAdjacentAlly = CalculateFinalDamage(
                    source,
                    Adjacent,
                    (int) (AdjacentDamageMultiplier * BashDamage)
                );
                _e.Add(DamageToAdjacentAlly);
            }

            if (_attackLevel == 3) {
                foreach(Character adjacent in NearbyAlliesOfVictim) {
                    bool adjacentStunHit = TryChance(33);
                    if (adjacentStunHit) {
                        Buff stunDebuff = new BuffStunned(source, adjacent, 1);
                        _e.Add(stunDebuff);
                    }
                }
            }
        }

        bool StunLanded = TryChance(75);

        if (StunLanded) {
            Buff stunDebuff = new BuffStunned(source, target, 1);
            _e.Add(stunDebuff);
        }

        if (_supportLevel == 1 || _supportLevel == 2) {
            Character RandomAlly = CombatantListFilter.RandomByScope(
                AllCombatants,
                source,
                EligibleTargetScopeType.FRIENDLYORSELF
            );
            if (RandomAlly != null) {
                Buff shieldBuff = new BuffShield(
                    source,
                    RandomAlly,
                    2,
                    BashDamage / 2
                );
                _e.Add(shieldBuff);
            }
        } else if (_supportLevel == 3) {
            List<Character> AllAllies = CombatantListFilter.ByScope(
                AllCombatants,
                source,
                EligibleTargetScopeType.FRIENDLYORSELF
            );

            AllAllies.ForEach(ally => {
                Buff shieldBuff = new BuffShield(
                    source,
                    ally,
                    2,
                    BashDamage / 4
                );
                _e.Add(shieldBuff);
            });
        }

        if (_supportLevel > 1) {
            Buff tauntBuff = new BuffTaunted(
                source,
                target,
                1
            );
            _e.Add(tauntBuff);
        }

        return _e;
    }     
}
