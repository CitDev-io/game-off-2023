using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoonOptionMgr : MonoBehaviour
{
    public Sprite Paladin;
    public Sprite Rogue;
    public Sprite Priest;
    public Sprite Warlock;
    public Image _bgImg;
    public GameObject Cursor;


    public Image Portrait;
    public TextMeshProUGUI AbilityName;
    public TextMeshProUGUI AbilityDescription;
    public TextMeshProUGUI UpgradeTypeText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI UpgradeDescription;


    public void SetContent(BaseBoonResolver bbr) {
        AbilityName.text = bbr.Name;
        AbilityDescription.text = bbr.Description;
        UpgradeTypeText.text = bbr.UpgradeType;
        LevelText.text = bbr.Level;
        UpgradeDescription.text = bbr.UpgradeDescription;

        switch (bbr.Character) {
            case PCAdventureClassType.PALADIN:
                Portrait.sprite = Paladin;
                break;
            case PCAdventureClassType.ROGUE:
                Portrait.sprite = Rogue;
                break;
            case PCAdventureClassType.PRIEST:
                Portrait.sprite = Priest;
                break;
            case PCAdventureClassType.WARLOCK:
                Portrait.sprite = Warlock;
                break;
        }
    }

    public void SetSelected(bool selected) {
        if (selected) {
            _bgImg.color = new Color(0.9254f, 0.5882f, 0.0667f, 0.7960f);
            Cursor.SetActive(true);
        } else {
            _bgImg.color = new Color(1f, 1f, 1f, 0.7960f);
            Cursor.SetActive(false);
        }
    }

}
