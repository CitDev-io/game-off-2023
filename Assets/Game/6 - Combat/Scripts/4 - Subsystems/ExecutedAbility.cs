using System.Collections.Generic;

public class ExecutedAbility
{
    public ExecutedAbility(Character source, Character target, BaseAbilityResolver ability)
    {
        Source = source;
        Target = target;
        Ability = ability;
    }
    
    public Character Source;
    public Character Target;
    public BaseAbilityResolver Ability;

    public List<CalculatedDamage> AppliedHealthChanges = new List<CalculatedDamage>();

    public List<Buff> AppliedBuffs = new List<Buff>();
    public List<ReviveOrder> CharactersReviving = new List<ReviveOrder>();
    public List<SummonOrder> SummonedUnits = new List<SummonOrder>();
    public List<ExecutedAbility> UncommittedResponses = new List<ExecutedAbility>();
    public List<ScaleOrder> ScaleChange = new List<ScaleOrder>();

    public ExecutedAbility Commit() {
        foreach (CalculatedDamage damage in AppliedHealthChanges) {
            damage.Target.TakeDamage(damage.DamageToHealth);
            damage.Target.TakeStagger(damage.DamageToStagger);

            bool TookNonZeroDamage = damage.DamageToHealth > 0 || damage.DamageToStagger > 0;

            if (damage.Target.HasBuff<BuffCounterattack>() && Ability is not AbilityCounterattack && TookNonZeroDamage) {
                ExecutedAbility ea = new ExecutedAbility(damage.Target, Source, new AbilityCounterattack());
                this.Add(ea);
            }
        }

        foreach (Buff buff in AppliedBuffs) {
            buff.Target.AddBuff(buff);
        }

        return this;
    }

    public ExecutedAbility AddToRevivalList(ReviveOrder ro) {
        CharactersReviving.Add(ro);
        return this;
    }

    public ExecutedAbility Add(Buff buff) {
        AppliedBuffs.Add(buff);
        return this;
    }

    public ExecutedAbility Add(CalculatedDamage dmg) {
        AppliedHealthChanges.Add(dmg);
        return this;
    }

    public ExecutedAbility Add(SummonOrder su) {
        SummonedUnits.Add(su);
        return this;
    }

    public ExecutedAbility Add(ExecutedAbility ea) {
        UncommittedResponses.Add(ea);
        return this;
    }

    public ExecutedAbility Add(ScaleOrder so) {
        ScaleChange.Add(so);
        return this;
    }
}
