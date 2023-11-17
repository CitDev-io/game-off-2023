public class BuffCharmed : Buff
{
    public BuffCharmed(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Charmed";
        Description = "Will basic attack an ally on their next turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
