using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActorCharacter : MonoBehaviour
{
    [Header("Temp Exposed Plumbing")]
    [SerializeField]
    public TMP_Text HealthTicker;
    [SerializeField]
    public TMP_Text NameTicker;
    [SerializeField]
    public TMP_Text StaggerTicker; 
    [SerializeField]
    public GameObject TurnIndicator;
    Character _character;
    void Awake() {
        _character = GetComponent<Character>();
    }

    void Start()
    {
        
    }

    // TODO: Point of Contact is probably UI Manager
    void OnMouseDown()
    {
        if (_character.isDead) return;
        Debug.Log(gameObject.name + " was clicked!");
        GameObject.Find("GameManager").GetComponent<CombatReferee>().ReportCombatantClicked(_character);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (HealthTicker != null) {
            HealthTicker.GetComponent<TMP_Text>().text = _character.currentHealth.ToString() + "/" + _character.Config.BaseHP.ToString();
        }
        if (NameTicker != null) {
            NameTicker.text = _character.Config.Name;
        }
        if (StaggerTicker != null) {
            StaggerTicker.text = _character.currentStagger.ToString() + "/" + _character.Config.BaseSP.ToString();
        }

        if (TurnIndicator != null) {
            if (_character.IsCurrentCombatant) {
                TurnIndicator.SetActive(true);
            } else {
                TurnIndicator.SetActive(false);
            }
        }
    }
}
