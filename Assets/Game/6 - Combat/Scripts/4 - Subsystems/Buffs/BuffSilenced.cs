public class BuffSilenced : Buff
{
    public BuffSilenced(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Silenced";
        Description = "Cannot use Special Ability or Ult";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        PortraitArt = "bufficons/silent";
    }
}
