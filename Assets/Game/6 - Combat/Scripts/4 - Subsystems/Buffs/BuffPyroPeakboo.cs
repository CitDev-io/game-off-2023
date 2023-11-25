public class BuffPyroPeakboo : Buff
{
    public BuffPyroPeakboo(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Pyro Peakboo";
        Description = "Will revive when killed";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        PortraitArt = "bufficons/peakaboo";
    }
}