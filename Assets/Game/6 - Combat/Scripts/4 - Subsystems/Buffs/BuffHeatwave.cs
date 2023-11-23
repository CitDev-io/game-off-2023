using System.Collections.Generic;

public class BuffHeatwave : Buff
{
    EffectHeavyweightHeatwave hh;
    List<Character> AllEnemies;
    int RoundsUntilBlast = 2;

    public BuffHeatwave(Character src, Character tgt, int duration, int charges, List<Character> allEnemies) : base(src, tgt, duration, charges)
    {
        Name = "Heatwave";
        Description = "Starts with 10 charges. After two turns, blasts all enemies with fire. Each time this unit takes damage, it loses 1 charge. Each charge removed reduces this damage by 10%.";
        AgingPhase = CombatPhase.CHARACTERTURN_CLEANUP;
        isDebuff = false;
        hh = new EffectHeavyweightHeatwave();
        AllEnemies = allEnemies;
        // PortraitArt = Resources.Load<Sprite>("Sprites/Icons/stunned");
    }

    public override EffectPlan ResolvePreflightEffects()
    {
        RoundsUntilBlast--;

        if (RoundsUntilBlast == 0) {
            return hh.GetUncommitted(Source, Target, AllEnemies);
        }
        return null;
    }
}
