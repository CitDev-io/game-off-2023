using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCharacter : Combatant
{
    [SerializeField]
    public TMP_Text NameTicker;

    internal override void MoreFixedUpdate(){
        if (NameTicker != null) {
            NameTicker.text = gameObject.name;
        }
    }
    internal override int CalculateFinalDamage(PowerType sourcePowerType, Combatant source, int rawDamage, int unmitigatedDamage) {
        return unmitigatedDamage;
    }
}
