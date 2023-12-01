using System.Collections.Generic;

public class EffectPlan
{
    public EffectPlan(Character caster, Character target, Effect source)
    {
        Caster = caster;
        Target = target;
        Source = source;
    }
    
    public Character Caster;
    public Character Target;
    public Effect Source;

    public List<DamageOrder> DamageOrders = new List<DamageOrder>();

    public List<CalculatedDamage> DamageResults = new List<CalculatedDamage>();

    public List<Buff> BuffOrders = new List<Buff>();
    public List<ReviveOrder> ReviveOrders = new List<ReviveOrder>();
    public List<SummonOrder> SummonOrders = new List<SummonOrder>();
    public List<EffectPlan> EffectResponseOrders = new List<EffectPlan>();
    public List<ScaleOrder> ScaleOrders = new List<ScaleOrder>();

    public EffectPlan Add(ReviveOrder ro) {
        ReviveOrders.Add(ro);
        return this;
    }

    public EffectPlan Add(Buff buff) {
        BuffOrders.Add(buff);
        return this;
    }

    public EffectPlan Add(DamageOrder dmg) {
        DamageOrders.Add(dmg);
        return this;
    }

    public EffectPlan Add(SummonOrder su) {
        SummonOrders.Add(su);
        return this;
    }

    public EffectPlan Add(EffectPlan ea) {
        EffectResponseOrders.Add(ea);
        return this;
    }

    public EffectPlan Add(ScaleOrder so) {
        ScaleOrders.Add(so);
        return this;
    }
    public EffectPlan Add(CalculatedDamage cd) {
        DamageResults.Add(cd);
        return this;
    }
}
