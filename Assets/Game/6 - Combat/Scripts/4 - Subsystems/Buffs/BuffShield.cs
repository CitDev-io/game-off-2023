public class BuffShield : Buff
{
    public BuffShield(Character src, Character tgt, int duration, int charges) : base(src, tgt, duration, charges)
    {
        Name = "Shield";
        Description = "Prevents the next " + charges + " damage taken";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        PortraitArt = "bufficons/shield";
    }
}
