using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }
    public int TurnsRemaining { get; private set; }
    public Character Source { get; protected set; }
    public Character Target { get; protected set; }
    public bool isDebuff = false;
    public int Charges = 1;
    // i suspect this needs to be preflight or cleanup only!
    public CombatPhase AgingPhase { get; protected set; }

    protected Buff(Character src, Character tgt, int duration, int charges = 1)
    {
        Source = src;
        Target = tgt;
        TurnsRemaining = duration;
        Charges = charges;
    }

    public void Tick() // Decrease the duration of the buff
    {
        if (TurnsRemaining > 0)
        {
            TurnsRemaining--;
        }
    }

    public virtual ExecutedAbility ResolvePreflightEffects(){ return null; }
}
