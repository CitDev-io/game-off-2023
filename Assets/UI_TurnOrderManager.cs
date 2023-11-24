using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TurnOrderManager : MonoBehaviour
{
    // public TextMeshProUGUI Flavor1;
    // public TextMeshProUGUI Flavor2;
    public TextMeshProUGUI Flavor3;

    public List<UI_PortraitController> Portraits;
    public void UpdateTurnOrder(Character CurrentCharacter, List<Character> QueuedCharacters)
    {
        if (CurrentCharacter == null) {
            return;
        }

        Portraits[0].SetCharacter(CurrentCharacter);
        // Flavor1.text = CurrentCharacter.Config.Name + "'s Turn";
        // Flavor2.text = CurrentCharacter.Config.Name + "'s Battlecry goes here";
        Flavor3.text = CurrentCharacter.currentHealth + "/" + CurrentCharacter.Config.BaseHP + " HP";


        for (int i = 1; i < Portraits.Count; i++)
        {
            if (i - 1 >= QueuedCharacters.Count)
            {
                Portraits[i].SetCharacter(null);
                Portraits[i].gameObject.SetActive(false);
                continue;
            }
            if (QueuedCharacters[i - 1] == CurrentCharacter && i == QueuedCharacters.Count)
            {
                Portraits[i].SetCharacter(null);
                Portraits[i].gameObject.SetActive(false);
                continue;
            }
            Portraits[i].gameObject.SetActive(true);
            Portraits[i].SetCharacter(QueuedCharacters[i - 1]);
        }
    }
}
