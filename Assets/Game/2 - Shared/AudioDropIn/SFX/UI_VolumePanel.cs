using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_VolumePanel : MonoBehaviour, IPointerClickHandler
{
    bool showPanel = false;
    [SerializeField] public GameObject volumeSelectionPanel;
    GameController_DDOL _gc;

    void Start() {
        _gc = FindObjectOfType<GameController_DDOL>();
        SetIconToVolumeLevel(_gc.currentVolume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TogglePanel();
    }

    void TogglePanel() {
        _gc.PlaySound("Menu_Navigate");
        showPanel = !showPanel;
        volumeSelectionPanel.SetActive(showPanel);
    }

    public void ChooseVolumeLevel(int volume) {
        _gc.SetSoundLevel(volume);
        SetIconToVolumeLevel(volume);
        if (showPanel) { TogglePanel(); }
    }

    void SetIconToVolumeLevel(int volume) {
        GetComponent<Image>().sprite = (Sprite) Resources.Load("Icons/sound" + volume, typeof(Sprite));
    }
}
