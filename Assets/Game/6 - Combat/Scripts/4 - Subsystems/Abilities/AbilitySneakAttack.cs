using System.Collections.Generic;

public class AbilitySneakAttack : Effect
{
    int _attackLevel = 0;
    int _supportLevel = 0;
    public AbilitySneakAttack(int AttackLevel = 0, int SupportLevel = 0) {
        _attackLevel = AttackLevel;
        _supportLevel = SupportLevel;
        Name = "Poison Strike";
        Description = "Attacks an enemy and applies Poisoned";
        TargetScope = EligibleTargetScopeType.ENEMY;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Abilities/PoisonStrike");   
    }

    public override EffectPlan GetUncommitted(Character source, Character target, List<Character> AllCombatants)
    {
        var _e = new EffectPlan(source, target, this);

        int SneakAttackDamage = source.GetSpecialAttackRoll(false);
        bool SneakAttackLanded = SneakAttackDamage != 0;
        DamageOrder DamageToTarget = new DamageOrder(
            source,
            target,
            SneakAttackDamage,
            this
        );

        _e.Add(DamageToTarget);

        if (!SneakAttackLanded) {
            return _e;
        }

        if (_attackLevel > 0) {
            int FollowupAttacks = 1;
            if (_attackLevel > 1) {
                FollowupAttacks = 2;
            }
            for (var i = 0; i < FollowupAttacks; i++) {
                Character RandomEnemy = CombatantListFilter.RandomByScope(
                    AllCombatants,
                    source,
                    EligibleTargetScopeType.ENEMY
                );
                DamageOrder FollowupAttack1 = new DamageOrder(
                    source,
                    RandomEnemy,
                    (int) (SneakAttackDamage * 0.25f),
                    this
                );
                _e.Add(FollowupAttack1);
            }
        }

        int PoisonDamage = SneakAttackDamage / 2;

        Buff poisonDebuff = new BuffPoisoned(source, target, 1, PoisonDamage);
        _e.Add(poisonDebuff);

        if (_supportLevel > 0) {
            Buff blindBuff = new BuffBlinded(source, target, 1);
            _e.Add(blindBuff);
        }

        if (_supportLevel > 1) {
            Buff smokeBuff = new BuffSmokeBomb(source, target, 2);
            _e.Add(smokeBuff);
        }

        if (_supportLevel > 2) {
            Buff silenceBuff = new BuffSilenced(source, target, 2);
            _e.Add(silenceBuff);
        }

        return _e;
    }    
}
