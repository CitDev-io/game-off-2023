using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MusicToggleButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Sprite musicOnIcon;
    [SerializeField] Sprite musicOffIcon;
    GameController_DDOL _gc;


    void Start()
    {
        _gc = Object.FindObjectOfType<GameController_DDOL>();
        DisplayMusicAs(_gc.musicOn);
    }

    public void ToggleMusic()
    {
        bool setTo = !_gc.musicOn;
        DisplayMusicAs(setTo);
        _gc.PlaySound("Menu_Select");
        _gc.SetMusicToggle(setTo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleMusic();
    }

    void DisplayMusicAs(bool turnItOn)
    {
        GetComponent<Image>().sprite = turnItOn ? musicOnIcon : musicOffIcon;
    }
}
