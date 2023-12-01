using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WinPanel : MonoBehaviour
{
    public UI_StarWarsScroll scroller;
    public void StartItUp()
    {
        scroller.Scroll();
    }
}
