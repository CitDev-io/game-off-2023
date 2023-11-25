public class BuffSmokeBomb : Buff
{
    public BuffSmokeBomb(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Smoke Bomb";
        Description = "Reduced chance to choose to attack the source of Smoke Bomb";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/smokebomb";
    }
}
