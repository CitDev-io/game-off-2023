public class BuffStunned : Buff
{
    public BuffStunned(Character src, int duration) : base(src, duration)
    {
        Name = "Stunned";
        Description = "Cannot take any actions on their turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
