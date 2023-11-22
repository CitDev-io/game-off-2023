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


    public ExecutedAbility Commit() {
        foreach (CalculatedDamage damage in AppliedHealthChanges) {
            damage.Target.TakeDamage(damage.DamageToHealth);
            damage.Target.TakeStagger(damage.DamageToStagger);
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
}
