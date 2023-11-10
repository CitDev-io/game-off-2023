using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_HPSpy : MonoBehaviour
{
    TextMeshProUGUI _text;
    [SerializeField]
    public Character combatant;

    void Start() {
        _text = transform.Find("HP").GetComponent<TextMeshProUGUI>();
    }

    void FixedUpdate()
    {
        if (combatant == null) {
            _text.text = "";
        } else {
            _text.text = combatant.currentHealth + "/" + combatant.Config.BaseHP;
        }

    }
}
