using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Combatant : MonoBehaviour
{
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

    public void TakeDamage(int Damage) {
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
