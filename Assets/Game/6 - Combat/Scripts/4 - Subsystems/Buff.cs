using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public Sprite PortraitArt { get; protected set; }
    public int TurnsRemaining { get; private set; }
    public Character Source { get; protected set; }
    // i suspect this needs to be preflight or cleanup only!
    public CombatPhase AgingPhase { get; protected set; }

    protected Buff(Character src, int duration)
    {
        Source = src;
        TurnsRemaining = duration;
    }

    public void Tick() // Decrease the duration of the buff
    {
        if (TurnsRemaining > 0)
        {
            TurnsRemaining--;
        }
    }
}
