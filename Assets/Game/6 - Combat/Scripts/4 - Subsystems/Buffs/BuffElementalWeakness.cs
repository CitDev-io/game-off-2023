public class BuffElementalWeakness : Buff
{
    public BuffElementalWeakness(Character src, Character tgt, int duration) : base(src, tgt, duration)
    {
        Name = "Elemental Weakness";
        Description = "Creature is not resistant to any elements";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = true;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/elementalweakness");
    }
}
