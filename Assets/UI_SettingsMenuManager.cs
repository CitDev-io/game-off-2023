using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SettingsMenuManager : MonoBehaviour
{
    [Header("Temp Plumbing")]
    public GameObject SwooshModal;
    public GameObject SettingsPanelUI;

    bool IsMoving = false;
    float TargetRotation = 90f;
    void Awake() {
        SwooshModal.SetActive(false);
        SettingsPanelUI.SetActive(false);
    }

    void ToggleRotationDirection() {
        if (TargetRotation == 90f) {
            TargetRotation = 270f;
        } else {
            TargetRotation = 90f;
        }
    }

    public void OnClick() {
        ToggleRotationDirection();
        if (!IsMoving) {
            StartCoroutine(DoRotation());
        }
    }

    public float SPINSPEED = 0.145f;
    public float WAITSPEED = 0.05f;
    IEnumerator DoRotation() {
        SwooshModal.SetActive(true);
        IsMoving = true;

        while (Mathf.Abs(SwooshModal.transform.rotation.eulerAngles.z - TargetRotation) > 0.5f) {
            SwooshModal.transform.rotation = Quaternion.Lerp(SwooshModal.transform.rotation, Quaternion.Euler(0f, 0f, TargetRotation+0.1f), SPINSPEED);

            SettingsPanelUI.SetActive(SwooshModal.transform.rotation.eulerAngles.z > 200f);

            yield return new WaitForSeconds(WAITSPEED);
        }
        SwooshModal.transform.rotation = Quaternion.Euler(0f, 0f, TargetRotation);
        if (TargetRotation == 0f) {
            SwooshModal.SetActive(false);
        }
        IsMoving = false;
    }
}
