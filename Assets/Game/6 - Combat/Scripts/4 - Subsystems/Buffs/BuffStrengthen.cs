public class BuffStrengthen : Buff
{
    public BuffStrengthen(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Strengthen";
        Description = "Damage and Healing Done is increased by 50%";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
