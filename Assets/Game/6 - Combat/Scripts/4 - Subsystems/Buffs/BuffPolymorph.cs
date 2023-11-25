public class BuffPolymorph : Buff
{
    public BuffPolymorph(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Polymorph";
        Description = "Enemy is transformed into a harmless sheep";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/polymorph";
    }
}