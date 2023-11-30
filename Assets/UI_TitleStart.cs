using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var ddol = FindFirstObjectByType<GameController_DDOL>();

        ddol.PlayMusic("Intro1");
    }

}
