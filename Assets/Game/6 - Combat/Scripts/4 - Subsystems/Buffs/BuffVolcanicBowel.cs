public class BuffVolcanicBowelSyndrome : Buff
{
    public BuffVolcanicBowelSyndrome(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Volcanic Bowel Syndrome";
        Description = "When this creature is defeated, it explodes, dealing damage to nearby allies";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/volcanic";
    }
}