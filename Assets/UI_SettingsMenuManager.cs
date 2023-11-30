using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SettingsMenuManager : MonoBehaviour
{
    [Header("Temp Plumbing")]
    public GameObject SwooshModal;
    public GameObject SettingsPanelUI;

    bool IsMoving = false;
    float TargetRotation = 270f;
    void Awake() {
        SwooshModal.SetActive(false);
        SettingsPanelUI.SetActive(false);
    }

    void ToggleRotationDirection() {
        if (TargetRotation == 90.1f) {
            TargetRotation = 270f;
        } else {
            TargetRotation = 90.1f;
        }
    }

    public void OnClick() {
        FindFirstObjectByType<GameController_DDOL>().PlaySound("Menu_Navigate");
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

        while (Mathf.Abs(SwooshModal.transform.rotation.eulerAngles.z - TargetRotation) > 10f) {
            SwooshModal.transform.rotation = Quaternion.Euler(0f, 0f, SwooshModal.transform.rotation.eulerAngles.z - SPINSPEED);

            SettingsPanelUI.SetActive(SwooshModal.transform.rotation.eulerAngles.z > 70f && SwooshModal.transform.rotation.eulerAngles.z < 160);

            yield return new WaitForSeconds(WAITSPEED);
        }
        SwooshModal.transform.rotation = Quaternion.Euler(0f, 0f, TargetRotation);
        if (TargetRotation == 270f) {
            SwooshModal.SetActive(false);
        }
        IsMoving = false;
    }
}
