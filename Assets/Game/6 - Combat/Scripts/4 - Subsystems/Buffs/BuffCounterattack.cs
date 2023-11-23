public class BuffCounterattack : Buff
{
    public BuffCounterattack(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Counterattack";
        Description = "When taking damage, this unit will attack the source back";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = false;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
