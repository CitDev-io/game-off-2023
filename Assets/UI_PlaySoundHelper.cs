using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PlaySoundHelper : MonoBehaviour
{
    public void PlaySound(string name) {
        FindFirstObjectByType<GameController_DDOL>()?.PlaySound(name);
    }
}
