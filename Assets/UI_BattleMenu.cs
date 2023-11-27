
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_BattleMenu : MonoBehaviour
{
   
    public TextMeshPro BasicAttackText;
    public TextMeshPro SpecialAttackText;
    public TextMeshPro UltimateText;
    public GameObject Cursor1;
    public GameObject Cursor2;
    public GameObject Cursor3;

    [SerializeField]
    AbilityCategory Selected = AbilityCategory.BASICATTACK;
   
   public void SetAvailableAbilities(List<AbilityCategory> categories) {
        BasicAttackText.gameObject.SetActive(
            categories.Contains(AbilityCategory.BASICATTACK)
        );
        SpecialAttackText.gameObject.SetActive(
            categories.Contains(AbilityCategory.SPECIALATTACK)
        );
        UltimateText.gameObject.SetActive(
            categories.Contains(AbilityCategory.ULTIMATE)
        );
   }

   public AbilityCategory CurrentlySelected() {
        return Selected;
   }

   int ActiveOptionCount() {
       int activeOptions = 0;
       if (BasicAttackText.gameObject.activeSelf) {
           activeOptions++;
       }
       if (SpecialAttackText.gameObject.activeSelf) {
           activeOptions++;
       }
       if (UltimateText.gameObject.activeSelf) {
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
        Cursor1.SetActive(false);
        Cursor2.SetActive(false);
        Cursor3.SetActive(false);
        
        GameObject SelectedOption = null;
        switch ((AbilityCategory)abilityIndex) {
            case AbilityCategory.BASICATTACK:
                SelectedOption = Cursor1;
                break;
            case AbilityCategory.SPECIALATTACK:
                SelectedOption = Cursor2;
                break;
            case AbilityCategory.ULTIMATE:
                SelectedOption = Cursor3;
                break;
        }
        
        Selected = (AbilityCategory)abilityIndex;
        SelectedOption.SetActive(true);
    }

    public void SetSpecialAbilityName(string name) {
        SpecialAttackText.text = name;
    }

    public void SetUltimateAbilityName(string name) {
        UltimateText.text = name;
    }

    public void ToggleAndReportAbility(int abilityIndex) {
        AbilityCategory category = (AbilityCategory)abilityIndex;
        ToggleToSelectedAbility((int) category);
        GameObject.Find("GameManager").GetComponent<UIManager>().AbilitySelected(category);
    }
}
