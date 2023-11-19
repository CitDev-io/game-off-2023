public class BuffTaunted : Buff
{
    public BuffTaunted(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Taunted";
        Description = "Will basic attack the source of Taunt on their next turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/taunted");
    }
}
