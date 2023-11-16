
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_TargetSelection : MonoBehaviour
{

    List<Character> EligibleTargets;
    public Character CurrentSelection;
    public List<GameObject> TargetBoxes;

    public void SetEligibleTargets(List<Character> targets) {
        EligibleTargets = targets;
        foreach(GameObject targetBox in TargetBoxes) {
            int indexOfCurrent = TargetBoxes.IndexOf(targetBox);
            if (EligibleTargets.Count > indexOfCurrent) {
                targetBox.GetComponent<Image>().enabled = true;
                targetBox.transform.Find("Nameplate").GetComponent<TextMeshProUGUI>().text = EligibleTargets[indexOfCurrent].Config.Name;
            } else {
                targetBox.GetComponent<Image>().enabled = false;
                targetBox.transform.Find("Nameplate").GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }
    public void SelectEligibleTarget(int index) {
        if (EligibleTargets.Count > index) {
            ToggleEligibleTarget(index);
            GameObject.Find("GameManager").GetComponent<UIManager>().TargetSelected(CurrentSelection);
            foreach(GameObject targetBox in TargetBoxes) {
                int indexOfCurrent = TargetBoxes.IndexOf(targetBox);
                targetBox.GetComponent<Image>().enabled = indexOfCurrent == index;
            }
        }
    }
   
   public void ToggleEligibleTarget(int index) {
        foreach(GameObject targetBox in TargetBoxes) {
            int indexOfCurrent = TargetBoxes.IndexOf(targetBox);
            targetBox.GetComponent<Image>().enabled = indexOfCurrent == index;
        }
        if (EligibleTargets.Count > index) {
            CurrentSelection = EligibleTargets[index];
        }
   }

    public void ToggleDown() {
        int indexToToggleTo = EligibleTargets.IndexOf(CurrentSelection);
        if (indexToToggleTo == EligibleTargets.Count - 1) {
            indexToToggleTo = 0;
        } else {
            indexToToggleTo++;
        }
        ToggleEligibleTarget(indexToToggleTo);
    }

    public void ToggleUp() {
        int indexToToggleTo = EligibleTargets.IndexOf(CurrentSelection);
        if (indexToToggleTo == 0) {
            indexToToggleTo = EligibleTargets.Count - 1;
        } else {
            indexToToggleTo--;
        }
        ToggleEligibleTarget(indexToToggleTo);
    }
}
 