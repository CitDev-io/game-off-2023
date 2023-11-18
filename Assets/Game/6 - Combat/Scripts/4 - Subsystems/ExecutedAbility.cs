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

    public ExecutedAbility Commit() {
        foreach (CalculatedDamage damage in AppliedHealthChanges) {
            Target.TakeDamage(damage.DamageToHealth);
            Target.TakeStagger(damage.DamageToStagger);
        }

        foreach (Buff buff in AppliedBuffs) {
            buff.Target.AddBuff(buff);
        }

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
}
