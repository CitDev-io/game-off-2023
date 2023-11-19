using System.Collections.Generic;

public class AbilityCurseOfStrength : BaseAbilityResolver
{
    int _attackLevel = 0;
    int _supportLevel = 0;

    public AbilityCurseOfStrength(int AttackLevel = 0, int SupportLevel = 0) {
        _attackLevel = AttackLevel;
        _supportLevel = SupportLevel;
        Name = "Curse of Strength";
        Description = "Curses an Enemy and Empowers an Ally";

        TargetScope = _attackLevel > 0 ? EligibleTargetScopeType.ENEMY : EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");
    }

    public override ExecutedAbility GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new ExecutedAbility(source, target, this);
        bool CurseLanded = source.GetSpecialAttackRoll(false) != 0;

        if (_attackLevel == 0) {
            if (!CurseLanded) {
                return _e;
            }
        }
        if (_attackLevel > 0) {
            int AttackDamage = source.GetSpecialAttackRoll(false);
            CalculatedDamage DamageToTarget = CalculateFinalDamage(
                source,
                target,
                AttackDamage
            );
            CurseLanded = AttackDamage != 0;

            _e.Add(DamageToTarget);
        }

        if (!CurseLanded) {
            return _e;
        }

        if (_attackLevel == 2) {
            Buff charmDebuff = new BuffCharmed(source, target, 1);
            _e.Add(charmDebuff);
        } else if (_attackLevel == 3) {
            Buff charmDebuff = new BuffGreaterCharmed(source, target, 1);
            _e.Add(charmDebuff);
        }

        if (_supportLevel > 0) {
            bool ElementalWeaknessLands = TryChance(25);
            if (ElementalWeaknessLands) {
                Buff ewDebuff = new BuffElementalWeakness(source, target, 1);
                _e.Add(ewDebuff);
            }
        }

        Character RandomEnemy = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.ENEMY
        );
        if (RandomEnemy != null) {
            Buff weaknessDebuff = new BuffWeakness(source, RandomEnemy, 1);
            _e.Add(weaknessDebuff);
        }

        Character RandomAlly = CombatantListFilter.RandomByScope(
            AllCombatants,
            source,
            EligibleTargetScopeType.FRIENDLYORSELF
        );
        if (RandomAlly != null) {
            Buff strengthenBuff = new BuffStrengthen(source, RandomAlly, 1);
            _e.Add(strengthenBuff);
        }

        if (_supportLevel > 1) {
            CalculatedDamage HealToRandomAlly = CalculateFinalDamage(
                source,
                RandomAlly,
                -30
            );

            CalculatedDamage DamageToRandomEnemy = CalculateFinalDamage(
                source,
                target,
                10
            );

            _e.Add(HealToRandomAlly);
            _e.Add(DamageToRandomEnemy);
        }

        if (_supportLevel > 2) {
            Buff msBuff = new BuffMultistrike(source, RandomAlly, 1);

            _e.Add(msBuff);
        }

        return _e;
    } 
}
