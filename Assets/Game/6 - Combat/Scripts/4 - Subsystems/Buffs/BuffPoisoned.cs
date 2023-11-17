public class BuffPoisoned : Buff
{
    public AbilityFlatDotDamage DotAbility;
    public BuffPoisoned(Character src, Character tgt, int duration, int damage) : base(src, tgt, duration)
    {
        Name = "Poisoned";
        Description = "Takes damage at the start of their turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        DotAbility = new AbilityFlatDotDamage(damage, "Poison Damage", "Enemy is Poisoned");
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/Poisoned");
    }

    public override ExecutedAbility ResolvePreflightEffects()
    {
        return DotAbility.Resolve(Source, Target);
    }
}