using System.Collections.Generic;

public class AbilityCurseOfStrength : Effect
{
    int _attackLevel = 0;
    int _supportLevel = 0;

    public AbilityCurseOfStrength(int AttackLevel = 0, int SupportLevel = 0) {
        _attackLevel = AttackLevel;
        _supportLevel = SupportLevel;
        Name = "Curse of Strength";
        Description = "Apply WEAKNESS to 1 random enemy & STRENGTH to 1 random hero";

        TargetScope = _attackLevel > 0 ? EligibleTargetScopeType.ENEMY : EligibleTargetScopeType.NONE;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/ShieldBash");
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);
        bool CurseLanded = source.GetSpecialAttackRoll(false) != 0;

        if (_attackLevel == 0) {
            if (!CurseLanded) {
                return _e;
            }
        }
        if (_attackLevel > 0) {
            int AttackDamage = source.GetSpecialAttackRoll(false);
            DamageOrder DamageToTarget = new DamageOrder(
                source,
                target,
                AttackDamage,
                this
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
            int Rounds = RandomAlly == source ? 2 : 1;
            Buff strengthenBuff = new BuffStrengthen(source, RandomAlly, Rounds);
            _e.Add(strengthenBuff);
        }

        if (_supportLevel > 1) {
            DamageOrder HealToRandomAlly = new DamageOrder(
                source,
                RandomAlly,
                -30,
                null
            );

            DamageOrder DamageToRandomEnemy = new DamageOrder(
                source,
                target,
                10,
                this
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
