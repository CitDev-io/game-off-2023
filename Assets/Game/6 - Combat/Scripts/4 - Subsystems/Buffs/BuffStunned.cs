public class BuffStunned : Buff
{
    public BuffStunned(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Stunned";
        Description = "Cannot take any actions on their turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
