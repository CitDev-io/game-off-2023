public class BuffGreaterCharmed : Buff
{
    public BuffGreaterCharmed(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Greater Charmed";
        Description = "Will target the wrong team with Basic Attack or Special Ability on their next turn";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/greatercharmed");
    }
}
