public class BuffBlinded : Buff
{
    public BuffBlinded(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Blinded";
        Description = "Reduced chance to hit";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/blind";
    }
}
