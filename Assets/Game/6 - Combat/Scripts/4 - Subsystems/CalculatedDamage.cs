public class CalculatedDamage {
    public CalculatedDamage(Character target, int damageToHealth, int damageToStagger, bool staggerCrackedByThis) {
        Target = target;
        DamageToHealth = damageToHealth;
        DamageToStagger = damageToStagger;
        StaggerCrackedByThis = staggerCrackedByThis;
    }
    public Character Target;
    public int DamageToHealth = 0;
    public int DamageToStagger = 0;
    public bool StaggerCrackedByThis = false;
}