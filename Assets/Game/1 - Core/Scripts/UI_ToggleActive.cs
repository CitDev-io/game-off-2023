using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ToggleActive : MonoBehaviour
{
    public void ToggleActive() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
