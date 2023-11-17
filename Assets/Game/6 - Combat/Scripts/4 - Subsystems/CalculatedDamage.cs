public class CalculatedDamage {
    public CalculatedDamage(Character target, int damageToHealth, int damageToStagger, int rawDamage, bool staggerCrackedByThis) {
        Target = target;
        DamageToHealth = damageToHealth;
        DamageToStagger = damageToStagger;
        RawDamage = rawDamage;
        StaggerCrackedByThis = staggerCrackedByThis;
    }
    public Character Target;
    public int DamageToHealth = 0;
    public int DamageToStagger = 0;
    public int RawDamage = 0;
    public bool StaggerCrackedByThis = false;
}