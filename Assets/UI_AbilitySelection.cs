
using UnityEngine;
using UnityEngine.UI;

public class UI_AbilitySelection : MonoBehaviour
{

    [SerializeField]
    public GameObject BasicAttackBox;
    [SerializeField]
    public GameObject SpecialAttackBox;

   
   public void ToggleAvailableAbilities(bool basic, bool special) {
    Debug.Log(basic + " " + special);
        BasicAttackBox.SetActive(basic);
        SpecialAttackBox.SetActive(special);
   }

   public bool CurrentlySelectedisBasic() {
        return BasicAttackBox.GetComponent<Image>().enabled;
   }

    public void Toggle() {
        ToggleSelectedAbility(!CurrentlySelectedisBasic());
    }

    public void ToggleSelectedAbility(bool isBasic) {
        if (isBasic) {
            BasicAttackBox.GetComponent<Image>().enabled = true;
            SpecialAttackBox.GetComponent<Image>().enabled = false;
        } else if (SpecialAttackBox.activeSelf) {
            BasicAttackBox.GetComponent<Image>().enabled = false;
            SpecialAttackBox.GetComponent<Image>().enabled = true;
        }
    }

    public void ToggleAndReportAbility(bool isBasic) {
        ToggleSelectedAbility(isBasic);
        GameObject.Find("GameManager").GetComponent<UIManager>().AbilitySelected(isBasic);
    }
}
