using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_CombatOverlay : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI ScaleCountText;
    CombatReferee _combatReferee;

    void Start()
    {
        StageText = transform.Find("Stage Text").GetComponent<TextMeshProUGUI>();
        WaveText = transform.Find("Wave Text").GetComponent<TextMeshProUGUI>();
        ScaleCountText = transform.Find("ScaleCount").GetComponent<TextMeshProUGUI>();
        _combatReferee = GameObject.Find("Combat Field").GetComponent<CombatReferee>();
    }

    void FixedUpdate()
    {
        StageText.text = "Stage: " + _combatReferee.gameState.StageNumber.ToString();
        WaveText.text = "Wave: " + _combatReferee.gameState.WaveNumber.ToString();
        ScaleCountText.text = "x" + _combatReferee.gameState.ScalesOwned.ToString();
    }
}
