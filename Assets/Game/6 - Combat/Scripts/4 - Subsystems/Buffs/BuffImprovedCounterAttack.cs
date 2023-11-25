public class BuffImprovedCounterAttack : Buff
{
    public BuffImprovedCounterAttack(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Improved Counterattack";
        Description = "When any enemy uses a Special Attack or Ultimate ability, this unit will attack that enemy";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = false;
        PortraitArt = "bufficons/improved counter attack";
    }
}
