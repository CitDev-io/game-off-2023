public class BuffPoisoned : Buff
{
    public EffectFlatDotDamage DotAbility;
    public BuffPoisoned(Character src, Character tgt, int duration, int damage) : base(src, tgt, duration)
    {
        Name = "Poisoned";
        Description = "Takes damage at the start of their turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        DotAbility = new EffectFlatDotDamage(damage, "Poison Damage", "Enemy is Poisoned");
        isDebuff = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/Poisoned");
    }

    public override EffectPlan ResolvePreflightEffects()
    {
        return DotAbility.GetUncommitted(Source, Target, null);
    }
}
