using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PortraitController : MonoBehaviour
{
    public Image HPFillBar;
    public Image SPFillBar;
    public GameObject StaggerRoot;
    public Image PortraitArt;
    public Image AffinityArt;
    public TextMeshProUGUI NameText;

    public Sprite LightAffinityArt;
    public Sprite ShadowAffinityArt;
    public Sprite NoPortraitArt;

    public Character Character;

    public void SetCharacter(Character character) {
        Character = character;
    }

    void FixedUpdate() {
        if (Character == null) {
            return;
        }

        NameText.text = Character.Config.Name;
        if (Character.Config.Portrait != null) {
            PortraitArt.sprite = Character.Config.Portrait;
        } else {
            PortraitArt.sprite = NoPortraitArt;
        }

        if (Character.Config.PowerType == PowerType.LIGHT) {
            AffinityArt.sprite = LightAffinityArt;
        } else {
            AffinityArt.sprite = ShadowAffinityArt;
        }
        HPFillBar.rectTransform.sizeDelta = new Vector2(62f * (
            (float)Character.currentHealth / (float)Character.Config.BaseHP), HPFillBar.rectTransform.sizeDelta.y
        );
        if (Character.Config.BaseSP == 0) {
            StaggerRoot.gameObject.SetActive(false);
        } else {
            StaggerRoot.gameObject.SetActive(true);
            SPFillBar.rectTransform.sizeDelta = new Vector2(62f *
                ((float)Character.currentStagger / (float)Character.Config.BaseSP), SPFillBar.rectTransform.sizeDelta.y);
        }
    }
}
