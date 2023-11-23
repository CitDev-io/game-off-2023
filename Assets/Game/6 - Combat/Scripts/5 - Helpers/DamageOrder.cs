public class DamageOrder {
    public Character Attacker;
    public Character Victim;
    public int RawDamage;
    public Effect Source;

    public DamageOrder(Character attacker, Character victim, int rawDamage, Effect source) {
        Attacker = attacker;
        Victim = victim;
        RawDamage = rawDamage;
        Source = source;
    }
}
