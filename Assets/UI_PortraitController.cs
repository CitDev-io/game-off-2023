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
    public bool ShowPointer = true;

    public Sprite LightAffinityArt;
    public Sprite ShadowAffinityArt;
    public Sprite NoPortraitArt;

    public Character Character;

    [Header("Peer Objects")]
    public GameObject Pointer;

    [Header("Buff Icons")]
    public Image Buff1;
    public Image Buff2;
    public Image Buff3;
    public Image Buff4;
    public Image Buff5;

    public void SetCharacter(Character character) {
        Character = character;
    }

    void FixedUpdate() {
        if (Character == null) {
            return;
        }

        NameText.text = Character.gameObject.name;
        if (Character.AlternativePortrait != null) {
            PortraitArt.sprite = Character.AlternativePortrait;
        } else if (Character.Config.Portrait != null) {
            PortraitArt.sprite = Character.Config.Portrait;
        } else {
            PortraitArt.sprite = NoPortraitArt;
        }

        if (Character.Config.PowerType == PowerType.LIGHT) {
            AffinityArt.sprite = LightAffinityArt;
        } else {
            AffinityArt.sprite = ShadowAffinityArt;
        }
        HPFillBar.rectTransform.sizeDelta = new Vector2(166f * (
            (float)Character.currentHealth / (float)Character.Config.BaseHP), HPFillBar.rectTransform.sizeDelta.y
        );
        if (Character.Config.BaseSP == 0) {
            StaggerRoot.gameObject.SetActive(false);
        } else {
            StaggerRoot.gameObject.SetActive(true);
            SPFillBar.rectTransform.sizeDelta = new Vector2(166f *
                ((float)Character.currentStagger / (float)Character.Config.BaseSP), SPFillBar.rectTransform.sizeDelta.y);
        }

        if (ShowPointer) {
            Pointer.SetActive(Character.IsHighlighted);
        }


        Sprite blank = Resources.Load<Sprite>("bufficons/blank");

        if (Character.Buffs.Count > 0) {
            Buff1.sprite = Resources.Load<Sprite>(Character.Buffs[0].PortraitArt);
            if (Character.Buffs[0].isDebuff) {
                Buff1.color = Color.red;
            } else {
                Buff1.color = Color.white;
            }
        } else {
            Buff1.sprite = blank;
        }

        if (Character.Buffs.Count > 1) {
            Buff2.sprite = Resources.Load<Sprite>(Character.Buffs[1].PortraitArt);
            if (Character.Buffs[1].isDebuff) {
                Buff2.color = Color.red;
            } else {
                Buff2.color = Color.white;
            }
        } else {
            Buff2.sprite = blank;
        }

        if (Character.Buffs.Count > 2) {
            Buff3.sprite = Resources.Load<Sprite>(Character.Buffs[2].PortraitArt);
            if (Character.Buffs[2].isDebuff) {
                Buff3.color = Color.red;
            } else {
                Buff3.color = Color.white;
            }
        } else {
            Buff3.sprite = blank;
        }

        if (Character.Buffs.Count > 3) {
            Buff4.sprite = Resources.Load<Sprite>(Character.Buffs[3].PortraitArt);
            if (Character.Buffs[3].isDebuff) {
                Buff4.color = Color.red;
            } else {
                Buff4.color = Color.white;
            }
        } else {
            Buff4.sprite = blank;
        }

        if (Character.Buffs.Count > 4) {
            Buff5.sprite = Resources.Load<Sprite>(Character.Buffs[4].PortraitArt);
            if (Character.Buffs[4].isDebuff) {
                Buff5.color = Color.red;
            } else {
                Buff5.color = Color.white;
            }
        } else {
            Buff5.sprite = blank;
        }
    }
}
