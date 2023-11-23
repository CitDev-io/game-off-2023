public class CalculatedDamage {
    public CalculatedDamage(Character attacker, Character target, int damageToHealth, int damageToStagger, int rawDamage, bool staggerCrackedByThis, Effect source) {
        Target = target;
        DamageToHealth = damageToHealth;
        DamageToStagger = damageToStagger;
        RawDamage = rawDamage;
        StaggerCrackedByThis = staggerCrackedByThis;
        Source = source;
        Attacker = attacker;
    }
    public Effect Source;
    public Character Attacker;
    public Character Target;
    public int DamageToHealth = 0;
    public int DamageToStagger = 0;
    public int RawDamage = 0;
    public bool StaggerCrackedByThis = false;
}