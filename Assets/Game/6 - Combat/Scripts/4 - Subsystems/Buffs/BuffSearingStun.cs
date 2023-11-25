public class BuffSearingStun : Buff
{
    public BuffSearingStun(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Searing Stun";
        Description = "Turned to stone, cannot take any actions on their turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/stun";
    }
}
