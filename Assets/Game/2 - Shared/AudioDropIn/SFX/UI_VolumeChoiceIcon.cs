using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_VolumeChoiceIcon : MonoBehaviour, IPointerClickHandler
{ 
    [SerializeField] public int volumeLevel;
    UI_VolumePanel volumePanel;

    void Start() {
        volumePanel = Object.FindObjectOfType<UI_VolumePanel>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetVolume();
    }

    void SetVolume(){
        volumePanel.ChooseVolumeLevel(volumeLevel);
    }
}