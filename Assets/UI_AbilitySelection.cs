
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_AbilitySelection : MonoBehaviour
{

    [SerializeField]
    public GameObject BasicAttackBox;
    [SerializeField]
    public GameObject SpecialAttackBox;
    [SerializeField]
    public GameObject UltimateBox;
   
   public void SetAvailableAbilities(List<AbilityCategory> categories) {
        BasicAttackBox.SetActive(
            categories.Contains(AbilityCategory.BASICATTACK)
        );
        SpecialAttackBox.SetActive(
            categories.Contains(AbilityCategory.SPECIALATTACK)
        );
        UltimateBox.SetActive(
            categories.Contains(AbilityCategory.ULTIMATE)
        );
   }

   public AbilityCategory CurrentlySelected() {
        if (BasicAttackBox.GetComponent<Image>().enabled) {
            return AbilityCategory.BASICATTACK;
        } else if (SpecialAttackBox.GetComponent<Image>().enabled) {
            return AbilityCategory.SPECIALATTACK;
        } else {
            return AbilityCategory.ULTIMATE;
        }
   }

   int ActiveOptionCount() {
       int activeOptions = 0;
       if (BasicAttackBox.activeSelf) {
           activeOptions++;
       }
       if (SpecialAttackBox.activeSelf) {
           activeOptions++;
       }
       if (UltimateBox.activeSelf) {
           activeOptions++;
       }
       return activeOptions;
   }

    public void ToggleUp() {
        int OPTIONCOUNT = ActiveOptionCount();
        int currentlySelected = (int)CurrentlySelected();
        currentlySelected = (currentlySelected - 1 + OPTIONCOUNT) % OPTIONCOUNT;
        ToggleToSelectedAbility(currentlySelected);
    }

    public void ToggleDown() {
        int OPTIONCOUNT = ActiveOptionCount();
        int currentlySelected = (int)CurrentlySelected();
        currentlySelected = (currentlySelected + 1) % OPTIONCOUNT;
        ToggleToSelectedAbility(currentlySelected);
    }

    public void ToggleToSelectedAbility(int abilityIndex) {
        BasicAttackBox.GetComponent<Image>().enabled = false;
        SpecialAttackBox.GetComponent<Image>().enabled = false;
        UltimateBox.GetComponent<Image>().enabled = false;
        
        GameObject SelectedBox = null;
        switch ((AbilityCategory)abilityIndex) {
            case AbilityCategory.BASICATTACK:
                SelectedBox = BasicAttackBox;
                break;
            case AbilityCategory.SPECIALATTACK:
                SelectedBox = SpecialAttackBox;
                break;
            case AbilityCategory.ULTIMATE:
                SelectedBox = UltimateBox;
                break;
        }
        
        SelectedBox.GetComponent<Image>().enabled = true;
    }

    public void SetSpecialAbilityName(string name) {
        SpecialAttackBox.transform.Find("Nameplate").GetComponent<TextMeshProUGUI>().text = name;
    }

    public void SetUltimateAbilityName(string name) {
        UltimateBox.transform.Find("Nameplate").GetComponent<TextMeshProUGUI>().text = name;
    }

    public void ToggleAndReportAbility(int abilityIndex) {
        AbilityCategory category = (AbilityCategory)abilityIndex;
        ToggleToSelectedAbility((int) category);
        GameObject.Find("GameManager").GetComponent<UIManager>().AbilitySelected(category);
    }
}
