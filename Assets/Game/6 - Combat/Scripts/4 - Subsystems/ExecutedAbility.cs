using System.Collections.Generic;

public class ExecutedAbility
{
    public ExecutedAbility(Character source, Character target, AttackType attackType, List<AppliedBuff> appliedBuffs, List<CalculatedDamage> appliedHealthChanges)
    {
        Source = source;
        Target = target;
        AttackType = attackType;
        AppliedBuffs = appliedBuffs;
        AppliedHealthChanges = appliedHealthChanges;
    }
    
    public Character Source;
    public Character Target;
    public AttackType AttackType = AttackType.NONE;

    public List<CalculatedDamage> AppliedHealthChanges = new List<CalculatedDamage>();

    public List<AppliedBuff> AppliedBuffs = new List<AppliedBuff>();

}

public class AppliedBuff {
    public AppliedBuff(Character affectedCharacter, Buff buff)
    {
        AffectedCharacter = affectedCharacter;
        Buff = buff;
    }
    public Character AffectedCharacter;
    public Buff Buff;
}
