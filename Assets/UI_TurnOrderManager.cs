using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class UI_TurnOrderManager : MonoBehaviour
{
    // public TextMeshProUGUI Flavor1;
    // public TextMeshProUGUI Flavor2;
    // public TextMeshProUGUI Flavor3;
    public Character CurrentSelection;
    List<Character> EligibleTargets = new List<Character>();
    public UI_PortraitController TopPortrait;
    public List<UI_PortraitController> Portraits;
    public bool SelectionModeOn = false;
    public void UpdateTurnOrder(Character CurrentCharacter, List<Character> QueuedCharacters)
    {
        if (CurrentCharacter == null) {
            return;
        }

        TopPortrait.SetCharacter(CurrentCharacter);

        for (int i = 0; i < Portraits.Count; i++)
        {
            if (i >= QueuedCharacters.Count)
            {
                Portraits[i].SetCharacter(null);
                Portraits[i].gameObject.SetActive(false);
                continue;
            }

            Portraits[i].gameObject.SetActive(true);
            Portraits[i].SetCharacter(QueuedCharacters[i]);
        }
    }

    Character findEligibleToTheLeft() {
        List<UI_PortraitController> selectablePortraits = Portraits.FindAll(p => EligibleTargets.Contains(p.Character)).OrderBy(p => Portraits.IndexOf(p)).ToList();

        if (selectablePortraits.Count == 0) {
            Debug.Log("NO SELECTABLE PORTRAITS");
            return null;
        }

        int currentIndex = selectablePortraits.IndexOf(selectablePortraits.Find(p => p.Character == CurrentSelection));
        if (currentIndex == 0) {
            return selectablePortraits[selectablePortraits.Count - 1].Character;
        } else {
            return selectablePortraits[currentIndex - 1].Character;
        }
    }

    Character findEligibleToTheRight() {
        List<UI_PortraitController> selectablePortraits = Portraits.FindAll(p => EligibleTargets.Contains(p.Character)).OrderBy(p => Portraits.IndexOf(p)).ToList();

        if (selectablePortraits.Count == 0) {
            Debug.Log("NO SELECTABLE PORTRAITS");
            return null;
        }

        int currentIndex = selectablePortraits.IndexOf(selectablePortraits.Find(p => p.Character == CurrentSelection));
        if (currentIndex == selectablePortraits.Count - 1) {
            return selectablePortraits[0].Character;
        } else {
            return selectablePortraits[currentIndex + 1].Character;
        }
    }

    public void ToggleRight() {
        Character next = findEligibleToTheLeft();
        if (next == null) return;
        FindFirstObjectByType<GameController_DDOL>().PlaySound("Menu_Navigate");
        CurrentSelection.IsHighlighted = false;

        CurrentSelection = next;
        CurrentSelection.IsHighlighted = true;
    }

    public void ToggleLeft() {
        Character next = findEligibleToTheRight();
        if (next == null) return;
        FindFirstObjectByType<GameController_DDOL>().PlaySound("Menu_Navigate");
        CurrentSelection.IsHighlighted = false;

        CurrentSelection = next;
        CurrentSelection.IsHighlighted = true;
    }

    public void SetSelectionMode(bool on) {
        SelectionModeOn = on;
        if (on && EligibleTargets.Count > 0) {
            CurrentSelection = EligibleTargets[0];
            CurrentSelection.IsHighlighted = true;
        }
        if (!on) {
            CurrentSelection = null;
            EligibleTargets.ForEach(t => t.IsHighlighted = false);
            EligibleTargets.Clear();
        }
    }

    public void SetEligibleTargets(List<Character> targets) {
        EligibleTargets = targets;
        if (EligibleTargets.Count == 0) return;

        // CurrentSelection = EligibleTargets[0];
        // CurrentSelection.IsHighlighted = true;
    }

    public void PortraitClickReport(int portraitIndex) {
        if (EligibleTargets.Contains(Portraits[portraitIndex].Character)) {
            CurrentSelection.IsHighlighted = false;
            CurrentSelection = Portraits[portraitIndex].Character;
            CurrentSelection.IsHighlighted = true;

            GameObject.Find("GameManager").GetComponent<UIManager>().TargetSelected(Portraits[portraitIndex].Character);
        }
    }

    public void ClearSelection() {
        CurrentSelection.IsHighlighted = false;
        CurrentSelection = null;
    }

    public void CurrentPortraitClickReports() {
        if (EligibleTargets.Contains(TopPortrait.Character)) {
            CurrentSelection.IsHighlighted = false;
            CurrentSelection = TopPortrait.Character;
            CurrentSelection.IsHighlighted = true;

            GameObject.Find("GameManager").GetComponent<UIManager>().TargetSelected(CurrentSelection);
        }
    }
}
