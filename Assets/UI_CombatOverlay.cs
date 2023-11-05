using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_CombatOverlay : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI WaveText;
    CombatReferee _combatReferee;

    void Start()
    {
        StageText = transform.Find("Stage Text").GetComponent<TextMeshProUGUI>();
        WaveText = transform.Find("Wave Text").GetComponent<TextMeshProUGUI>();
        _combatReferee = GameObject.Find("Combat Field").GetComponent<CombatReferee>();
    }

    void FixedUpdate()
    {
        StageText.text = "Stage: " + _combatReferee.StageNumber.ToString();
        WaveText.text = "Wave: " + _combatReferee.WaveNumber.ToString();
    }
}
