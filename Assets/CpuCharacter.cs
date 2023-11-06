using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CpuCharacter : Combatant
{
    [SerializeField]
    public TMP_Text StaggerTicker; 
    [SerializeField]
    public int maximumStagger = 0;
    [SerializeField]
    public int currentStagger = 0;

    internal override void MoreFixedUpdate(){
        if (StaggerTicker != null) {
            StaggerTicker.text = currentStagger.ToString() + "/" + maximumStagger.ToString();
        }
    }

    public void RestoreStagger() {
        currentStagger = maximumStagger;
    }

    internal override int CalculateFinalDamage(PowerType sourcePowerType, Combatant source, int rawDamage, int unmitigatedDamage) {
        bool CharacterIsCracked = currentStagger == 0;

        if (CharacterIsCracked) {
            return unmitigatedDamage;
        }

        bool SourceAffectsStagger = sourcePowerType != powerType;

        int DamageDealtToStagger = SourceAffectsStagger ? rawDamage : 0;

        currentStagger = Mathf.Clamp(
            currentStagger - DamageDealtToStagger,
            0,
            currentStagger
        );

        int DamageToHealth = (int) (unmitigatedDamage / 2f);

        return DamageToHealth;
    }
}
