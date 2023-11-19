public class BuffWeakness : Buff
{
    public BuffWeakness(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Weakness";
        Description = "Damage and Healing Done reduced by 50%";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }
}
