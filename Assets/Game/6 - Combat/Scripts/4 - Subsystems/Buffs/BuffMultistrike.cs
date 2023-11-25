using System.Collections.Generic;

public class BuffMultistrike : Buff
{
    public BuffMultistrike(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Multistrike";
        Description = "Will take an additional turn after their next turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        PortraitArt = "bufficons/multistrike";
    }
}
