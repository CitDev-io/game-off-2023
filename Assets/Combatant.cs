using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Combatant : MonoBehaviour
{
    [SerializeField]
    public int BaseMitigation = 0;
    [SerializeField]
    public PowerType powerType = PowerType.LIGHT;
    [SerializeField]
    public int maximumHealth = 1;
    [SerializeField]
    public int currentHealth = 1;
    [SerializeField]
    public TMP_Text HealthTicker;
    bool IsCurrentCombatant = false;
    [SerializeField]
    public GameObject TurnIndicator;
    [SerializeField]
    public int MinDamage = 1;
    public int MaxDamage = 1;
    public bool isDead = false;

    void OnMouseDown()
    {
        if (isDead) return;
        Debug.Log(gameObject.name + " was clicked!");
        GameObject.Find("Combat Field").GetComponent<CombatReferee>().ReportCombatantClicked(this);
    }

    void FixedUpdate()
    {
        if (HealthTicker != null) {
            HealthTicker.GetComponent<TMP_Text>().text = currentHealth.ToString() + "/" + maximumHealth.ToString();
        }
        MoreFixedUpdate();

        if (TurnIndicator != null) {
            if (IsCurrentCombatant) {
                TurnIndicator.SetActive(true);
            } else {
                TurnIndicator.SetActive(false);
            }
        }
    }

    public void TurnStart() {
        IsCurrentCombatant = true;
    }

    public void TurnEnd() {
        IsCurrentCombatant = false;
    }

    public int GetRandomDamageRoll() {
        return Random.Range(MinDamage, MaxDamage);
    }

    public int HandleIncomingAttack(PowerType sourcePowerType, Combatant source) {
        int rawDamage = source.GetRandomDamageRoll();

        bool resistantToPowerType = sourcePowerType == powerType;
        int PowerTypeResistance = resistantToPowerType ? 10 : 0;
        int mitigationPower = BaseMitigation + PowerTypeResistance;
        int mitigatedDamage = (int) (rawDamage * (mitigationPower / 100f));

        int unmitigatedDamage = Mathf.Clamp(
            rawDamage - mitigatedDamage,
            0,
            rawDamage
        );

        int FinalDamage = CalculateFinalDamage(sourcePowerType, source, rawDamage, unmitigatedDamage);

        TakeDamage(FinalDamage);
        return FinalDamage;
    }

    internal abstract void MoreFixedUpdate();
    internal abstract int CalculateFinalDamage(PowerType sourcePowerType, Combatant source, int rawDamage, int unmitigatedDamage);


    void TakeDamage(int Damage) {
        currentHealth -= Damage;
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
    }

    void Die() {
        isDead = true;
    }
}
