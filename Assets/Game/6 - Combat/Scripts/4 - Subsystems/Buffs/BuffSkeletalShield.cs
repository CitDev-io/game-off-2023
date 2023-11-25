public class BuffSkeletalShield : Buff
{
    public BuffSkeletalShield(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Skeletal Shield";
        Description = "Resistant to both Light and Shadow";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = false;
        PortraitArt = "bufficons/skeletal shield";
    }
}
